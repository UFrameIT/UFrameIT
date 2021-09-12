using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using static CommunicationEvents;

public class SolutionOrganizer : FactOrganizer
{
    private const string
        endingSol = "_sol",
        endingVal = "_val";

    private string path = null;
    private static List<Directories>
        hierVal = new List<Directories> { Directories.ValidationSets };

    public List<SubSolution> ValidationSet;

    public class SubSolution
    // needs to be public for JSONWriter
    {
        // Actual solution:
        //  HashSet<string> MasterIDs: string{SolutionOrganizer.FacDict.Values}
        //      SolutionFacts to set into relation
        //
        //  List<int> SolutionIndex: int{[],[0, SolutionOrganizer.ValidationSet.IndexOf(this) - 2]}
        //      marks LevelFacts found as solution in a previous entry
        //      or none if empty
        //
        //  Comparer FactComparer:
        //      Comparer to relation with between SolutionFacts and LevelFacts

        public HashSet<string> MasterIDs = new HashSet<string>();
        public List<int> SolutionIndex = new List<int>();
        [JsonIgnore]
        public FactComparer Comparer = new FactEquivalentsComparer();

        public string ComparerString
        {
            get { return Comparer.ToString(); }
            set {
                // Select and create FactComparer by name
                var typ = fact_comparer.First(t => t.Name == value);
                Comparer = Activator.CreateInstance(typ) as FactComparer;
            }
        }
        private static IEnumerable<Type> fact_comparer = Assembly.GetExecutingAssembly().GetTypes().Where(typeof(FactComparer).IsAssignableFrom);

        public SubSolution() { }

        public SubSolution(HashSet<string> MasterIDs, List<int> SolutionIndex, FactComparer Comparer)
        {
            if (MasterIDs != null)
                this.MasterIDs = MasterIDs;

            if (SolutionIndex != null)
                this.SolutionIndex = SolutionIndex;

            if (Comparer != null)
                this.Comparer = Comparer;
        }
    }

    public SolutionOrganizer(bool invoke = false): base(invoke)
    {
        ValidationSet = new List<SubSolution>();
    }

    public List<Fact> getMasterFactsByIndex (int i)
    {
        return ValidationSet[i].MasterIDs.Select(id => this[id]).ToList();
    }

    public new void store(string name, List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierVal.AsEnumerable());

        base.store(name + endingSol, hierarchie, use_install_folder);

        string path_o = path;
        path = CreatePathToFile(out _, name + endingVal, "JSON", hierarchie, use_install_folder);
        JSONManager.WriteToJsonFile(path, this.ValidationSet.Select(e => (e.MasterIDs, e.Comparer.ToString())), 0);
        path = path_o;

        hierarchie.RemoveRange(hierarchie.Count - hierVal.Count, hierVal.Count);
    }

    public static bool load(ref SolutionOrganizer set, bool draw, string name, List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierVal.AsEnumerable());

        string path = CreatePathToFile(out bool loadable, name + endingVal, "JSON", hierarchie, use_install_folder);
        if (!loadable)
        {
            hierarchie.RemoveRange(hierarchie.Count - hierVal.Count, hierVal.Count);
            return false;
        }


        FactOrganizer save = StageStatic.stage.factState;
        StageStatic.stage.factState = new SolutionOrganizer(false) as FactOrganizer;

        loadable = FactOrganizer.load(ref StageStatic.stage.player_record.factState, draw, name + endingSol, hierarchie, use_install_folder, out Dictionary<string, string> old_to_new);
        if (loadable)
        {
            set = (SolutionOrganizer)StageStatic.stage.factState;
            set.path = path;
        }

        StageStatic.stage.factState = save;
        hierarchie.RemoveRange(hierarchie.Count - hierVal.Count, hierVal.Count);
        if (!loadable)
            return false;


        var JsonTmp = JSONManager.ReadFromJsonFile <List<SubSolution>> (path);
        foreach (var element in JsonTmp)
        // Parse and add
        {
            element.MasterIDs.Select(k => old_to_new[k]);
            set.ValidationSet.Add(element);
        }

        return true;
    }

    public new void delete()
    {
        FactOrganizer.delete(path);
    }
}
