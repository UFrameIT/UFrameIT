using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void LevelReset()
    {
        CommunicationEvents.LevelReset.Invoke(); // currently unused

        // delete Facts at Server
        CommunicationEvents.LevelFacts.hardreset(false);
        // only when generated! (in Level.cs)
        CommunicationEvents.SolutionManager.hardreset(false);

        Fact.Clear();

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
