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
//TODO: support renamne functionality

//PERF: avoid string as key (general: allocations & dict: hash -> colission? -> strcmp[!])

/// <summary>
/// Organizes (insertion/ deletion / etc. operations) and sepperates <see cref="Fact">Fact</see> spaces.
/// Keeps track of insertion/ deletion actions for <see cref="undo"/> and <see cref="redo"/>.
/// </summary>
public class FactOrganizer
{
    /// <summary>
    /// - <c>Key</c>: <see cref="Fact.Id"/>
    /// - <c>Value</c>: <see cref="Fact"/>
    /// </summary>
    protected internal Dictionary<string, Fact> FactDict;

    /// <summary>
    /// - <c>Key</c>: <see cref="Fact.Id"/>
    /// - <c>Value</c>: <see cref="meta"/>
    /// </summary>
    protected internal Dictionary<string, meta> MetaInf = new Dictionary<string, meta>();

    /// <summary>
    /// Keeps track of insertion/ deletion/ etc. operations for <see cref="undo"/> and <see cref="redo"/>
    /// </summary>
    protected internal List<stepnote> Workflow = new List<stepnote>();

    /// <summary>
    /// Notes position in <see cref="Workflow"/> for <see cref="undo"/> and <see cref="redo"/>; the pointed to element is non-acitve
    /// </summary>
    protected internal int marker = 0;

    /// <summary>
    /// Backlock logic redundant - for convinience.
    /// Keeps track of number of steps in <see cref="Workflow"/>.
    /// One step can consist of multiple operations.
    /// <seealso cref="stepnote"/>
    /// </summary>
    protected internal int worksteps = 0;
    /// <summary>
    /// Backlock logic redundant - for convinience.
    /// Keeps track of number of steps in <see cref="Workflow"/>, which are not set active.
    /// One step can consist of multiple operations.
    /// <seealso cref="stepnote"/>
    /// </summary>
    protected internal int backlog = 0;

    /// <summary>
    /// Set to <c>true</c> if recently been resetted.
    /// </summary>
    protected internal bool soft_resetted = false;

    /// <summary>
    /// If set to <c>true</c>, <see cref="Remove(string, bool)"/> and <see cref="Add(Fact, out bool, bool)"/> will invoke <see cref="CommunicationEvents.RemoveFactEvent"/> and <see cref="CommunicationEvents.AddFactEvent"/> respectively.
    /// </summary>
    public bool invoke;

    // TODO? SE: better seperation
    /// <summary>
    /// Keeps track of maximum <see cref="Fact.LabelId"/> for <see cref="Fact.generateLabel"/>.
    /// </summary>
    protected internal int MaxLabelId = 0;
    /// <summary>
    /// Stores unused <see cref="Fact.LabelId"/> for <see cref="Fact.generateLabel"/>, wich were freed in <see cref="Fact.freeAutoLabel"/> for later reuse to keep naming space compact.
    /// </summary>
    protected internal SortedSet<int> UnusedLabelIds = new SortedSet<int>();

    // TODO: put this stuff in Interface
    /// @{ <summary>
    /// For <see cref="store(string, List<Directories>, bool, bool)"/> and <see cref="load(ref FactOrganizer, bool, string, List<Directories>, bool, out Dictionary<string, string>)"/>
    /// </summary>
    private string path = null;
    private static List<Directories>
        hierState = new List<Directories> { Directories.FactStateMachines };
    /// @}


    /// <summary>
    /// Keeps track of insertion/ deletion/ etc. operations for <see cref="undo"/> and <see cref="redo"/>
    /// Used Terminology
    /// ================
    /// - steproot: elements where <see cref="samestep"/> == <c>true</c>
    /// - steptail: elements where <see cref="samestep"/> == <c>false</c>
    /// <seealso cref="Workflow"/>
    /// </summary>
    protected internal struct stepnote
    {
        /// <summary> <see cref="Fact.Id"/> </summary>
        public string Id;

        /// <summary>
        /// <c>true</c> if this Fact has been created in the same step as the last one 
        ///      steproot[false] (=> steptail[true])*
        /// </summary>
        public bool samestep;

