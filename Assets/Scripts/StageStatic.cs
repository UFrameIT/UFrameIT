using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StageStatic
{
    public static Dictionary<string, Stage> StageOfficial;
    public static Dictionary<string, Stage> StageLocal;

    public static string current_name;
    public static bool local_stage;
    public static bool devel = false;
    public static Mode mode;

    public static List<string> Worlds
    {
        get { return _Worlds ?? GenerateWorldList(); }
    }
    private static List<string> _Worlds = null;

    public static Stage stage {
        get {
            return (local_stage ? StageLocal : StageOfficial)[current_name];
        }
        set {
            current_name = value.name;
            local_stage = !value.use_install_folder;

            (local_stage ? StageLocal : StageOfficial).Remove(current_name);
            (local_stage ? StageLocal : StageOfficial).Add(current_name, value);

            bool tmp = value.creatorMode;
            value.creatorMode = true;
            value.store();
            value.creatorMode = tmp;
        }
    }

    // TODO! generate at buildtime
    private static List<string> GenerateWorldList()
    {
        _Worlds = new List<string>();

        string world = "World";
        string ending = ".unity";
        foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                string name = new System.IO.FileInfo(scene.path).Name;
                name = name.Substring(0, name.Length - ending.Length);

                if (0 == string.Compare(name, name.Length - world.Length, world, 0, world.Length))
                {
                    _Worlds.Add(name);
                }
            }
        }

        return _Worlds;
    }

    public enum Mode
    {
        Play,
        Create,
    }

    public static void SetMode(Mode mode, GameObject gameObject = null)
    {
        gameObject ??= new GameObject();

        // handle StageStatic.mode
        switch (StageStatic.mode = mode)
        {
            case Mode.Play:
                gameObject.UpdateTagActive("CreatorMode", false);
                break;
            case Mode.Create:
                gameObject.UpdateTagActive("CreatorMode", true);
                break;
        }

        // handle stage mode
        switch (mode)
        {
            case Mode.Play:
            case Mode.Create:
                stage.SetMode(mode == Mode.Create);
                break;
        }

    }

    public static int Validate(int id, string name, string description, string scene)
    {
        int error = 0;

        error = (error << 1) + (!Worlds.Contains(scene) ? 1 : 0);
        error = (error << 1) + (false ? 1 : 0);
        error = (error << 1) + (name.Length == 0 || ContainsKey(name, true) ? 1 : 0);
        error = (error << 1) + (ContainsNumber(id, true) ? 1 : 0);

        return error;
    }

    public static int LoadNewStage(int id, string name, string description, string scene)
    {
        int ret = Validate(id, name, description, scene);
        if (ret != 0)
            return ret;

        stage = new Stage(id, name, description, scene, true);
        stage.creatorMode = true;
        stage.store();

        LoadCreate();
        return ret;
    }

    public static void LoadCreate()
    {
        devel = true;
        Loader.LoadScene(stage.scene);
    }

    public static int NextNumber(bool local)
    {
        var numbers = (local ? StageLocal : StageOfficial).Values.Select(s => s.number).ToList();

        if (0 == numbers.Count)
            return 1;

        numbers.Sort();
        int last = numbers[0];
        foreach (int i in numbers)
        {
            if (i > last && i != last + 1)
                return last + 1;
            last = i;
        }

        return last + 1;
    }

    public static bool ContainsNumber(int i, bool local)
    {
        return (local ? StageLocal : StageOfficial).Values.Select(s => s.number).Contains(i);
    }

    public static void ShallowLoadStages()
    {
        StageOfficial = Stage.Grup(null, true);
        StageLocal = Stage.Grup(null, false);
    }

    public static void SetStage(string name, bool local)
    {
        local_stage = local;
        current_name = name;
    }

    public static Stage GetStage(string name, bool local)
    {
        return (local ? StageLocal : StageOfficial)[name];
    }

    public static void Delete(Stage stage)
    {
        GetStage(stage.name, !stage.use_install_folder).delete();
        (!stage.use_install_folder ? StageLocal : StageOfficial).Remove(stage.name);
    }

    public static bool LoadInitStage(string name, bool local = false, bool restore_session = true, GameObject gameObject = null)
    {
        if (!ContainsKey(name, local))
            return false;

        bool old_l = local_stage;
        string old_n = current_name;
        SetStage(name, local);

        if (!LoadInitStage(restore_session, gameObject))
        {
            local_stage = old_l;
            current_name = old_n;
            return false;
        }

        return true;
    }

    public static bool LoadInitStage(bool restore_session, GameObject gameObject = null)
    {
        if (current_name == null || current_name.Length == 0 || !stage.DeepLoad())
            return false;

        gameObject ??= new GameObject();

        if (restore_session)
        {
            stage.factState.invoke = true;
            stage.factState.Draw();
        }
        else
            stage.factState = new FactOrganizer(true);

        gameObject.UpdateTagActive("DevelopingMode", devel);
        SetMode(stage.creatorMode ? Mode.Create : Mode.Play);
        return true;
    }

    public static bool ContainsKey(string key)
    {
        return ContainsKey(key, local_stage);
    }
    public static bool ContainsKey(string key, bool local = false)
    {
        return (local ? StageLocal : StageOfficial).ContainsKey(key);
    }
}
