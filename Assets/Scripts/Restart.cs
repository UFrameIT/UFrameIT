using UnityEngine;
using static StartServer;

public class Restart : MonoBehaviour
{
    public void LoadStartScreen()
    {
        process.Kill();
        Level.solved = false;
        CommunicationEvents.Facts.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OnApplicationQuit()
    {
        process.Kill();
    }
}
