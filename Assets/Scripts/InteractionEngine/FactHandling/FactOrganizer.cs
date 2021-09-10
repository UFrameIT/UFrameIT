using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;
using static CommunicationEvents;

//TODO: MMT: move some functionality there
//TODO: consequent!= samestep != dependent

//PERF: avoid string as key (general: allocations & dict: hash -> colission? -> strcmp[!])
public class FactOrganizer
{
    protected internal Dictionary<string, Fact> FactDict;
    protected internal Dictionary<string, meta> MetaInf = new Dictionary<string, meta>();
    protected internal List<stepnote> Workflow = new List<stepnote>();
    // notes position in Workflow for un-/redo; the pointed to element is non-acitve
    protected internal int marker = 0;
    // backlock logic for convinience
    protected internal int worksteps = 0;
    protected internal int backlog = 0;
    // set if recently been resetted
    protected internal bool soft_resetted = false;
    // InvokeEvents?
    public bool invoke;
    // TODO? SE: better seperation
    // Label Managment; Communicates with Facts
    protected internal int MaxLabelId = 0;
    protected internal SortedSet<int> UnusedLabelIds = new SortedSet<int>();


    private static List<Directories>
        hierState = new List<Directories> { Directories.FactStateMachines };

    protected internal struct stepnote
    {
        // Fact.Id
        public string Id;
        // true if this Fact has been created in the same step as the last one
        //      steproot[false] (=> steptail[true])*
        public bool samestep;
        // reference to steproot/ after steptail-end
        public int steplink;
        // distincts creation and deletion
        public bool creation;


        public stepnote(string Id, bool samestep, bool creation, FactOrganizer that)
        {
            this.Id = Id;
            this.samestep = samestep;
            this.creation = creation;

            if (samestep)
            // steplink = !first_steptail ? previous.steplink : steproot
            {
                stepnote prev = that.Workflow[that.marker - 1];
                this.steplink = prev.samestep ? prev.steplink : that.marker - 1;
            }
            else
                // steproot sets steplink after itself (end of steptail)
                this.steplink = that.marker + 1;

        }
    }

    protected internal struct meta
    {
        // TODO? -> public int last_occurence for safe_dependencies

        // reference to first occurrence in Workflow
        public int workflow_id;
        // keeps track wether Fact is currently in Scene
        public bool active;

        public meta(int workflow_id, bool active = true)
        {
            this.workflow_id = workflow_id;
            this.active = active;
        }
    }

    public FactOrganizer(bool invoke = false)
    {
        FactDict = new Dictionary<string, Fact>();
        this.invoke = invoke;
    }

    private static void FactOrganizerFromPublic(ref FactOrganizer set, PublicFactOrganizer exposed, bool invoke, out Dictionary<string, string> old_to_new)
    {
        // TODO: other strategy needed when MMT save/load supported
        // map old URIs to new ones
        old_to_new = new Dictionary<string, string>();
        // combine T:Fact to Fact
        Dictionary<string, Fact> old_FactDict = new Dictionary<string, Fact>();

        /*
        FieldInfo[] finfos = typeof(PublicFactOrganizer).GetFields();
        foreach(string type in PublicFactOrganizer.WatchedFacts)
            AddListToDict(
                finfos.First(x => x.Name.Remove(x.Name.Length-1) == type)
                .GetValue(exposed)
                as List<Fact>);
        */
        
        AddListToDict(exposed.PointFacts);
        AddListToDict(exposed.LineFacts);
        AddListToDict(exposed.RayFacts);
        AddListToDict(exposed.AngleFacts);
        AddListToDict(exposed.OnLineFacts);


        // initiate
        set.invoke = invoke;
        set.MaxLabelId = exposed.MaxLabelId;
        set.UnusedLabelIds = exposed.UnusedLabelIds;
        set.FactDict = new Dictionary<string, Fact>();

        // work Workflow
        foreach (var sn in exposed.Workflow)
        {
            if (sn.creation)
            // Add
            {
                Fact add;
                if (old_to_new.ContainsKey(sn.Id))
                    add = set.FactDict[old_to_new[sn.Id]];
                else
                {
                    Fact old_Fact = old_FactDict[sn.Id];

                    add = old_Fact.GetType()
                        .GetConstructor(new Type[] { old_Fact.GetType(), old_to_new.GetType(), typeof(FactOrganizer) })
                        .Invoke(new object[] { old_Fact, old_to_new, set })
                        as Fact;

                    old_to_new.Add(sn.Id, add.Id);
                }

                set.Add(add, out _, sn.samestep);
            }
            else if(old_to_new.ContainsKey(sn.Id))
            // Remove
            {
                Fact remove = set.FactDict[old_to_new[sn.Id]];
                set.Remove(remove, sn.samestep);
            }
        }

        // set un-redo state
        while (set.backlog < exposed.backlog)
            set.undo();

        set.soft_resetted = exposed.soft_resetted;


        // === local functions ===

        void AddListToDict<T>(List<T> list) where T:Fact
        {
            foreach (T ft in list)
                old_FactDict.Add(ft.Id, ft);
        }
    }

