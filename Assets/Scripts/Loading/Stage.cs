using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using static CommunicationEvents;

public class Stage
{
    public int number = -1;

    public string category = null;
    public string name = null;
    public string description = null;
    public string scene = null;

    public bool use_install_folder = false;
    public List<Directories> hierarchie = null;

    [JsonIgnore]
    //TODO? update color if changed
    public bool completed_once { get { return player_record_list != null && player_record_list.Aggregate(false, (last, next) => last || next.Value.solved); } }
    public Dictionary<string, PlayerRecord> player_record_list = null;

    [JsonIgnore]
    public SolutionOrganizer solution = null;
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
    public PlayerRecord player_record = null;
    private string record_name { get { return name + "_save"; } }

    [JsonIgnore]
    public bool creatorMode = false;

    private string path = null;
    private static List<Directories>
        hierStage = new List<Directories> { Directories.Stages };

    private FactOrganizer hiddenState;

    public Stage()
    {
        InitOOP();
    }

    public Stage(string category, int number, string name, string description, string scene, bool local = true)
    {
        InitFields(category, number, name, description, scene, local);
        InitOOP();
    }

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

    public void InitFields(string category, int number, string name, string description, string scene, bool local)
    {
        this.category = category;
        this.number = number;
        this.name = name;
        this.description = description;
        this.scene = scene;
        this.use_install_folder = !local;
    }

    private void InitOOP()
    {
        solution = new SolutionOrganizer();
        player_record = new PlayerRecord(record_name);
        player_record_list = new Dictionary<string, PlayerRecord>();
    }

    public void ClearAll()
    {
        ClearSolution();
        ClearPlay();
        ClearALLRecords();
    }

    public void ClearSolution()
    {
        solution.hardreset(false);
        solution = new SolutionOrganizer();
    }

    public void ClearPlay()
    {
        player_record.factState.hardreset(false);
        player_record = new PlayerRecord(record_name);
    }

    public void ClearALLRecords()
    {
        foreach (var record in player_record_list.Values.ToList())
            deletet_record(record, false);

        player_record_list = new Dictionary<string, PlayerRecord>();
    }

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

    public bool set_record(PlayerRecord record)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        if (record != null)
            if(!player_record_list.ContainsKey(record.name))
                return false;
            else if (!record.load(hierarchie))
            {
                deletet_record(record);
                hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
                return false;
            }

        player_record = record == null ? new PlayerRecord(record_name) : record.Clone(hierarchie);
        player_record.name = record_name;

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);

        store(false);

        return true;
    }

    public void push_record(double seconds_s = -1, bool force_push = false)
    {
        if(!force_push && StageStatic.devel && creatorMode)
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

    public void delete(bool player_record_too)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        solution.delete();
        player_record.delete(hierarchie);
        if (player_record_too)
            foreach (var r in player_record_list)
                r.Value.delete(hierarchie);

        if (File.Exists(path))
            File.Delete(path);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
    }

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
            if(solution != null && solution.ValidationSet.Count > 0 && !solution.ValidationSet.Aggregate(false, (last, next) => last || next.IsEmpty()))
                solution.store(name, hierarchie, use_install_folder);
        }

        if (player_record != null)
            player_record.store(hierarchie, true);

        foreach (var track in player_record_list)
            track.Value.store(hierarchie, false);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
    }

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

    public static bool ShallowLoad(ref Stage set, string path)
    {
        if (!System.IO.File.Exists(path))
            return false;

        set = JSONManager.ReadFromJsonFile<Stage>(path);
        set.path = path;

        set.hierarchie ??= new List<Directories>();
        set.hierarchie.AddRange(hierStage.AsEnumerable());
        if (!set.player_record.load(set.hierarchie))
            set.player_record = new PlayerRecord(set.record_name);
        set.hierarchie.RemoveRange(set.hierarchie.Count - hierStage.Count, hierStage.Count);

        return true;
    }

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

    public void ResetPlay()
    {
        ClearPlay();
        store(true);
    }

    public void ResetSaves()
    {
        ClearPlay();
        ClearALLRecords();
        store(true);
    }

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

public class PlayerRecord
{
    public bool solved = false;
    public long date = System.DateTime.Now.ToBinary();
    public double seconds = 0;

    [JsonIgnore]
    public FactOrganizer factState = null;
    public string name = null;

    private static List<Directories>
        hierStage = new List<Directories> { /*Directories.FactStateMachines*/ };

    public PlayerRecord() { }

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
        if (!loadable) {
            return false;
        }

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
        return true;
    }

    public void delete(List<Directories> hierarchie)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        FactOrganizer.delete(name, hierarchie, false);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
    }

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