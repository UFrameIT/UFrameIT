using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    public void LoadStartScreen()
    {
        CommunicationEvents.Facts.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