    public Fact this[string id]
    {
        get { return FactDict[id]; }
    }

    public bool ContainsKey(string id)
    {
        return FactDict.ContainsKey(id);
    }

    public bool ContainsLabel(string label)
    {
        if (string.IsNullOrEmpty(label))
            return false;

        var hit = FactDict.FirstOrDefault(e => e.Value.Label == label);
        return !hit.Equals(System.Activator.CreateInstance(hit.GetType()));
    }

    //TODO? MMT? PERF: O(n), every Fact-insertion
    private bool FindEquivalent(Fact search, out string found, out bool exact)
    // Looks for existent facts (found) which are very similar to prposed fact (search)
    // does not check active state
    {
        if (exact = FactDict.ContainsKey(search.Id))
        {
            found = search.Id;
            return true;
        }

        foreach (var entry in FactDict)
        {
            if (entry.Value.Equivalent(search))
            {
                found = entry.Key;
                return true;
            }
        }

        found = null;
        return false;
    }

    private void WorkflowAdd(stepnote note)
    // prunes & adds Workflow; Invokes Events
    {
        PruneWorkflow();

        if (note.samestep)
        // update steplink of steproot
        {
            stepnote tmp = Workflow[note.steplink];
            tmp.steplink = Workflow.Count + 1;
            Workflow[note.steplink] = tmp;
        }
        else
            worksteps++;

        Workflow.Add(note);
        marker = Workflow.Count;

        InvokeFactEvent(note.creation, note.Id);
    }

    private void PruneWorkflow()
    // set current (displayed) state in stone; resets un-redo parameters
    {
        /*if (soft_resetted)
            this.hardreset(false); // musn't clear

        else*/ if (backlog > 0)
        {
            worksteps -= backlog;
            backlog = 0;

            for (int i = Workflow.Count - 1; i >= marker; i--)
            // cleanup now obsolete Facts
            {
                stepnote last = Workflow[i];

                if (last.creation // may be zombie
                 && MetaInf[last.Id].workflow_id == i)
                // remove for good, if original creation gets pruned
                {
                    this[last.Id].delete();
                    FactDict.Remove(last.Id);
                    MetaInf.Remove(last.Id);
                }
            }

            // prune Worklfow down to marker
            Workflow.RemoveRange(marker, Workflow.Count - marker);
        }
    }

    public string Add(Fact value, out bool exists, bool samestep = false)
    // also checks for duplicates and active state
    // returns key of actual Fact
    {
        soft_resetted = false;
        string key;

        if (exists = FindEquivalent(value, out key, out bool exact))
        {
            if (!exact)
                // no longer needed
                value.delete();

            if (MetaInf[key].workflow_id >= marker)
            // check for zombie-status
            {
                // protect zombie from beeing pruned
                var zombie = Workflow[MetaInf[key].workflow_id];
                zombie.creation = false;
                Workflow[MetaInf[key].workflow_id] = zombie;
                // set new init location
                MetaInf[key] = new meta(marker, true);
            }
            // zombies are undead!
            else if (MetaInf[key].active)
                // desired outcome already achieved
                return key;

        }
        else
        // brand new Fact
        {
            key = value.Id;
            FactDict.Add(key, value);
            MetaInf.Add(key, new meta(marker, true));
        }

        WorkflowAdd(new stepnote(key, samestep, true, this));
        return key;
    }

    public bool Remove(Fact value, bool samestep = false)
    {
        return this.Remove(value.Id, samestep);
    }

