using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using static CommunicationEvents;

public class Stage
{
    /// <summary> Which category this <see cref="Stage"/> should be displayed in. </summary>
    public string category = null;
    /// <summary> Where to display this <see cref="Stage"/> within a <see cref="category"/> relative to others. </summary>
    public int number = -1;
    /// <summary>
    /// The name this <see cref="Stage"/> will be displayed with.
    /// Also defines names of save files of <see cref="player_record_list">stage progress</see>, <see cref="solution"/>
    /// </summary>
    public string name = null;
    /// <summary> The description this <see cref="Stage"/> will be displayed with.</summary>
    public string description = null;

    /// <summary> The name of a <see cref="UnityEngine.SceneManagement.Scene"/> that this <see cref="Stage"/> takes place in.</summary>
    public string scene = null;

    /// <summary> Wether this <see cref="Stage"/> is located in installation folder or user data (a.k.a. !local).</summary>
    public bool use_install_folder = false;
    /// <summary> TODO? interface </summary>
    public List<Directories> hierarchie = null;

    /// <summary>
    /// <see langword="true"/> iff there is at least one element in <see cref="player_record_list"/> where <see cref="PlayerRecord.solved"/> == <see langword="true"/>.
    /// </summary>
    [JsonIgnore]
    //TODO? update color if changed
    public bool completed_once { get { return player_record_list != null && player_record_list.Values.Where(s => s.solved == true).Any(); } }
    /// <summary>
    /// A list containing all saved player progress. <br/>
    /// - Key: name of file
    /// - Value: <see cref="PlayerRecord"/>
    /// </summary>
    public Dictionary<string, PlayerRecord> player_record_list = null;

    /// <summary>
    /// Defining when this <see cref="Stage.player_record"/> is considered as solved.
    /// <seealso cref="FactOrganizer.DynamiclySolved(SolutionOrganizer, out List{List{string}}, out List{List{string}})"/>
    /// </summary>
    [JsonIgnore]
    public SolutionOrganizer solution = null;

    /// <summary>
    /// A wrapper returning (or setting) <see cref="player_record.factState"/>. <br/>
    /// When <see cref="player_record"/> == <see langword="null"/>:
    /// - <c>get</c> returns <see langword="null"/>
    /// - <c>set</c> initiates <see cref="player_record"/>
    /// </summary>
    [JsonIgnore]
    public FactOrganizer factState { 
        get {
            if (player_record == null)
                return null;
            return player_record.factState;
        }
        set { 
            if (player_record == null) 
                 player_record = new PlayerRecord(record_name);
            player_record.factState = value; 
        } 
    }
    /// <summary> Current Stage progress.</summary>
    public PlayerRecord player_record = null;
    /// <summary> Returns a name for <see cref="player_record.name"/> which needs to be uniquified once put into <see cref="player_record_list"/> (e.g. by <see cref="push_record(double, bool) adding '_i'"/>).</summary>
    private string record_name { get { return name + "_save"; } }

    /// <summary> Wether <see cref="player_record.factState"/> (<see langword="false"/>) or <see cref="solution"/> (<see langword="true"/>) is exposed and drawn.</summary>
    [JsonIgnore]
    public bool creatorMode = false;

    /// @{ <summary>
    /// TODO? interafce
    /// </summary>
    private string path = null;
    private static List<Directories>
        hierStage = new List<Directories> { Directories.Stages };
    /// @}

    /// <summary> Tempory variable storing <see cref="factState"/> when <see cref="creatorMode"/> == <see langword="true"/>. </summary>
    private FactOrganizer hiddenState;

    /// <summary>
    /// Initiates all parameterless members. <br/>
    /// Used by <see cref="JsonConverter"/> to initate empty <c>class</c>.
    /// <seealso cref="InitOOP"/>
    /// </summary>
    public Stage()
    {
        InitOOP();
    }

