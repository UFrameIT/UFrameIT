using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CommunicationEvents;

public static class Loader
{
    private static AsyncOperation loadingscene;
    private static string nextscene;

    private class MonoDummy : MonoBehaviour { };

    public static float progress { 
        get { 
            return loadingscene == null ? 1f : loadingscene.progress; 
        } 
    }

    public static bool isDone {
        get {
            return loadingscene == null ? true : loadingscene.isDone;
        }
    }

    public static void UnloadStage()
    {
        StageStatic.stage.factState.hardreset(false);
        StageStatic.stage.solution.hardreset(false);
        StageStatic.devel = false;
        Fact.Clear();
    }

    public static bool LoadStage(string name, bool local, bool restore_session = true)
    {
        if (!StageStatic.LoadInitStage(name, local, restore_session))
            return false;

        LoadScene(StageStatic.stage.scene);
        return true;
    }

    public static void LoadScene(string scene)
    {
        nextscene = scene;
        SceneManager.LoadScene("LoadingScene");
        // loads LoadingScreen, which will call LoaderCallback() in LoadingScreenPercentage
    }

    public static void LoadScene(Scene scene)
    {
        nextscene = scene.name;
        SceneManager.LoadScene("LoadingScene");
        // loads LoadingScreen, which will call LoaderCallback() in LoadingScreenPercentage
    }

    public static void PostLoad()
    {
        ;
    }

    public static void LoaderCallback()
    {
        loadingscene = SceneManager.LoadSceneAsync(nextscene);
    }
}
