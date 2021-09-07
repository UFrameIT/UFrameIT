using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalStatic
{
    public static Dictionary<string, Stage> StageOfficial;
    public static Dictionary<string, Stage> StageLocal;

    public static string current_name;
    public static bool local_stage;

    public static Stage stage {
        get {
            return (local_stage ? StageLocal : StageOfficial)[current_name];
        }
    }

    public enum Mode
    {
        Play,
        Create,
    }

    public static void SetMode(Mode mode)
    {
        switch (mode)
        {
            case Mode.Play:
            case Mode.Create:
                stage.SetMode(mode == Mode.Create);
                break;
        }
    }

    public static void ShallowLoadStages()
    {
        StageOfficial = Stage.Grup(null, true);
        StageLocal = Stage.Grup(null, false);
    }

    public static void SetStage(string name, bool local = false)
    {
        local_stage = local;
        current_name = name;
    }

    public static bool LoadInitStage(string name, bool local = false, bool restore_session = true)
    {
        if (!ExistsKey(name, local))
            return false;

        bool old_l = local_stage;
        string old_n = current_name;
        SetStage(name, local);

        if (!LoadInitStage(restore_session))
        {
            local_stage = old_l;
            current_name = old_n;
            return false;
        }

        return true;
    }

    public static bool LoadInitStage(bool restore_session = true)
    {
        if (current_name == null || current_name.Length == 0 || !stage.DeepLoad())
            return false;

        if (restore_session)
        {
            stage.factState.invoke = true;
            stage.factState.Draw();
        }
        else
            stage.factState = new FactOrganizer(true);

        return true;
    }

    public static bool ExistsKey(string key, bool local = false)
    {
        return (local ? StageLocal : StageOfficial).ContainsKey(key);
    }
}
