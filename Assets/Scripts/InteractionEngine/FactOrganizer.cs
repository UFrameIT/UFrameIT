using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//TODO? PERF? (often inserts) SortedDict <-> Dict (often reads)
//TODO: MMT: move some functionality there
//TODO: consequent!= samestep != dependent

//PERF: avoid string as key (general: allocations & dict: hash -> colission? -> strcmp[!])

[System.Serializable]
public class FactOrganizer
{
    private Dictionary<string, Fact> FactDict;
    private Dictionary<string, meta> MetaInf = new Dictionary<string, meta>();
    private List<stepnote> Workflow = new List<stepnote>();
    // notes position in Workflow for un-/redo; the pointed to element is non-acitve
    private int marker = 0;
    // backlock logic for convinience
    private int worksteps = 0;
    private int backlog = 0;
    // set if recently been resetted
    private bool soft_resetted = false;
    // InvokeEvents?
    private bool invoke;

    private struct stepnote
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

    private struct meta
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

    public FactOrganizer(IDictionary<string, Fact> dictionary, bool invoke = false)
    {
        FactDict = new Dictionary<string, Fact>(dictionary);
        this.invoke = invoke;
    }

    public Fact this[string id]
    {
        get{ return FactDict[id]; }
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

    public void store()
    {
        // TODO: save state of all of this?
        // probably nothing:
        //      safe class instance somewhere
    }

    public void load(bool draw_all = true)
    // call this after assigning a stored instance in an empty world
    {
        // TODO: see issue #58
        // TODO: communication with MMT

        foreach (var mett in MetaInf)
        {
            // update active info if needed
            if (mett.Value.active)
            {
                meta info = mett.Value;
                info.active = false;
                MetaInf[mett.Key] = info;
            }
        }

        marker = 0;
        var stop = backlog;
        backlog = worksteps;

        if(draw_all)
            while(backlog > stop)
                redo();
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

        if (!creation)
            FactDict[Id].freeLabel();
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

}