        /// <summary>
        /// For fast access of beginning and end of steps.
        /// Reference to position in <see cref="Workflow"/> of:
        /// - steproot: for all elements in steptail
        /// - after steptail-end: for steproot
        /// </summary>
        public int steplink;

        /// <summary> distincts creation and deletion </summary>
        public bool creation;


        /// <summary>
        /// Initiator
        /// </summary>
        /// <param name="Id"><see cref="Fact.Id"/></param>
        /// <param name="samestep">sets <see cref="samestep"/></param>
        /// <param name="creation">sets <see cref="creation"/></param>
        /// <param name="that"><see cref="FactOrganizer"/> of which <c>this</c> will be added in its <see cref="FactOrganizer.Workflow"/></param>
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

    /// <summary>
    /// Each <see cref="Fact"/> entry in <see cref="FactDict"/> has a corresponding <see cref="meta"/> entry in <see cref="MetaInf"/>.
    /// The <see cref="meta"/> struct is a collection of %meta-variables.
    /// <seealsocref="PruneWorkflow"/>
    /// </summary>
    protected internal struct meta
    {
        // TODO? -> public int last_occurence; // for safe_dependencies
        /// <summary>
        /// position of first occurrence in <see cref="Workflow"/>
        /// </summary>
        public int workflow_id;

        /// <summary>
        /// keeps track wether <see cref="Fact"/> is currently in Scene
        /// </summary>
        public bool active;

        /// <summary>
        /// Initiator
        /// </summary>
        /// <param name="workflow_id">sets <see cref="workflow_id"/></param>
        /// <param name="active">sets <see cref="active"/></param>
        public meta(int workflow_id, bool active = true)
        {
            this.workflow_id = workflow_id;
            this.active = active;
        }
    }

    /// <summary>
    /// Standard Constructor for empty, ready to use <see cref="FactOrganizer"/>
    /// </summary>
    /// <param name="invoke">sets <see cref="invoke"/>.</param>
    public FactOrganizer(bool invoke = false)
    {
        FactDict = new Dictionary<string, Fact>();
        this.invoke = invoke;
    }

    /// <summary>
    /// Used to parse <see cref="JsonReader"/>/ <see cref="JsonWriter"/> readable and creatable <see cref="PublicFactOrganizer">format</see> of this <see cref="FactOrganizer">class</see> to an actual instance of this <see cref="FactOrganizer">class</see>.
    /// <remarks>TODO: repair and use <see cref="JSONManager.JsonInheritenceConverter<T>"/> o.s. to bypass all of this _hardwired_ implementation, including the entirety of <see cref="PublicFactOrganizer"/></remarks>
    /// </summary>
    /// <param name="set">to be parsed into, will be overwritten. 
    /// If <c><paramref name="invoke"/> = true</c>, <paramref name="set"/> should be <see cref="StageStatic.stage.factState"/>, outherwise <see cref="InvokeFactEvent(bool, string)"/> will cause <see cref="Exception">Exceptions</see> when it invokes Events of <see cref="CommunicationEvents"/></param>
    /// <param name="exposed">instance to be parsed</param>
    /// <param name="invoke">see <see cref="invoke"/></param>
    /// <param name="old_to_new">generated to map <c>Key</c> <see cref="Fact.Id"/> of <paramref name="exposed"/> to corresponding <c>Value</c> <see cref="Fact.Id"/> of <paramref name="set"/></param>.
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
            else if (old_to_new.ContainsKey(sn.Id))
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

        void AddListToDict<T>(List<T> list) where T : Fact
        {
            foreach (T ft in list)
                old_FactDict.Add(ft.Id, ft);
        }
    }

    /// <summary>
    /// wrappes <c><see cref="FactDict"/>[<paramref name="id"/>]</c>
    /// <seealso cref="ContainsKey(string)"/>
    /// </summary>
    /// <param name="id">a <see cref="Fact.Id"/> in <see cref="FactDict"/></param>
    /// <returns><c><see cref="FactDict"/>[<paramref name="id"/>]</c></returns>
    public Fact this[string id]
    {
        get { return FactDict[id]; }
    }

