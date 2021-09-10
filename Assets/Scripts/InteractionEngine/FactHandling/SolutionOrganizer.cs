using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static CommunicationEvents;

public class SolutionOrganizer : FactOrganizer
{
    private const string
        endingSol = "_sol",
        endingVal = "_val";

    private static List<Directories>
        hierVal = new List<Directories> { Directories.ValidationSets };

    public List<(HashSet<string> MasterIDs, FactComparer Comparer)> ValidationSet;

    public SolutionOrganizer(bool invoke = false): base(invoke)
    {
        ValidationSet = new List<(HashSet<string> MasterIDs, FactComparer Comparer)>();
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

        string path = CreatePathToFile(out _, name + endingVal, "JSON", hierarchie, use_install_folder);
        JSONManager.WriteToJsonFile(path, this.ValidationSet.Select(e => (e.MasterIDs, e.Comparer.ToString())), 0);

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

        loadable = FactOrganizer.load(ref StageStatic.stage.factState, draw, name + endingSol, hierarchie, use_install_folder, out Dictionary<string, string> old_to_new);
        if (loadable)
            set = (SolutionOrganizer) StageStatic.stage.factState;

        StageStatic.stage.factState = save;
        hierarchie.RemoveRange(hierarchie.Count - hierVal.Count, hierVal.Count);
        if (!loadable)
            return false;


        var JsonTmp = JSONManager.ReadFromJsonFile < List<(HashSet<string> MasterIDs, string ComparerString)> > (path);
        foreach (var element in JsonTmp)
        {
            // Get all FactComparer
            var FactCompTypes = Assembly.GetExecutingAssembly().GetTypes().Where(typeof(FactComparer).IsAssignableFrom);
            // Select and create FactComparer by name
            var typ = FactCompTypes.First(t => t.Name == element.ComparerString);
            FactComparer Comparer = Activator.CreateInstance(typ) as FactComparer;
            // Parse and add
            set.ValidationSet.Add((new HashSet<string>(element.MasterIDs.Select(k => old_to_new[k])), Comparer));
        }

        return true;
    }
}
