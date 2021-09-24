using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class StageStatic
{
    public static Dictionary<string, Stage> StageOfficial;
    public static Dictionary<string, Stage> StageLocal;
    public static Dictionary<string, int> Category = new Dictionary<string, int> {
        { "", -1 },
        { "Demo Category", 0 },
    };

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

    // TODO: set when encountering an error
    public static StageErrorStruct error {
        get;
        private set;
    }

    // TODO! generate at buildtime
    private static List<string> GenerateWorldList()
    {
#if UNITY_EDITOR

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
#else
        _Worlds = new List<string> {"TreeWorld", "RiverWorld"};
        //UnityEditor.Debug.Log("WorldList might be incomplete!");
#endif
        return _Worlds;
    }

    public enum Mode
    {
        Play,
        Create,
    }

    public struct StageErrorStruct
    {
        public bool category    { get { return state[0]; } set { state[0] = value; } }
        public bool id          { get { return state[1]; } set { state[1] = value; } }
        public bool name        { get { return state[2]; } set { state[2] = value; } }
        public bool description { get { return state[3]; } set { state[3] = value; } }
        public bool scene       { get { return state[4]; } set { state[4] = value; } }
        public bool local       { get { return state[5]; } set { state[5] = value; } }
        public bool load        { get { return state[6]; } set { state[6] = value; } }

        private bool[] state;

        public bool pass
        {
            get { return state.Aggregate(true, (last, next) => last && !next); }
        }

        public readonly static StageErrorStruct
            InvalidFolder = new StageErrorStruct(false, false, false, false, false, true, false),
            NotLoadable   = new StageErrorStruct(false, false, false, false, false, false, true);

        public StageErrorStruct(bool category, bool id, bool name, bool description, bool scene, bool local, bool load)
        {
            state = new bool[7];

            this.category = category;
            this.id = id;
            this.name = name;
            this.description = description;
            this.scene = scene;
            this.local = local;
            this.load = load;
        }
    }

    public static void SetMode(Mode mode, UnityEngine.GameObject gameObject = null)
    {
        gameObject ??= new UnityEngine.GameObject();

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

    public static StageErrorStruct Validate(string category, int id, string name, string description, string scene, bool local = true)
    {   
        return new StageErrorStruct(
            category.Length == 0,
            ContainsNumber(category, id, true),
            name.Length == 0 || ContainsKey(name, true) || ContainsKey(name, false),
            false,
            !Worlds.Contains(scene),
            local == false,
            false
            );
    }

    public static StageErrorStruct LoadNewStage(string category, int id, string name, string description, string scene)
    {
        StageErrorStruct ret = Validate(category, id, name, description, scene, true);
        if (!ret.pass)
            return ret;

        stage = new Stage(category, id, name, description, scene, true);
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

    public static bool ContainsNumber(string category, int i, bool local)
    {
        return (local ? StageLocal : StageOfficial).Values
            .Where(s => s.category == category)
            .Select(s => s.number)
            .Contains(i);
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
        GetStage(stage.name, !stage.use_install_folder).delete(true);
        (!stage.use_install_folder ? StageLocal : StageOfficial).Remove(stage.name);
    }

    public static bool LoadInitStage(string name, bool local = false, bool restore_session = true, UnityEngine.GameObject gameObject = null)
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

    public static bool LoadInitStage(bool restore_session, UnityEngine.GameObject gameObject = null)
    {
        if (current_name == null || current_name.Length == 0 || !stage.DeepLoad())
            return false;

        gameObject ??= new UnityEngine.GameObject();

        if (restore_session)
        {
            stage.factState.invoke = true;
            stage.factState.Draw();
        }
        else
        {
            stage.ResetPlay();
            if(devel) // block saving "player" progress
                stage.player_record.seconds = -1;
        }

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