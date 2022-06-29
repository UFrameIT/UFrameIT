using UnityEngine;
using UnityEngine.SceneManagement;
using static UIconfig;

public class WaitingForLocalMMT : MonoBehaviour
{
   
    

    private void Start()
    {
        
        
    }

    private void Update()
    {
        
        if (CommunicationEvents.ServerRunning == true)
        {
            CommunicationEvents.ServerAdress = CommunicationEvents.ServerAddressLocal;
            UnityEngine.Debug.Log("StartMainMenue");
            SceneManager.LoadScene("MainMenue");
        }

    }
}