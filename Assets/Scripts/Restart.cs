using UnityEngine;
using UnityEngine.SceneManagement;
using static UIconfig;

public class Restart : MonoBehaviour
{
    public void LevelReset()
    {

        StageStatic.stage.ResetPlay();
        //UIconfig.CanvasOnOff_Array[2] = 0;
        //UIconfig.GamePaused = false;
        //Time.timeScale = 1; // UIconfig.Game_TimeScale;
        Loader.LoadStage(StageStatic.stage.name, !StageStatic.stage.use_install_folder, false);
        //StageStatic.stage.factState.softreset();

    }

    public void LoadMainMenue()
    {
        //not over SceneManager.LoadingScreen as MainMenue is too light to need to load over a LoadingScreen
        SceneManager.LoadScene("MainMenue");
    }


    public void StageFactState_modundo()
    {
        StageStatic.stage.factState.undo();
    }
    public void StageFactState_modredo()
    {
        StageStatic.stage.factState.redo();
    }

    public void StageFactState_modreset()
    {
        StageStatic.stage.factState.softreset();
    }

    public void Stage_modsave()
    {
        StageStatic.stage.push_record();
    }

    public void StageFactState_modload()
    {
        StageStatic.stage.factState.hardreset();
        StageStatic.LoadInitStage(StageStatic.stage.name, !StageStatic.stage.use_install_folder);
    }
           
   


    public void LoadStartScreen()
    {
        StartServer.process.Kill(); // null reference exception if Server started manually
        SceneManager.LoadScene(0);
    }

    public void OnApplicationQuit()
    {
        StartServer.process.Kill(); // null reference exception if Server started manually
    }
}