    /// <summary>
    /// wrappes <c><see cref="FactDict"/>.ContainsKey(<paramref name="id"/>)</c>
    /// </summary>
    /// <param name="id">a <see cref="Fact.Id"/></param>
    /// <returns><c><see cref="FactDict"/>.ContainsKey(<paramref name="id"/>)</c></returns>
    public bool ContainsKey(string id)
    {
        return FactDict.ContainsKey(id);
    }

    /// <summary>
    /// Looks up if there is a <paramref name="label"/> <see cref="Fact.Label"/> in <see cref="FactDict"/>.Values
    /// </summary>
    /// <param name="label">supposed <see cref="Fact.Label"/> to be checked</param>
    /// <returns><c>true</c> iff <see cref="FactDict"/> conatains a <c>Value</c> <see cref="Fact"/>, where <see cref="Fact.Label"/> == <paramref name="label"/>.</returns>
    public bool ContainsLabel(string label)
    {
        if (string.IsNullOrEmpty(label))
            return false;

        var hit = FactDict.FirstOrDefault(e => e.Value.Label == label);
        return !hit.Equals(System.Activator.CreateInstance(hit.GetType()));
    }

    //TODO? MMT? PERF: O(n), every Fact-insertion
    /// <summary>
    /// Looks for existent <see cref="Fact"/> (<paramref name="found"/>) which is very similar or identical (<paramref name="exact"/>) to prposed <see cref="Fact"/> (<paramref name="search"/>)
    /// <remarks>does not check active state</remarks>
    /// </summary>
    /// <param name="search">to be searched for</param>
    /// <param name="found"><see cref="Fact.Id"/> if return value is <c>true</c></param>
    /// <param name="exact"><c>true</c> iff <paramref name="found"/> == <paramref name="search"/><see cref="Fact.Id">.Id</see></param>
    /// <returns><c>true</c> iff the exact same or an equivalent <see cref="Fact"/> to <paramref name="search"/> was found in <see cref="FactDict"/></returns>
    private bool FindEquivalent(Fact search, out string found, out bool exact)
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

    /// <summary>
    /// <see cref="PruneWorkflow">prunes</see> & adds <paramref name="note"/> to <see cref="Workflow"/>; <see cref="InvokeFactEvent(bool, string)">Invokes Events</see>
    /// </summary>
    /// <param name="note">to be added</param>
    private void WorkflowAdd(stepnote note)
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

