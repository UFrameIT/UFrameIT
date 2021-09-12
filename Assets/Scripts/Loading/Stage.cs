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
    public bool completed_once { get { return player_record_list != null && player_record_list.Aggregate(false, (last, next) => last || next.solved); } }
    public List<PlayerRecord> player_record_list = null;

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

    public Stage() { }

    public Stage(string category, int number, string name, string description, string scene, bool local = true)
    {
        this.category = category;
        this.number = number;
        this.name = name;
        this.description = description;
        this.scene = scene;
        this.use_install_folder = !local;

        solution = new SolutionOrganizer();
        player_record = new PlayerRecord(record_name);
        player_record_list = new List<PlayerRecord>();
    }

    public void CopyStates(Stage get)
    {
        this.solution = get.solution;
        this.player_record = get.player_record;
        this.player_record_list = get.player_record_list;
    }

    public void deletet_record(PlayerRecord record)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        record.delete(hierarchie);
        player_record_list.Remove(record);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
        store();
    }

    public bool set_record(PlayerRecord record)
    {
        if (record != null && !player_record_list.Contains(record))
            return false;

        player_record = record == null ? new PlayerRecord(record_name) : record;

        store(false);

        return true;
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
        solution.delete();
        player_record.delete(hierarchie);
        if (player_record_too)
            foreach (var r in player_record_list)
                r.delete(hierarchie);

        if (File.Exists(path))
            File.Delete(path);
    }

    public void store(bool reset = false)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        if (reset)
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
                solution.store(name, hierarchie, use_install_folder);
        }

        if (player_record != null)
            player_record.store(hierarchie);

        foreach (var track in player_record_list)
            track.store(hierarchie);

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
    }

    public static bool load(ref Stage set, string name, List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        Stage ret = new Stage();

        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        bool loadable = ShallowLoad(ref ret, name, hierarchie, use_install_folder);
        if (!loadable)
        {
            hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
            return false;
        }

        loadable = ret.DeepLoad();
        if (!loadable)
        {
            hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
            return false;
        }

        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
        set = ret;
        return true;
    }

    public static bool ShallowLoad(ref Stage set, string path)
    {
        if (!System.IO.File.Exists(path))
            return false;

        set = JSONManager.ReadFromJsonFile<Stage>(path);

        set.hierarchie ??= new List<Directories>();
        set.hierarchie.AddRange(hierStage.AsEnumerable());
        set.player_record.load(set.hierarchie);
        set.hierarchie.RemoveRange(set.hierarchie.Count - hierStage.Count, hierStage.Count);

        return true;
    }

    public static bool ShallowLoad(ref Stage set, string name, List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        string path = CreatePathToFile(out bool loadable, name, "JSON", hierarchie, use_install_folder);
        hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
        if (!loadable)
            return false;

        set = JSONManager.ReadFromJsonFile<Stage>(path);
        set.path = path;
        set.hierarchie = hierarchie;
        set.use_install_folder = use_install_folder;

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

    public void Reset()
    {
        store(true);
    }

    public bool CheckSolved()
    {
        float time_s = Time.time;
        bool solved =
            factState.DynamiclySolved(solution, out _, out List<List<string>> hits);

        if (solved)
            foreach (var hitlist in hits)
                foreach (var hit in hitlist)
                    AnimateExistingFactEvent.Invoke(factState[hit]);

        if (solved && player_record.time > 0)
        {
            player_record.solved = solved;
            player_record.time = time_s - player_record.time;

            var pr = player_record_list.FirstOrDefault();
            int i = pr == null ? 1 : pr.name_nr;
            for (; player_record_list.Exists(p => p.name_nr == i); i++) ;
            player_record.name = record_name + "_" + i.ToString();

            player_record_list.Add(player_record);
            player_record_list = player_record_list.OrderBy(p => p.time).ToList();

            var old = player_record;
            store(true);
            player_record = old;
        }

        return solved;
    }

}

public class PlayerRecord
{
    public bool solved = false;
    public float date = Time.time; // TODO: correct?
    public float time = 0;

    [JsonIgnore]
    public FactOrganizer factState = null;
    public string name = null;
    public int name_nr = 1;

    private static List<Directories>
        hierStage = new List<Directories> { /*Directories.FactStateMachines*/ };

    public PlayerRecord() { }

    public PlayerRecord(string name) {
        this.name = name;
        factState = new FactOrganizer();
    }

    public void store(List<Directories> hierarchie)
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        if (factState != null)
            factState.store(name, hierarchie, false, false);

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
}