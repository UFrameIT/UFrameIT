using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using static CommunicationEvents;

/// <summary>
/// Solution of a <see cref="Stage"/>
/// </summary>
public class SolutionOrganizer : FactOrganizer
{
    /// @{ <summary> adds to the end of the file name of a </summary>
    private const string
        /// <summary> SolutionFile (stores <see cref="SolutionOrganizer">this</see>) </summary>
        endingSol = "_sol",
        /// <summary> ValidationFile (stores <see cref="ValidationSet"/>) </summary>
        endingVal = "_val";
    /// @}

    /// <summary>
    /// \copydoc FactOrganizer.path
    /// Additional value for <see cref="ValidationSet"/>
    /// </summary>
    private string path_Val = null;
    /// <summary>
    /// \copydoc FactOrganizer.hierState
    /// Additional value for <see cref="ValidationSet"/>
    /// </summary>
    private static List<Directories>
        hierVal = new List<Directories> { Directories.ValidationSets };

    /// <summary>
    /// A collection of constrains of which *all* have to be <see langword="true"/>
    /// <seealso cref="SubSolution"/>
    /// <seealso cref="FactOrganizer.DynamiclySolved(SolutionOrganizer, out List<List<string>>, out List<List<string>>)"/>
    /// </summary>
    public List<SubSolution> ValidationSet;

    /// <summary>
    /// Sits at the heart, but represents only a part of the whole Solution.
    /// </summary>
    public class SubSolution
    // needs to be public for JSONWriter
    {
        /// <summary>
        /// entails <b>{<see cref="FactOrganizer.FactDict">SolutionOrganizer.FacDict.Values</see>}</b> <br/>
        /// <see cref="FactOrganizer.FactDict">SolutionFacts</see> to relate from.
        /// </summary>
        public HashSet<string> MasterIDs = new HashSet<string>();

        /// <summary>
        /// entails <b>{[],[0, <see cref="SolutionOrganizer.ValidationSet"/><c>.IndexOf(<see cref="SubSolution">this</see>)</c> - 2]}</b> <br/>
        /// Marks LevelFacts (<see cref="StageStatic.stage.factState"/>) found as solution (<see cref="FactOrganizer.DynamiclySolved(SolutionOrganizer, out List<List<string>>, out List<List<string>>)"/>)
        ///  in a previous entry of <see cref="SolutionOrganizer.ValidationSet"/><br/>
        /// to relate from *in addition* to <see cref="MasterIDs"/> <br/>
        /// or _none_ if <c>empty</c>
        /// </summary>
        public List<int> SolutionIndex = new List<int>();

        /// <summary>
        /// entails <b>{[],[0, <see cref="SolutionOrganizer.ValidationSet"/><c>.IndexOf(<see cref="SubSolution">this</see>)</c> - 2]}</b> <br/>
        /// Marks LevelFacts (<see cref="StageStatic.stage.factState"/>) found as solution (<see cref="FactOrganizer.DynamiclySolved(SolutionOrganizer, out List<List<string>>, out List<List<string>>)"/>)
        ///  in a previous entry of <see cref="SolutionOrganizer.ValidationSet"/><br/>
        /// to relate to *instead of* all LevelFacts (<see cref="StageStatic.stage.factState"/>) <br/>
        /// or _none_ if <c>empty</c>
        /// </summary>
        public List<int> RelationIndex = new List<int>();

        /// <summary>
        /// Comparer defining relation between <see cref="FactOrganizer.FactDict">SolutionFacts</see> and LevelFacts (<see cref="StageStatic.stage.factState"/>)
        /// </summary>
        [JsonIgnore]
        public FactComparer Comparer = new FactEquivalentsComparer();

        /// <summary>
        /// Enables (especially <see cref="JsonConverter"/>) to read and set <see cref="Comparer"/> by its <c>string</c> representation.
        /// </summary>
        public string ComparerString
        {
            get { return Comparer.ToString(); }
            set {
                // Select and create FactComparer by name
                var typ = fact_comparer.First(t => t.Name == value);
                Comparer = Activator.CreateInstance(typ) as FactComparer;
            }
        }
        /// <summary>
        /// Collection of <c>string</c> representations of *all* available <see cref="FactComparer"/> to choose from.
        /// </summary>
        [JsonIgnore]
        public static readonly IEnumerable<Type> fact_comparer = Assembly.GetExecutingAssembly().GetTypes().Where(typeof(FactComparer).IsAssignableFrom);

        /// <summary>
        /// Only used by <see cref="JsonConverter"/> to initiate empty instance.
        /// </summary>
        public SubSolution() { }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="MasterIDs">sets <see cref="MasterIDs"/> iff not <see langword="null"/></param>
        /// <param name="SolutionIndex">sets <see cref="SolutionIndex"/> iff not <see langword="null"/></param>
        /// <param name="RelationIndex">sets <see cref="RelationIndex"/> iff not <see langword="null"/></param>
        /// <param name="Comparer">sets <see cref="Comparer"/> iff not <see langword="null"/></param>
        public SubSolution(HashSet<string> MasterIDs, List<int> SolutionIndex, List<int> RelationIndex, FactComparer Comparer)
        {
            if (MasterIDs != null)
                this.MasterIDs = MasterIDs;

            if (SolutionIndex != null)
                this.SolutionIndex = SolutionIndex;

            if (RelationIndex != null)
                this.RelationIndex = RelationIndex;

            if (Comparer != null)
                this.Comparer = Comparer;
        }

        /// <summary>
        /// <see langword="true"/> if there is no solution to be deducted.
        /// </summary>
        /// <returns><c>MasterIDs.Count == 0 && SolutionIndex.Count == 0;</c></returns>
        public bool IsEmpty()
        {
            return MasterIDs.Count == 0 && SolutionIndex.Count == 0;
        }
    }

