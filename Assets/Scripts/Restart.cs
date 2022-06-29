using UnityEngine;
using UnityEngine.SceneManagement;
using static UIconfig;
using static Restart_script;

public class Restart : MonoBehaviour
{
    public void LevelReset()
    {

        Restart_script resClass = new Restart_script();
        resClass.LevelReset();
        

    }

    public void LoadMainMenue()
    {
        Restart_script resClass = new Restart_script();
        resClass.LoadMainMenue(); 
    }


    public void StageFactState_modundo()
    {
        Restart_script resClass = new Restart_script();
        resClass.LoadMainMenue();
    }
    public void StageFactState_modredo()
    {
        Restart_script resClass = new Restart_script();
        resClass.StageFactState_modredo();
    }

    public void StageFactState_modreset()
    {
        Restart_script resClass = new Restart_script();
        resClass.StageFactState_modreset();
    }

    public void Stage_modsave()
    {
        Restart_script resClass = new Restart_script();
        resClass.Stage_modsave();
    }

    public void StageFactState_modload()
    {
        Restart_script resClass = new Restart_script();
        resClass.StageFactState_modload();
    }
           
   


    public void LoadStartScreen()
    {
        Restart_script resClass = new Restart_script();
        resClass.LoadStartScreen();
    }

    public void OnApplicationQuit()
    {
        Restart_script resClass = new Restart_script();
        resClass.OnApplicationQuit();
    }
}
