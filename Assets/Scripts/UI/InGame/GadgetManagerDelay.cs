using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class GadgetManagerDelay : MonoBehaviour
{

    public GameObject GobjWithGadgetManager;
    

    void Start()
    {
        //GobjWithGadgetManager.GetComponent<GadgetManager>().enabled = true;
        //GobjWithGadgetManager.GetComponent<GadgetManager>().enabled = true;
        //GobjWithGadgetManager.GetComponent<Gadget>().enabled = true;
        //GobjWithGadgetManager.SetActive(true);
        
        StartCoroutine(DelayRoutine());
    }

    IEnumerator DelayRoutine()
    {

        yield return new WaitForSeconds(1);
        //Activate ButtonGeneration 
        GobjWithGadgetManager.SetActive(true);
        print("GadgetManagerSuccessfullDelayed");
        StopCoroutine(DelayRoutine());
        print("Stopped:DelayRoutine()");



    }
}