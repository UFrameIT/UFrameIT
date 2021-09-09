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

    public string name = null;
    public string description = null;
    public string scene = null;

    public bool use_install_folder = false;
    public List<Directories> hierarchie = null;

    [JsonIgnore]
    public SolutionOrganizer solution = null;
    [JsonIgnore]
    public FactOrganizer factState = null;

    [JsonIgnore]
    public bool creatorMode = false;

    private static List<Directories>
        hierStage = new List<Directories> { Directories.Stages };

    private FactOrganizer hiddenState;

    public Stage() { }

    public Stage(int number, string name, string description, string scene, bool local = true)
    {
        this.number = number;
        this.name = name;
        this.description = description;
        this.scene = scene;
        this.use_install_folder = !local;

        solution = new SolutionOrganizer();
        factState = new FactOrganizer();
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
            solution.invoke = false;
            factState.Undraw();

            factState = hiddenState;
            factState.Draw();
        }

    }

    public void store()
    {
        hierarchie ??= new List<Directories>();
        hierarchie.AddRange(hierStage.AsEnumerable());

        if (creatorMode)
        {
            string path = CreatePathToFile(out bool exists, name, "JSON", hierarchie, use_install_folder);
            //TODO: if exists
            hierarchie.RemoveRange(hierarchie.Count - hierStage.Count, hierStage.Count);
            JSONManager.WriteToJsonFile(path, this, 0);
            hierarchie.AddRange(hierStage.AsEnumerable());

            solution.store(name, hierarchie, use_install_folder);
        }
        
        factState.store(name, hierarchie, false);

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


        factState ??= new FactOrganizer(false);
        loadable = FactOrganizer.load(ref factState, false, name, hierarchie, false);

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

}