    /// <summary>
    /// set current (displayed) state in stone, a.k.a. <see cref="Fact.delete(bool)">delete</see> non <see cref="meta.active"/> <see cref="Fact">Facts</see> for good;
    /// resets <see cref="undo">un</see>-<see cref="redo"/> parameters
    /// </summary>
    private void PruneWorkflow()
    {
        /*if (soft_resetted)
            this.hardreset(false); // musn't clear

        else*/
        if (backlog > 0)
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

    /// <summary>
    /// Call this to Add a <see cref="Fact"/> to <see cref="FactOrganizer">this</see> instance.
    /// <remarks>*Warning*: If return_value != <paramref name="value"/><see cref="Fact.Id">.Id</see>, <paramref name="value"/> will be <see cref="Fact.delete(bool)">deleted</see> for good to reduce ressource usage!</remarks>
    /// </summary>
    /// <param name="value">to be added</param>
    /// <param name="exists"><c>true</c> iff <paramref name="value"/> already exists (may be <see cref="meta.active">inactive</see> before opreation)</param>
    /// <param name="samestep">set <c>true</c> if <see cref="Fact"/> creation happens as a subsequent/ consequent step of multiple <see cref="Fact"/> creations and/or deletions, 
    /// and you whish that these are affected by a single <see cref="undo"/>/ <see cref="redo"/> step</param>
    /// <returns><see cref="Fact.Id"/> of <paramref name="value"/> or <see cref="FindEquivalent(Fact, out string, out bool)">found</see> <see cref="Fact"/> iff <paramref name="exists"/>==<c>true</c></returns>
    public string Add(Fact value, out bool exists, bool samestep = false)
    {
        soft_resetted = false;
#pragma warning disable IDE0018 // Inlinevariablendeklaration
        string key;
#pragma warning restore IDE0018 // Inlinevariablendeklaration

        if (exists = FindEquivalent(value, out key, out bool exact))
        {
            if (!exact)
                // no longer needed
                value.delete();

            if (MetaInf[key].workflow_id >= marker)
            // check for zombie-status: everything >= marker will be pruned
            {
                // protect zombie from beeing pruned
                var zombie = Workflow[MetaInf[key].workflow_id];
                zombie.creation = false; // this meta entry will be deleted, but will not trigger deletion
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

    /// <summary>
    /// Call this to Remove a <see cref="Fact"/> from <see cref="FactOrganizer">this</see> instance.
    /// If other <see cref="Fact">Facts</see> depend on <paramref name="value"/> <see cref="Remove(Fact, bool)">Remove(/<depending Fact/>, <c>true</c>)</see> will be called recursively/ cascadingly.
    /// </summary>
    /// <remarks>this will not <see cref="Fact.delete(bool)">delete</see> a <see cref="Fact"/>, but sets it <see cref="meta.active">inactive</see> for later <see cref="Fact.delete(bool)">deletion</see> when <see cref="PruneWorkflow">pruned</see>.</remarks>
    /// <param name="value">to be removed</param>
    /// <param name="samestep">set <c>true</c> if <see cref="Fact"/> deletion happens as a subsequent/ consequent step of multiple <see cref="Fact"/> creations and/or deletions, 
    /// and you whish that these are affected by a single <see cref="undo"/>/ <see cref="redo"/> step</param>
    /// <returns><c>true</c> iff <paramref name="value"/><see cref="Fact.Id">.Id</see> was found.</returns>
    public bool Remove(Fact value, bool samestep = false)
    {
        return this.Remove(value.Id, samestep);
    }

    /// \copybrief Remove(Fact, bool)
    /// <remarks>this will not <see cref="Fact.delete(bool)">delete</see> a <see cref="Fact"/>, but sets it <see cref="meta.active">inactive</see> for later <see cref="Fact.delete(bool)">deletion</see> when <see cref="PruneWorkflow">pruned</see>.</remarks>
    /// <param name="key">to be removed</param>
    /// <param name="samestep">set <c>true</c> if <see cref="Fact"/> deletion happens as a subsequent/ consequent step of multiple <see cref="Fact"/> creations and/or deletions, 
    /// and you whish that these are affected by a single <see cref="undo"/>/ <see cref="redo"/> step</param>
    /// <returns><c>true</c> iff <paramref name="value"/> was found.</returns>
    public bool Remove(string key, bool samestep = false)
    //no reset check needed (impossible state)
    {
        if (!FactDict.ContainsKey(key))
            return false;

        if (!MetaInf[key].active)
            // desiered outcome reality
            return true;

        //TODO: see issue #58

        safe_dependencies(key, out List<string> deletethis);

        if (deletethis.Count > 0)
        {
            yeetusdeletus(deletethis, samestep);
        }

        return true;
    }

    // TODO: MMT: decide dependencies there (remember virtual deletions in Unity (un-redo)!)
    // TODO? decrease runtime from amorised? O((n/2)^2)
    /// <summary>
    /// searches recursively for <see cref="Fact">Facts</see> where <see cref="Fact.getDependentFactIds"/> includes <paramref name="key"/>/ found dependencies
    /// </summary>
    /// <param name="key">to be cross referenced</param>
    /// <param name="dependencies">all <see cref="Fact">Facts</see> where <see cref="Fact.getDependentFactIds"/> includes <paramref name="key"/>/ found dependencies</param>
    /// <returns><c>false</c> if any dependencies are <see cref="stepnote">steproots</see></returns>
    public bool safe_dependencies(string key, out List<string> dependencies)
    {
        dependencies = new List<string>();
        int c_unsafe = 0;

        int pos = MetaInf[key].workflow_id;
        dependencies.Add(key);

        // accumulate facts that are dependent of dependencies
        for (int i = pos; i < marker; i++)
        {
            // TODO: consequent != samestep != dependent (want !consequent)
            if (!Workflow[i].creation && Workflow[i].Id != key)
            {
                // just try
                if (dependencies.Remove(Workflow[i].Id) && !Workflow[i].samestep)
                    c_unsafe--;
            }
            else if (this[Workflow[i].Id].getDependentFactIds().Intersect(dependencies).Any() && Workflow[i].Id != key)
            {
                dependencies.Add(Workflow[i].Id);
                if (!Workflow[i].samestep)
                    c_unsafe++;
            }
        }

        return c_unsafe == 0;
    }

    /// <summary>
    /// Turns every <see cref="Fact"/> in <paramref name="deletereverse"/> (in reverse order) <see cref="meta.active">inactive</see>, as it would be <see cref="Remove(string, bool)">removed</see>, but without checking for (recursive) dependencies.
    /// </summary>
    /// <param name="deletereverse">to be <see cref="Remove(string, bool)">removed</see>, but without checking for (recursive) dependencies</param>
    /// <param name="samestep">see <see cref="Remove(string, bool).samestep"/>. Only applies to last (first iteration) element of <paramref name="deletereverse"/>; for everything else <paramref name="samestep"/> will be set to <c>true</c>.</param>
    private void yeetusdeletus(List<string> deletereverse, bool samestep = false)
    {
        for (int i = deletereverse.Count - 1; i >= 0; i--, samestep = true)
        {
            WorkflowAdd(new stepnote(deletereverse[i], samestep, false, this));
        }
    }

    /// <summary>
    /// reverses any entire step; adds process to Workflow!
    /// </summary>
    /// <remarks>*Warning*: unused therefore untested and unmaintained.</remarks>
    /// <param name="pos">position after <see cref="stepnote">steptail-end</see> of the step to be reversed</param>
    /// <param name="samestep">see <see cref="yeetusdeletus(List<string>, bool).samestep"/></param>
    private void reversestep(int pos, bool samestep = false)
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
            else if (!MetaInf[Workflow[i].Id].active)
                WorkflowAdd(new stepnote(Workflow[i].Id, samestep, true, this));
        }
    }

    /// <summary>
    /// Undoes an entire <see cref="stepnote">step</see> or last <see cref="softreset"/> .
    /// No <see cref="Fact"/> will be actually <see cref="Add(Fact, out bool, bool)">added</see>, <see cref="Remove(Fact, bool)">removed</see> or <see cref="Fact.delete(bool)">deleted</see>; only its visablity and <see cref="meta.active"/> changes.
    /// <seealso cref="marker"/>
    /// <seealso cref="worksteps"/>
    /// <seealso cref="backlog"/>
    /// </summary>
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

    /// <summary>
    /// Redoes an entire <see cref="stepnote">step</see> .
    /// No <see cref="Fact"/> will be actually <see cref="Add(Fact, out bool, bool)">added</see>, <see cref="Remove(Fact, bool)">removed</see> or <see cref="Fact.delete(bool)">deleted</see>; only its visablity and <see cref="meta.active"/> changes.
    /// <seealso cref="marker"/>
    /// <seealso cref="worksteps"/>
    /// <seealso cref="backlog"/>
    /// </summary>
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

    /// <summary>
    /// Resets to "factory conditions".
    /// Neither <see cref="Fact.delete(bool)">deletes</see> <see cref="Fact">Facts</see> nor invokes <see cref="CommunicationEvents.RemoveFactEvent"/>
    /// </summary>
    /// <seealso cref="hardreset(bool)"/>
    public void Clear()
    {
        FactDict.Clear();
        MetaInf.Clear();
        Workflow.Clear();
        marker = 0;
        worksteps = 0;
        backlog = 0;
        soft_resetted = false;
    }

    /// <summary>
    /// Resets to "factory conditions".
    /// <see cref="Fact.delete(bool)">deletes</see> <see cref="Fact">Facts</see> and invokes <see cref="CommunicationEvents.RemoveFactEvent"/> iff <paramref name="invoke_event"/> && <see cref="invoke"/>.
    /// </summary>
    /// <seealso cref="Clear"/>
    /// <param name="invoke_event">if set to <c>true</c> *and* <see cref="invoke"/> set to <c>true</c> will invoke <see cref="CommunicationEvents.RemoveFactEvent"/></param>
    public void hardreset(bool invoke_event = true)
    {
        foreach (var entry in FactDict)
        {
            if (invoke_event && invoke && MetaInf[entry.Key].active)
                CommunicationEvents.RemoveFactEvent.Invoke(entry.Value);
            entry.Value.delete();
        }
        this.Clear();
    }

    /// <summary>
    /// <see cref="undo">Undoes</see> *all* <see cref="worksteps"/> (since <see cref="marker"/>) and sets <see cref="soft_resetted"/> to <c>true</c>.
    /// </summary>
    public void softreset()
    {
        if (soft_resetted)
        {
            fastforward();
            return;
        }

        while (marker > 0)
            undo();
        // marker = 0; backlog = worksteps;

        soft_resetted = true;
    }

    /// <summary>
    /// <see cref="redo">Redoes</see> *all* <see cref="worksteps"/> (from <see cref="marker"/> onwards) and sets <see cref="soft_resetted"/> to <c>false</c>.
    /// </summary>
    public void fastforward()
    {
        while (backlog > 0)
            // also sets resetted = false;
            redo();
    }

    /// @{ 
    /// TODO? move to interface?
    /// TODO: document
    public void store(string name, List<Directories> hierarchie = null, bool use_install_folder = false, bool overwrite = true)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierState.AsEnumerable());

        string path_o = path;
        path = CreatePathToFile(out bool exists, name, "JSON", hierarchie, use_install_folder);

        hierarchie.RemoveRange(hierarchie.Count - hierState.Count, hierState.Count);

        // note: max depth for "this" is 2, since Fact has non-serilazible member, that is not yet ignored (see Fact.[JasonIgnore] and JSONManager.WriteToJsonFile)
        // using public dummy class to circumvent deserialiation JsonInheritanceProblem (see todos @PublicFactOrganizer)
        if (!exists || overwrite)
            JSONManager.WriteToJsonFile(path, new PublicFactOrganizer(this), 0);

        path = path_o;
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
        set.path = path;

        return true;
    }