    /// <summary>
    /// Standard Constructor. <br/>
    /// Initiates all members.
    /// <seealso cref="InitOOP"/>
    /// <seealso cref="InitFields(string, int, string, string, string, bool)"/>
    /// </summary>
    /// <param name="category">sets <see cref="category"/></param>
    /// <param name="number">sets <see cref="number"/></param>
    /// <param name="name">sets <see cref="name"/></param>
    /// <param name="description">sets <see cref="description"/></param>
    /// <param name="scene">sets <see cref="scene"/></param>
    /// <param name="local">sets !<see cref="use_install_folder"/></param>
    public Stage(string category, int number, string name, string description, string scene, bool local = true)
    {
        InitFields(category, number, name, description, scene, local);
        InitOOP();
    }

    /// <summary>
    /// Copy Constructor. <br/>
    /// <seealso cref="InitOOP"/>
    /// <seealso cref="InitFields(string, int, string, string, string, bool)"/>
    /// </summary>
    /// <param name="get">to be copied</param>
    /// <param name="category">sets <see cref="category"/></param>
    /// <param name="number">sets <see cref="number"/></param>
    /// <param name="name">sets <see cref="name"/></param>
    /// <param name="description">sets <see cref="description"/></param>
    /// <param name="scene">sets <see cref="scene"/></param>
    /// <param name="local">sets !<see cref="use_install_folder"/></param>
    public Stage(Stage get, string category, int number, string name, string description, string scene, bool local = true)
    {
        InitOOP();
        Stage cpy = new Stage();
        // "DeepCopy" of ref-types, 'cause screw c# and ICloneable
        load(ref cpy, get.name, null, get.use_install_folder);
        this.hierarchie = cpy.hierarchie;
        this.solution = cpy.solution;
        this.player_record = cpy.player_record;

        InitFields(category, number, name, description, scene, local);

        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        player_record.load(hierarchie);
        player_record.name = player_record.name.Replace(get.record_name, record_name);
        player_record.store(hierarchie, false);

        //this.player_record_list = cpy.player_record_list;
        foreach (var record in cpy.player_record_list.Values)
        {
            record.load(hierarchie);
            record.name = record.name.Replace(get.record_name, record_name);
            record.store(hierarchie, false);
            player_record_list.Add(record.name, record);
        }

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
        store(false);
    }

    /// <summary>
    /// Sets members which are primitives.
    /// </summary>
    /// <param name="category">sets <see cref="category"/></param>
    /// <param name="number">sets <see cref="number"/></param>
    /// <param name="name">sets <see cref="name"/></param>
    /// <param name="description">sets <see cref="description"/></param>
    /// <param name="scene">sets <see cref="scene"/></param>
    /// <param name="local">sets !<see cref="use_install_folder"/></param>
    public void InitFields(string category, int number, string name, string description, string scene, bool local)
    {
        this.category = category;
        this.number = number;
        this.name = name;
        this.description = description;
        this.scene = scene;
        this.use_install_folder = !local;
    }

    /// <summary>
    /// Initiates members which are non primitives.
    /// </summary>
    private void InitOOP()
    {
        solution = new SolutionOrganizer();
        player_record = new PlayerRecord(record_name);
        player_record_list = new Dictionary<string, PlayerRecord>();
    }

    /// <summary>
    /// Resets to factory condition.
    /// <see cref="ClearSolution"/>
    /// <see cref="ClearPlay"/>
    /// <see cref="ClearALLRecords"/>
    /// </summary>
    public void ClearAll()
    {
        ClearSolution();
        ClearPlay();
        ClearALLRecords();
    }

    /// <summary>
    /// Resets <see cref="solution"/> and calling <see cref="solution.hardreset(bool)"/>.
    /// <seealso cref="FactOrganizer.hardreset(bool)"/>
    /// </summary>
    public void ClearSolution()
    {
        solution.hardreset(false);
        solution = new SolutionOrganizer();
    }