    public bool Remove(string key, bool samestep = false)
    //no reset check needed (impossible state)
    {
        if (!FactDict.ContainsKey(key))
            return false;

        //TODO: see issue #58

        safe_dependencies(key, out List<string> deletethis);

        if(deletethis.Count > 0)
        {
            yeetusdeletus(deletethis, samestep);
        }

        return true;
    }

    // TODO: MMT: decide dependencies there (remember virtual deletions in Unity (un-redo)!)
    // TODO? decrease runtime from O(n/2)
    public bool safe_dependencies(string key, out List<string> dependencies)
    // searches for dependencies of a Fact; returns false if any dependencies are steproots
    // int key: Fact to be deleted
    // out List<int> dependencies: dependencyList
    {
        dependencies = new List<string>();
        int c_unsafe = 0;

        int pos = MetaInf[key].workflow_id;
        dependencies.Add(key);

        // accumulate facts that are dependent of dependencies
        for (int i = pos; i < marker; i++)
        {
            // TODO: consequent != samestep != dependent (want !consequent)
            if (!Workflow[i].creation)
            {
                // just try
                if (dependencies.Remove(Workflow[i].Id) && !Workflow[i].samestep)
                    c_unsafe--;
            }
            else if (0 < this[Workflow[i].Id].getDependentFactIds().Intersect(dependencies).Count())
            {
                dependencies.Add(Workflow[i].Id);
                if (!Workflow[i].samestep)
                    c_unsafe++;
            }
        }

        return c_unsafe == 0;
    }

    private void yeetusdeletus(List<string> deletereverse, bool samestep = false)
    {
        for(int i = deletereverse.Count - 1; i >= 0; i--, samestep = true)
        {
            WorkflowAdd(new stepnote(deletereverse[i], samestep, false, this));
        }
    }

    private void reversestep(int pos, bool samestep = false)
    // reverses any entire step; adds process to Workflow!
    // int pos: position after steptail-end of the step to be reversed
    {
        pos--;

        if (pos >= marker)
        // check for valid step (implicit reset check)
            return;
        
        for (int i = pos, stop = Workflow[pos].samestep ? Workflow[pos].steplink : pos;
            i >= stop; i--, samestep = true)
        {
            if (Workflow[i].creation)
                Remove(Workflow[i].Id, samestep);
            else
                WorkflowAdd(new stepnote(Workflow[i].Id, samestep, true, this));
        }
    }

    public void undo()
    {
        if (soft_resetted)
            fastforward(); // revert softreset

        else if (backlog < worksteps) {
            backlog++;

            stepnote last = Workflow[--marker];
            int stop = last.samestep ? last.steplink : marker;
            for (int i = marker; i >= stop; i--)
            {
                last = Workflow[i];
                InvokeFactEvent(!last.creation, last.Id);
            }

            marker = stop;
        }
    }

    public void redo()
    {
        soft_resetted = false;

        if (backlog > 0)
        {
            backlog--;

            stepnote last = Workflow[marker];
            int stop = last.samestep ? Workflow[last.steplink].steplink : last.steplink;
            for (int i = marker; i < stop; i++)
            {
                last = Workflow[i];
                InvokeFactEvent(last.creation, last.Id);
            }

            marker = stop;
        }
    }

    public void Clear()
    // Does not Invoke RemoveFactEvent(s)!
    {
        FactDict.Clear();
        MetaInf.Clear();
        Workflow.Clear();
        marker = 0;
        worksteps = 0;
        backlog = 0;
        soft_resetted = false;
    }

    public void hardreset(bool invoke_event = true)
    {
        foreach(var entry in FactDict)
        {
            if (invoke_event && invoke && MetaInf[entry.Key].active)
                CommunicationEvents.RemoveFactEvent.Invoke(entry.Value);
            entry.Value.delete();
        }
        this.Clear();
    }

    public void softreset()
    {
        if (soft_resetted)
        {
            fastforward();
            return;
        }

        // TODO: PREF: alt: EventResetAll
        // marker = 0; backlog = worksteps;
        while (marker > 0)
            undo();

        soft_resetted = true;
    }

    public void fastforward()
    {
        while (backlog > 0)
            // also sets resetted = false;
            redo();
    }

    public void store(string name, List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierState.AsEnumerable());

        string path = CreatePathToFile(out _, name, "JSON", hierarchie, use_install_folder);
        hierarchie.RemoveRange(hierarchie.Count - hierState.Count, hierState.Count);