    public static void delete(string name, List<Directories> hierarchie, bool use_install_folder)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierState.AsEnumerable());

        string path = CreatePathToFile(out bool _, name, "JSON", hierarchie, use_install_folder);
        hierarchie.RemoveRange(hierarchie.Count - hierState.Count, hierState.Count);

        delete(path);
    }

    public static void delete(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    public void delete()
    {
        delete(path);
    }
    /// @}

    /// <summary>
    /// Call this after assigning a stored instance in an empty world, that was not drawn.
    /// <see cref="redo">Redoes</see>/ draws everything from <see cref="marker"/> = 0 to <paramref name="draw_all"/><c> ? worksteps : backlog</c>
    /// </summary>
    /// <remarks>Does not invoke <see cref="softreset"/> or <see cref="undo"/> in any way and thus may trigger <see cref="Exception">Exceptions</see> or undefined behaviour if any <see cref="Fact"/> in <see cref="FactDict"/> is already drawn.</remarks>
    public void Draw(bool draw_all = false)
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

    /// <summary>
    /// Undraws everything by invoking <see cref="CommunicationEvents.RemoveFactEvent"/>, that is <see cref="meta.active"/>, but does not change that satus.
    /// </summary>
    /// <param name="force_invoke">if set <c>true</c>, invokes <see cref="CommunicationEvents.RemoveFactEvent"/> for every <see cref="Fact"/> regardles of <see cref="meta.active"/> status or <see cref="invoke"/></param>
    public void Undraw(bool force_invoke = false)
    {
        foreach (var entry in FactDict)
        {
            if (force_invoke || (invoke && MetaInf[entry.Key].active))
                CommunicationEvents.RemoveFactEvent.Invoke(entry.Value);
        }
    }

    /// <summary>
    /// Updates <see cref="MetaInf"/>, <see cref="Fact.Label"/> and invokes <see cref="CommunicationEvents"/> (latter iff <see cref="invoke"/> is set)
    /// </summary>
    /// <param name="creation">wether <see cref="Fact"/> is created or removed</param>
    /// <param name="Id"><see cref="Fact.Id"/></param>
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

    /// <summary>
    /// Used to check wether <see cref="FactOrganizer">this</see> satisfies the constrains of an <see cref="SolutionOrganizer">Solution</see>.
    /// Only <see cref="meta.active"/> are accounted for.
    /// </summary>
    /// <param name="MinimalSolution">describes constrains</param>
    /// <param name="MissingElements">elements which were *not* found in <see cref="SolutionOrganizer.ValidationSet"/> in a format reflecting that of <see cref="SolutionOrganizer.ValidationSet"/></param>
    /// <param name="Solutions">elements which *were* found in <see cref="SolutionOrganizer.ValidationSet"/> in a format reflecting that of <see cref="SolutionOrganizer.ValidationSet"/></param>
    /// <returns><c>true</c> iff *all* constrains set by <paramref name="MinimalSolution"/> are met</returns>
    public bool DynamiclySolved(
        SolutionOrganizer MinimalSolution,
        out List<List<string>> MissingElements,
        out List<List<string>> Solutions)
    {
        MissingElements = new List<List<string>>();
        // need to work not on ref/out
        List<List<string>> Solution_L = new List<List<string>>();

        int MissingElementsCount = 0;
        var activeList = FactDict.Values.Where(f => MetaInf[f.Id].active);

        foreach (var ValidationSet in MinimalSolution.ValidationSet)
        {
            // List to relato to. Either all active facts or those defined in RelationIndex if not empty
            var relateList = ValidationSet.RelationIndex.Count == 0 ? activeList :
                ValidationSet.RelationIndex.Select(i => Solution_L[i]) // Select by Index
                .SelectMany(i => i) // Flatten structure
                .Select(URI => this[URI]); // Get Facts

            // check by MasterIds
            // ALL Masters must relate
            var part_minimal = 
                ValidationSet.MasterIDs.Select(URI => MinimalSolution[URI]);

            var part_solution =
                relateList.Where(active => part_minimal.Contains(active, ValidationSet.Comparer.SetSearchRight()))
                .ToList(); // needed for some reason
            
            var part_missing =
                part_minimal.Except(part_solution, ValidationSet.Comparer.SetSearchLeft());

            // SolutionIndex may include current index
            Solution_L.Add(part_solution.Select(fact => fact.Id).ToList());
            MissingElements.Add(part_missing.Select(fact => fact.Id).ToList());
            MissingElementsCount += part_missing.Count();

            // check by previous solutions
            // at least ONE member must relate
            var part_consequential_minimal =
                ValidationSet.SolutionIndex.Select(i => Solution_L[i]) // Select by Index
                .SelectMany(i => i) // Flatten structure
                .Select(URI => this[URI]); // Get Facts

            var part_consequential_solution =
                relateList.Where(active => part_consequential_minimal.Contains(active, ValidationSet.Comparer.SetSearchRight()));

            Solution_L.Last().AddRange(part_consequential_solution.Select(fact => fact.Id).ToList());
            MissingElementsCount += Convert.ToInt32(
                part_consequential_solution.Count() == 0 && part_consequential_minimal.Count() != 0);
        }

        Solutions = Solution_L;
        return MissingElementsCount == 0;
    }

}


// TODO? PERF? SE? JsonInheritanceProblem: scrap this hardwired class and implement dynamic approach with JsonConverter (see: JSONManager.JsonInheritenceConverter)
/// <summary>
/// <see cref="JsonReader"/>/ <see cref="JsonWriter"/> readable and creatable format.
/// TODO? PERF? SE? JsonInheritanceProblem: scrap this hardwired class and implement dynamic approach with JsonConverter (see <see cref="JSONManager.JsonInheritenceConverter<T>"/>)
/// </summary>
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