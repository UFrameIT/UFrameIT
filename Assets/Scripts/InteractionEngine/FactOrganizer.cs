using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//TODO? PERF? (often inserts) SortedDict <-> Dict (often reads)
//TODO? hide base dict
//TODO: MMT: move some functionality there
//TODO: consequent!= samestep != dependent
//TODO! use URI as key

//PERF: avoid string as key (hash -> colission? -> strcmp[!])

public class FactOrganizer: Dictionary<string, Fact>
{
    private Dictionary<string, meta> MetaInf = new Dictionary<string, meta>();
    private List<stepnote> Workflow = new List<stepnote>();
    // notes position in Workflow for un-/redo; the pointed to element is non-acitve
    private int marker = 0;
    // backlock logic for convinience
    private int worksteps = 0;
    private int backlog = 0;
    // set if recently been resetted
    private bool resetted = false;

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

    public FactOrganizer() : base() { }

    public FactOrganizer(IDictionary<string, Fact> dictionary) : base(dictionary) { }


    //TODO: PERF: better way than string search? -> use string as key!
    public bool searchURI(string uri, out string found)
    {
        foreach(var element in this)
            if (element.Value.URI.Equals(uri))
            {
                found = element.Key;
                return true;
            }

        found = null;
        return false;
    }

    //TODO? MMT? PERF: O(n), every Fact-insertion
    private bool FindEquivalent(Fact search, out Fact found)
    // Looks for existent facts (found) which are very similar to prposed fact (search)
    // does not check active state
    {
        foreach (var entry in this)
        {
            if (entry.Value.GetType() == search.GetType() &&
                entry.Value.Equivalent(search))
            {
                found = entry.Value;
                return true;
            }
        }

        found = null;
        return false;
    }

    private void WorkflowAdd(stepnote note)
    // prunes & adds Workflow; updates meta struct; Invokes Events
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

        // update active info
        meta info = MetaInf[note.Id];
        info.active = note.creation;
        MetaInf[note.Id] = info;

        InvokeFactEvent(note.creation, note.Id);
    }

    private void PruneWorkflow()
    // set current (displayed) state in stone; resets un-redo parameters
    {
        if (backlog > 0)
        {
            worksteps -= backlog;
            backlog = 0;

            for (int i = Workflow.Count - 1; i >= marker; i--)
            // cleanup now obsolete Facts
            {
                stepnote last = Workflow[i];

                if (MetaInf[last.Id].workflow_id == i)
                // remove for good, if original creation gets pruned
                {
                    this[last.Id].delete();
                    base.Remove(last.Id);
                    MetaInf.Remove(last.Id);
                }
                else
                // update active status
                {
                    meta inf = MetaInf[last.Id];
                    inf.active = !last.creation;
                    MetaInf[last.Id] = inf;
                }
            }

            // prune Worklfow down to marker
            Workflow.RemoveRange(marker, Workflow.Count - marker);
        }
    }

    public new void Add(string key, Fact value)
    // hide
    {
        this.Add(value, out bool obsolete);
    }

    public string Add(Fact value, out bool exists, bool samestep = false)
    // also checks for duplicates and active state
    // returns key of actual Fact
    {
        if (resetted)
            this.hardreset(false);

        string key;
        if (exists = FindEquivalent(value, out Fact found))
        {
            //TODO: MMT: del 'fact' (value) in MMT (alt.: s.TODO in addFact)

            key = found.URI;
            if (MetaInf[key].active)
                return key;
        }
        else
        {
            //TODO: MMT: alt: insert in MMT if needed here/ on Invoke() (see WorkflowAdd)

            key = value.URI;
            base.Add(key, value);
            MetaInf.Add(key, new meta(marker, true));
        }

        WorkflowAdd(new stepnote(key, samestep, true, this));
        return key;
    }

    public new bool Remove(string key)
    // hide
    {
        return this.Remove(key, false);
    }

    public bool Remove(Fact value, bool samestep = false)
    {
        if (!this.ContainsKey(value.URI))
            return false;

        this.Remove(value.URI, samestep);
        return true;
    }

    public bool Remove(string key, bool samestep = false)
    //no reset check needed (impossible state)
    {
        if (!base.ContainsKey(key))
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

        /* consequent != samestep != dependent
        // get steproot
        if (Workflow[pos].samestep)
            pos = Workflow[pos].steplink;

        // add entire step
        for (int i = pos; i < Workflow[pos].steplink; i++)
            dependencies.Add(Workflow[i].Id);
        pos = Workflow[pos].steplink;
        */

        // accumulate facts that are dependent of dependencies
        for (int i = pos; i < marker; i++)
        {
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

        // check for valid step (implicit reset check)
        if (pos >= marker)
            return;
        
        for (int i = pos, stop = Workflow[pos].samestep ? Workflow[pos].steplink : pos;
            i >= stop; i--, samestep = true)
        {
            WorkflowAdd(new stepnote(Workflow[i].Id, samestep, !Workflow[i].creation, this));
        }
    }

    public void undo()
    {
        if (resetted)
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
        resetted = false;

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

    public new void Clear()
    // Does not Invoke RemoveFactEvent(s)!
    {
        base.Clear();
        Workflow.Clear();
        marker = 0;
        worksteps = 0;
        backlog = 0;
        resetted = false;
    }

    public void hardreset(bool invoke_event = true)
    {
        foreach(var entry in this)
        {
            if (invoke_event) //TODO? check if removed
                CommunicationEvents.RemoveFactEvent.Invoke(entry.Value);
            entry.Value.delete();
        }
        this.Clear();
    }

    public void softreset()
    {
        if (resetted)
        {
            fastforward();
            return;
        }

        // TODO: PREF: alt: EventResetAll
        // marker = 0; backlog = worksteps;
        while (marker > 0)
            undo();

        resetted = true;
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
    }

    public void load()
    {
        // TODO: see issue #58
    }

    private void InvokeFactEvent(bool creation, string Id)
    {
        if (creation)
            CommunicationEvents.AddFactEvent.Invoke(this[Id]);
        else
            CommunicationEvents.RemoveFactEvent.Invoke(this[Id]);
    }

}