        // note: max depth for "this" is 2, since Fact has non-serilazible member, that is not yet ignored (see Fact.[JasonIgnore] and JSONManager.WriteToJsonFile)
        // using public dummy class to circumvent deserialiation JsonInheritanceProblem (see todos @PublicFactOrganizer)
        JSONManager.WriteToJsonFile(path, new PublicFactOrganizer(this), 0);
    }

    public static bool load(ref FactOrganizer set, bool draw, string name, List<Directories> hierarchie, bool use_install_folder, out Dictionary<string, string> old_to_new)
    {
        old_to_new = null;
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierState.AsEnumerable());

        string path = CreatePathToFile(out bool loadable, name, "JSON", hierarchie, use_install_folder);
        hierarchie.RemoveRange(hierarchie.Count - hierState.Count, hierState.Count);
        if (!loadable)
            return false;

        PublicFactOrganizer de_json = JSONManager.ReadFromJsonFile<PublicFactOrganizer>(path);
        FactOrganizerFromPublic(ref set, de_json, draw, out old_to_new);

        return true;
    }

    public void Draw(bool draw_all = false)
    // call this after assigning a stored instance in an empty world, that was not drawn
    {
        // TODO: see issue #58
        // TODO: communication with MMT

        foreach (var key in FactDict.Keys)
        {
            // update active info if needed
            meta info = MetaInf[key];
            if (info.active)
            {
                info.active = false;
                MetaInf[key] = info;
            }
        }

        marker = 0;
        var stop = draw_all ? worksteps : backlog;
        backlog = worksteps;

        while(backlog > stop)
            redo();
    }

    public void Undraw(bool force_invoke = false)
    {
        foreach (var entry in FactDict)
        {
            if (force_invoke || (invoke && MetaInf[entry.Key].active))
                CommunicationEvents.RemoveFactEvent.Invoke(entry.Value);
        }
    }

    private void InvokeFactEvent(bool creation, string Id)
    {
        // update meta struct
        meta info = MetaInf[Id];
        info.active = creation;
        MetaInf[Id] = info;

        if (invoke)
            if (creation)
                CommunicationEvents.AddFactEvent.Invoke(this[Id]);
            else
                CommunicationEvents.RemoveFactEvent.Invoke(this[Id]);

        if (creation)
        // undo freeLabel()
            _ = FactDict[Id].Label;
        else
            FactDict[Id].freeAutoLabel();
    }

    public bool StaticlySovled(List<Fact> StaticSolution, out List<Fact> MissingElements, out List<Fact> Solutions)
    // QoL for simple Levels
    {
        return DynamiclySolved(StaticSolution, out MissingElements, out Solutions, new FactEquivalentsComparer());
    }

    //TODO: PERF: see CommunicationEvents.Solution
    public bool DynamiclySolved(List<Fact> MinimalSolution, out List<Fact> MissingElements, out List<Fact> Solutions, FactComparer FactComparer)
    {
        Solutions = FactDict.Values.Where(f => MetaInf[f.Id].active)
            .Where(active => MinimalSolution.Contains(active, FactComparer.SetSearchRight()))
            .ToList();

        MissingElements = MinimalSolution.Except(Solutions, FactComparer.SetSearchLeft()).ToList();

        return MissingElements.Count == 0;
    }

    //TODO: repair
    public bool DynamiclySolvedEXP(
        SolutionOrganizer MinimalSolution,
        out List<List<string>> MissingElements,
        out List<List<string>> Solutions)
    {
        MissingElements = new List<List<string>>();
        Solutions = new List<List<string>>();

        int MissingElementsCount = 0;
        var activeList = FactDict.Values.Where(f => MetaInf[f.Id].active);

        foreach (var ValidationSet in MinimalSolution.ValidationSet)
        {
            var part_minimal = 
                ValidationSet.MasterIDs.Select(URI => MinimalSolution[URI]);

            var part_solution =
                activeList.Where(active => part_minimal.Contains(active, ValidationSet.Comparer.SetSearchRight()));

            var part_missing =
                part_minimal.Except(part_solution, ValidationSet.Comparer.SetSearchLeft());

            Solutions.Add(part_solution.Select(fact => fact.Id).ToList());
            MissingElements.Add(part_missing.Select(fact => fact.Id).ToList());

            MissingElementsCount += part_missing.Count();
        }

        return MissingElementsCount == 0;
    }

}


