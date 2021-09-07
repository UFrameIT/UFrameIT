using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void LevelReset()
    {
        Loader.UnloadStage();
        Loader.LoadStage(GlobalStatic.stage.name, !GlobalStatic.stage.use_install_folder, false);
    }

    public void LoadMainMenue()
    {
        SceneManager.LoadScene("MainMenue");
    }

    public void LoadStartScreen()
    {
        StartServer.process.Kill();  // null reference exception if Server started manually
        SceneManager.LoadScene(0);
    }

    public void OnApplicationQuit()
    {
        StartServer.process.Kill();
    }
}