    /// \copydoc FactOrganizer.FactOrganizer(bool)
    public SolutionOrganizer(bool invoke = false): base(invoke)
    {
        ValidationSet = new List<SubSolution>();
    }

    /*
    public List<Fact> getMasterFactsByIndex (int i)
    {
        return ValidationSet[i].MasterIDs.Select(id => this[id]).ToList();
    }
    */

    /// @{ 
    /// TODO? move to interface?
    /// TODO: document
    public new void store(string name, List<Directories> hierarchie = null, bool use_install_folder = false, bool overwrite = true)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierVal.AsEnumerable());

        base.store(name + endingSol, hierarchie, use_install_folder, overwrite);

        string path_o = path_Val;
        path_Val = CreatePathToFile(out bool exists, name + endingVal, "JSON", hierarchie, use_install_folder);
        hierarchie.RemoveRange(hierarchie.Count - hierVal.Count, hierVal.Count);

        if (exists && !overwrite)
        {
            path_Val = path_o;
            return;
        }

        JSONManager.WriteToJsonFile(path_Val, this.ValidationSet, 0);
        path_Val = path_o;
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

        loadable = FactOrganizer.load(ref StageStatic.stage.player_record.factState
            , draw, name + endingSol, hierarchie, use_install_folder, out Dictionary<string, string> old_to_new);

        if (loadable)
        {
            set = (SolutionOrganizer)StageStatic.stage.factState;
            set.path_Val = path;
        }

        StageStatic.stage.factState = save;
        hierarchie.RemoveRange(hierarchie.Count - hierVal.Count, hierVal.Count);
        if (!loadable)
            return false;


        var JsonTmp = JSONManager.ReadFromJsonFile <List<SubSolution>> (path);
        foreach (var element in JsonTmp)
        // Parse and add
        {
            element.MasterIDs = new HashSet<string>(element.MasterIDs.Select(k => old_to_new[k]));
            set.ValidationSet.Add(element);
        }

        return true;
    }

    public new void delete()
    {
        base.delete();
        if (System.IO.File.Exists(path_Val))
            System.IO.File.Delete(path_Val);
    }
    /// @}
}