// TODO? PERF? SE? JsonInheritanceProblem: scrap this hardwired class and implement dynamic approach with JsonConverter (see: JSONManager.JsonInheritenceConverter)
public class PublicFactOrganizer : FactOrganizer
// public class exposing all protected members of FactOrganizer for JSON conversion
{
    // TODO? check once if those are all with reflection
    protected internal static List<string> WatchedFacts = new List<string>(new string[] {
        "PointFact",
        "LineFact",
        "RayFact",
        "OnLineFact",
        "AngleFact"
    });

    public List<PointFact> PointFacts = new List<PointFact>();
    public List<LineFact> LineFacts = new List<LineFact>();
    public List<RayFact> RayFacts = new List<RayFact>();
    public List<OnLineFact> OnLineFacts = new List<OnLineFact>();
    public List<AngleFact> AngleFacts = new List<AngleFact>();

    public new Dictionary<string, meta> MetaInf = new Dictionary<string, meta>();
    public new List<stepnote> Workflow = new List<stepnote>();
    // notes position in Workflow for un-/redo; the pointed to element is non-acitve
    public new int marker = 0;
    // backlock logic for convinience
    public new int worksteps = 0;
    public new int backlog = 0;
    // set if recently been resetted
    public new bool soft_resetted = false;
    // InvokeEvents?
    public new bool invoke;
    // TODO? SE: better seperation
    // Label Managment; Communicates with Facts
    public new int MaxLabelId = 0;
    public new SortedSet<int> UnusedLabelIds = new SortedSet<int>();

    public new struct stepnote
    {
        // Fact.Id
        public string Id;
        // true if this Fact has been created in the same step as the last one
        //      steproot[false] (=> steptail[true])*
        public bool samestep;
        // reference to steproot/ after steptail-end
        public int steplink;
        // distincts creation and deletion
        public bool creation;

        public stepnote(string Id, bool samestep, int steplink, bool creation)
        {
            this.Id = Id;
            this.samestep = samestep;
            this.steplink = steplink;
            this.creation = creation;
        }

        /*public stepnote(string Id, bool samestep, bool creation, PublicFactOrganizer that)
        {
            this.Id = Id;
            this.samestep = samestep;
            this.creation = creation;

            if (samestep)
            // steplink = !first_steptail ? previous.steplink : steproot
            {
                stepnote prev = that.Workflow[that.marker - 1];
                this.steplink = prev.samestep ? prev.steplink : that.marker - 1;
            }
            else
                // steproot sets steplink after itself (end of steptail)
                this.steplink = that.marker + 1;

        }*/
    }

    public new struct meta
    {
        // TODO? -> public int last_occurence for safe_dependencies

        // reference to first occurrence in Workflow
        public int workflow_id;
        // keeps track wether Fact is currently in Scene
        public bool active;

        public meta(int workflow_id, bool active)
        {
            this.workflow_id = workflow_id;
            this.active = active;
        }
    }

    public PublicFactOrganizer()
    {
        FactDict = new Dictionary<string, Fact>();
        this.invoke = false;
    }

    protected internal PublicFactOrganizer(FactOrganizer expose)
    {
        // expose all non-abstract members
        marker = expose.marker;
        worksteps = expose.worksteps;
        backlog = expose.backlog;
        soft_resetted = expose.soft_resetted;
        invoke = expose.invoke;
        MaxLabelId = expose.MaxLabelId;
        UnusedLabelIds = expose.UnusedLabelIds;

        foreach (var sn in expose.Workflow)
            Workflow.Add(new stepnote(sn.Id, sn.samestep, sn.steplink, sn.creation));

        foreach (var mt in expose.MetaInf)
            MetaInf.Add(mt.Key, new meta(mt.Value.workflow_id, mt.Value.active));

        // expose and deserialize all abstract members
        foreach (var fc in expose.FactDict.Values)
        // keys are Fact.Id
        {
            switch (fc.GetType().Name)
            {
                case "PointFact":
                    PointFacts.Add(fc as PointFact);
                    break;
                case "LineFact":
                    LineFacts.Add(fc as LineFact);
                    break;
                case "RayFact":
                    RayFacts.Add(fc as RayFact);
                    break;
                case "OnLineFact":
                    OnLineFacts.Add(fc as OnLineFact);
                    break;
                case "AngleFact":
                    AngleFacts.Add(fc as AngleFact);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}