using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void LevelReset()
    {
        CommunicationEvents.LevelReset.Invoke(); // currently unused
        Level.solved = false; // needed?
        CommunicationEvents.LevelFacts.hardreset(false); // delete Facts at Server
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
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
