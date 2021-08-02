using UnityEngine;
using static StartServer;

public class Restart : MonoBehaviour
{
    public void LoadStartScreen()
    {
        process.Kill();
        Level.solved = false;
        //TODO: CommunicationEvents.Facts.Clear();
        CommunicationEvents.Facts.hardreset();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OnApplicationQuit()
    {
        process.Kill();
    }
}