    /// <summary>
    /// Resets current <see cref="player_record"/> and calling <see cref="player_record.factState.hardreset(bool)"/>.
    /// <seealso cref="FactOrganizer.hardreset(bool)"/>
    /// </summary>
    public void ClearPlay()
    {
        player_record.factState.hardreset(false);
        player_record = new PlayerRecord(record_name);
    }

    /// <summary>
    /// Resets and <see cref="deletet_record(PlayerRecord, bool)">deletes</see> all members of <see cref="player_record_list"/>.
    /// <seealso cref="PlayerRecord.delete(List<Directories>)"/>
    /// </summary>
    public void ClearALLRecords()
    {
        foreach (var record in player_record_list.Values.ToList())
            deletet_record(record, false);

        player_record_list = new Dictionary<string, PlayerRecord>();
    }

    /// <summary>
    /// <see cref="PlayerRecord.delete(List<Directories>)">Deletes</see> <paramref name="record"/> and calls <see cref="PlayerRecord.factState.hardreset()"/>.
    /// <seealso cref="PlayerRecord.delete(List<Directories>)"/>
    /// <seealso cref="FactOrganizer.hardreset(bool)"/>
    /// </summary>
    /// <param name="record">to be deleted</param>
    /// <param name="b_store">iff <see langword="true"/> <see cref="store(bool)">stores</see> changes made to this <see cref="Stage"/></param>
    public void deletet_record(PlayerRecord record, bool b_store = true)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        if (record.factState != null)
            record.factState.hardreset();
        record.delete(hierarchie);
        player_record_list.Remove(record.name);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
        if(b_store)
            store();
    }

    /// <summary>
    /// Clones <paramref name="record"/> to <see cref="player_record"/> iff found in <see cref="player_record_list"/> <br/>
    /// or initiates new <see cref="player_record"/> iff <paramref name="record"/>==<see langword="null"/>.
    /// </summary>
    /// <param name="record">to be set or <see langword="null"/></param>
    /// <returns><see langword="false"/> iff <paramref name="record"/> not found in <see cref="player_record_list"/> <br/>
    /// or <see cref="PlayerRecord.load(List<Directories>)"/> fails.</returns>
    public bool set_record(PlayerRecord record)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        if (record != null)
        {
            if (!player_record_list.ContainsKey(record.name))
            {
                hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
                return false;
            }

            else if (!record.load(hierarchie))
            {
                deletet_record(record);
                hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
                return false;
            }
        }

        player_record = record == null ? new PlayerRecord(record_name) : record.Clone(hierarchie);
        player_record.name = record_name;

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);

        store(false);

        return true;
    }

    /// <summary>
    /// Adds current <see cref="player_record"/> to <see cref="player_record_list"/> incrementing <see cref="PlayerRecord.seconds"/> beforehand.
    /// </summary>
    /// <param name="seconds_s">time in seconds to be added to <see cref="player_record.seconds"/> before pushing. <br/>
    /// Iff set to <c>-1</c> <see cref="Time.timeSinceLevelLoadAsDouble"/> will be used.</param>
    /// <param name="force_push">iff set <see langword="true"/> && <see cref="StageStatic.mode"/> == <see cref="StageStatic.Mode.Create"/> && <see cref="creatorMode"/> <br/>
    /// current displayed <see cref="solution"/> in <see cref="factState"/> will be pushed into <see cref="player_record_list"/></param>
    public void push_record(double seconds_s = -1, bool force_push = false)
    {
        if(!force_push && StageStatic.mode == StageStatic.Mode.Create && creatorMode)
        // store solution space
        {
            SetMode(false);
            store(false);
            //push_record(seconds_s, false);
            SetMode(true);
            return;
        }

        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        if (seconds_s == -1)
            seconds_s = Time.timeSinceLevelLoadAsDouble;
        player_record.seconds += seconds_s;

        var push = player_record.Clone(hierarchie);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);

        int i = 0;
        push.name = record_name + "_" + i.ToString();
        for (; player_record_list.ContainsKey(push.name); i++)
            push.name = record_name + "_" + i.ToString();

        player_record_list.Add(push.name, push);

        store(false);
    }

    /// <summary>
    /// Switches between <see cref="player_record.factState"/> (<see langword="false"/>) and <see cref="solution"/> (<see langword="true"/>) to display in GameWorld.
    /// </summary>
    /// <param name="create">sets <see cref="creatorMode"/></param>
    public void SetMode(bool create)
    {
        if (create == creatorMode)
            return;
        
        creatorMode = create;

        if (create)
        {
            hiddenState = factState;
            factState.Undraw();

            factState = solution as FactOrganizer;
            factState.invoke = true;
            factState.Draw();
        }
        else
        {
            solution = factState as SolutionOrganizer;
            factState.Undraw();
            //solution.invoke = false;

            factState = hiddenState;
            factState.Draw();
        }

    }

    /// <summary>
    /// Clears and deletes all files associated with this <see cref="Stage"/>.
    /// </summary>
    /// <param name="player_record_list_too">iff set <see langword="false"/>, all files regarding <see cref="player_record_list"/> will be spared.</param>
    public void delete(bool player_record_list_too)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        ClearSolution();
        ClearPlay();
        solution.delete();
        player_record.delete(hierarchie);

        foreach (var record in player_record_list.Values.ToList())
            record.factState.hardreset(false);
        if (player_record_list_too)
            ClearALLRecords();

        if (File.Exists(path))
            File.Delete(path);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
    }

    /// <summary>
    /// Stores and overwrites this <see cref="Stage"/>, <see cref="player_record"/>, every element in <see cref="player_record_list"/> and <see cref="solution"/> (no overwrite for latter if empty).
    /// </summary>
    /// <param name="reset_player">wether to clear current <see cref="player_record"/></param>
    public void store(bool reset_player = false)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        player_record.name = record_name;
        if (reset_player)
            player_record = new PlayerRecord(record_name);

        //if (creatorMode || StageStatic.devel)
        {
            string path_o = path;
            path = CreatePathToFile(out bool exists, name, "JSON", hierarchie, use_install_folder);
            hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);

            JSONManager.WriteToJsonFile(path, this, 0);
            path = path_o;

            hierarchie.AddRange(hierStage.AsEnumerable());
            if(solution != null)
                solution.store(name, hierarchie, use_install_folder,
                    overwrite: solution.ValidationSet.Count > 0 && !solution.ValidationSet.Aggregate(true, (last, next) => last && next.IsEmpty()));
        }

        if (player_record != null)
            player_record.store(hierarchie, true);

        foreach (var track in player_record_list)
            track.Value.store(hierarchie, false);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
    }

    /// <summary>
    /// Loads a Stage complete using:
    /// - <see cref="ShallowLoad(ref Stage, string, List<Directories>, bool)"/>
    /// - <see cref="DeepLoad"/>
    /// </summary>
    /// <param name="set">see <see cref="ShallowLoad(ref Stage, string, List<Directories>, bool)"/></param>
    /// <param name="name">see <see cref="ShallowLoad(ref Stage, string, List<Directories>, bool)"/></param>
    /// <param name="hierarchie">see <see cref="ShallowLoad(ref Stage, string, List<Directories>, bool)"/></param>
    /// <param name="use_install_folder">see <see cref="ShallowLoad(ref Stage, string, List<Directories>, bool)"/></param>
    /// <returns><see langword="true"/> iff succeeded</returns>
    public static bool load(ref Stage set, string name, List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        Stage ret = new Stage();

        bool loadable = ShallowLoad(ref ret, name, hierarchie, use_install_folder);
        if (!loadable)
            return false;

        loadable = ret.DeepLoad();
        if (!loadable)
            return false;

        set = ret;
        return true;
    }

    /// <summary>
    /// Reads File given by <paramref name="path"/> and writes its contents into <paramref name="set"/>.
    /// <remarks>Will not read members decorated with <see cref="JsonIgnoreAttribute"/>: <see cref="solution"/>, <see cref="player_record"/>.</remarks>
    /// </summary>
    /// <param name="set">to be written in</param>
    /// <param name="path">file location</param>
    /// <returns><see langword="true"/> iff succeeded</returns>
    public static bool ShallowLoad(ref Stage set, string path)
    {
        if (!System.IO.File.Exists(path))
            return false;

        set = JSONManager.ReadFromJsonFile<Stage>(path);
        set.path = path;

        set.hierarchie ??= new List<Directories>();
        set.hierarchie.AddRange(hierStage.AsEnumerable());
        //if (!set.player_record.load(set.hierarchie))
        //    set.player_record = new PlayerRecord(set.record_name);
        set.hierarchie.RemoveRange(set.hierarchie.Count - hierStage.Count, hierStage.Count);

        return true;
    }

    /// <summary>
    /// Determines path via <paramref name="hierarchie"/> and <paramref name="use_install_folder"/> and calls <see cref="ShallowLoad(ref Stage, string)"/>.
    /// <seealso cref="ShallowLoad(ref Stage, string)"/>
    /// </summary>
    /// <param name="set">see <see cref="ShallowLoad(ref Stage, string)"/></param>
    /// <param name="name">see <see cref="ShallowLoad(ref Stage, string)"/></param>
    /// <param name="hierarchie">see <see cref="hierarchie"/> // TODO? Interface</param>
    /// <param name="use_install_folder">see <see cref="use_install_folder"/></param>
    /// <returns><see langword="true"/> iff succeeded</returns>
    public static bool ShallowLoad(ref Stage set, string name, List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        string path = CreatePathToFile(out bool loadable, name, "JSON", hierarchie, use_install_folder);
        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);

        if (!loadable || !ShallowLoad(ref set, path))
            return false;

        return true;
    }

    /// <summary>
    /// Loads every member decorated with <see cref="JsonIgnoreAttribute"/>: <see cref="solution"/>, <see cref="player_record"/>.
    /// </summary>
    /// <returns><see langword="false"/> iff <see cref="solution"/> could not be <see cref="SolutionOrganizer.load(ref SolutionOrganizer, bool, string, List<Directories>, bool)">loaded</see>.</returns>
    public bool DeepLoad()
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        bool loadable;

        solution ??= new SolutionOrganizer(false);
        loadable = SolutionOrganizer.load(ref solution, false, name, hierarchie, use_install_folder);
        if (!loadable)
            return false;

        player_record.load(hierarchie);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
        return true;
    }

    /// <summary>
    /// Looks for saved <see cref="Stage">Stages</see> in parametised directories.
    /// </summary>
    /// <param name="hierarchie">see <see cref="hierarchie"/> //TODO? Interface</param>
    /// <param name="use_install_folder">see <see cref="use_install_folder"/></param>
    /// <returns>contians all <see cref="Stage">Stages</see> found given parameters.</returns>
    public static Dictionary<string, Stage> Grup(List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        Dictionary<string, Stage> ret = new Dictionary<string, Stage>();

        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        string path = CreatePathToFile(out _, "", "", hierarchie, use_install_folder);
        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);

        string ending = ".JSON";

        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles();
        foreach(var file in fileInfo)
        {
            if (0 != string.Compare(ending, 0, file.Name, file.Name.Length - ending.Length, ending.Length))
                continue;

            Stage tmp = new Stage();
            if (ShallowLoad(ref tmp, file.FullName))
                ret.Add(tmp.name, tmp);

        }

        return ret;
    }

    /// <summary>
    /// Calls <see cref="ClearPlay"/> and <see cref="store(bool)">store(true)</see>.
    /// </summary>
    public void ResetPlay()
    {
        ClearPlay();
        store(true);
    }

    /// <summary>
    /// Calls <see cref="ClearPlay"/>, <see cref="ClearALLRecords"/> and <see cref="store(bool)">store(true)</see>.
    /// </summary>
    public void ResetSaves()
    {
        ClearPlay();
        ClearALLRecords();
        store(true);
    }

    /// <summary>
    /// Checks if current <see cref="player_record"/> is solved. <br/>
    /// Iff return value <see langword="true"/>:
    /// - Highlites all <see cref="Fact">Facts</see> in <see cref="factState"/> beeing found in <see cref="solution"/>
    /// - Iff <see cref="player_record.seconds"/> > 0: 
    /// <see cref="push_record(double, bool)">Pushes</see> current <see cref="player_record"/> to <see cref="player_record_list"/> and sets <see cref="PlayerRecord.solved"/> to <see langword="true"/>.
    /// </summary>
    /// <returns><see langword="true"/> iff current <see cref="player_record"/> is solved.</returns>
    /// <seealso cref="FactOrganizer.DynamiclySolved(SolutionOrganizer, out List<List<string>>, out List<List<string>>)"/>
    public bool CheckSolved()
    {
        double time_s = Time.timeSinceLevelLoadAsDouble;

        bool solved =
            factState.DynamiclySolved(solution, out _, out List<List<string>> hits);

        if (solved)
            foreach (var hitlist in hits)
                foreach (var hit in hitlist)
                    AnimateExistingFactEvent.Invoke(factState[hit]);

        if (solved && player_record.seconds > 0)
        {
            player_record.solved = true;
            push_record(time_s);
            store(true); // reset player_record
            player_record.solved = false;
        }

        return solved;
    }

}


