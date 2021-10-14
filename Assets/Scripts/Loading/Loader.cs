using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CommunicationEvents;

public static class Loader
{
    /// <summary> <see langword="null"/> or current <see cref="AsyncOperation"/> loading a <see cref="Scene"/>.</summary>
    private static AsyncOperation loadingscene;
    /// <summary> Defines last <see cref="Scene"/> loaded by this and/ or to be loaded when calling <see cref="LoaderCallback"/>.</summary>
    private static string nextscene;

    private class MonoDummy : MonoBehaviour { };

    /// <summary>
    /// <c>return <see cref="loadingscene"/> == <see langword="null"/> ? 1f : <see cref="loadingscene"/>.progress;</c>
    /// </summary>
    public static float progress { 
        get { 
            return loadingscene == null ? 1f : loadingscene.progress; 
        } 
    }

    /// <summary>
    /// <c>return <see cref="loadingscene"/> == null ? <see langword="true"/> : <see cref="loadingscene"/>.isDone;</c>
    /// </summary>
    public static bool isDone {
        get {
            return loadingscene == null ? true : loadingscene.isDone;
        }
    }

    /// <summary>
    /// Tries to init (via <see cref="StageStatic.LoadInitStage(string, bool, bool, GameObject)"/>) defined <see cref="Stage"/> and load it (via <see cref="LoadScene(string)"/>).
    /// </summary>
    /// <param name="name">defines <see cref="Stage.name"/></param>
    /// <param name="local">defines !<see cref="Stage.use_install_folder"/></param>
    /// <param name="restore_session">see <see cref="StageStatic.LoadInitStage(string, bool, bool, GameObject)"/></param>
    /// <returns>see <see cref="StageStatic.LoadInitStage(string, bool, bool, GameObject)"/></returns>
    public static bool LoadStage(string name, bool local, bool restore_session)
    {
        if (!StageStatic.LoadInitStage(name, local, restore_session))
            return false;

        LoadScene(StageStatic.stage.scene);
        return true;
    }

    /// <summary>
    /// Sets <see cref="nextscene"/> and synchronously loads "LoadingScene", which in turn will asynchronously load <paramref name="scene"/>.
    /// </summary>
    /// <param name="scene">sets <see cref="nextscene"/></param>
    public static void LoadScene(string scene)
    {
        nextscene = scene;
        SceneManager.LoadScene("LoadingScene");
        // loads LoadingScreen, which will call LoaderCallback() in LoadingScreenPercentage
    }

    /// <summary>
    /// Wrapps <see cref="LoadScene(string)"/>
    /// <br/> // TODO: needed? used in Inspector?
    /// </summary>
    /// \copybrief LoadScene(string)
    public static void LoadScene(Scene scene)
    {
        LoadScene(scene.name);
    }

    /// <summary> Called when <see cref="LoadingScreenPercentage"/> is destroyed. </summary>
    /// <remarks> Does currently nothing. </remarks>
    public static void PostLoad()
    {
        ;
    }

    /// <summary>
    /// Called when <see cref="LoadingScreenPercentage"/> starts and loads asynchronously <see cref="nextscene"/>. <br/>
    /// sets <see cref="loadingscene"/>
    /// </summary>
    public static void LoaderCallback()
    {
        loadingscene = SceneManager.LoadSceneAsync(nextscene);
    }
}
