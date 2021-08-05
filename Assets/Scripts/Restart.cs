using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void LevelReset()
    {
        CommunicationEvents.LevelReset.Invoke(); // currently unused
        Level.solved = false; // needed since static

        // delete Facts at Server
        CommunicationEvents.LevelFacts.hardreset(false);
        // only when generated! (in Level.cs)
        CommunicationEvents.SolutionManager.hardreset(false);

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