/// <summary>
/// Represents a save slot.
/// </summary>
public class PlayerRecord
{
    /// <summary> Wether this save has solved the <see cref="Stage"/> which contains it. </summary>
    public bool solved = false;
    /// <summary> When this save was created (not modified!). </summary>
    public long date = System.DateTime.Now.ToBinary();
    /// <summary> The time spent within this save since creation. </summary>
    public double seconds = 0;

    /// <summary> Stage progress. </summary>
    [JsonIgnore]
    public FactOrganizer factState = null;
    /// <summary> save game file name </summary>
    public string name = null;

    private static List<Directories>
        hierStage = new List<Directories> { /*Directories.FactStateMachines*/ };

    /// <summary>
    /// Empty constructor for <see cref="JsonConverter"/>
    /// </summary>
    public PlayerRecord() { }

    /// <summary>
    /// Standard Constructor.
    /// </summary>
    /// <param name="name">sets <see cref="name"/></param>
    public PlayerRecord(string name) {
        this.name = name;
        factState = new FactOrganizer();
    }

    public void store(List<Directories> hierarchie, bool force_write)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        if (factState != null)
            factState.store(name, hierarchie, false, force_write);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
    }


    public bool load(List<Directories> hierarchie)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        factState = new FactOrganizer(false);
        bool loadable = FactOrganizer.load(ref factState, false, name, hierarchie, false, out _);
        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
        if (!loadable) {
            return false;
        }

        return true;
    }

    public void delete(List<Directories> hierarchie)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        FactOrganizer.delete(name, hierarchie, false);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
    }

    /// <summary>
    /// Copies a specified <see cref="PlayerRecord"/>
    /// </summary>
    /// <param name="hierarchie">// TODO:</param>
    /// <returns>a copied <see cref="PlayerRecord"/></returns>
    public PlayerRecord Clone(List<Directories> hierarchie)
    {
        this.store(hierarchie, true);

        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        var ret = new PlayerRecord(this.name);
        ret.solved = this.solved;
        ret.seconds = this.seconds;
        ret.load(hierarchie);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
        return ret;
    }
}