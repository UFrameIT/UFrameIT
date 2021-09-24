using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;

public class IPTest : MonoBehaviour
{
      
    //public Button AfterIPset1; //andr
    //public Text buttonText;
    
	
    

  

    void Start()
    {
	
	
	
    }


   public void SetText(string text)
	{
		Text txt = transform.Find("Text").GetComponent<Text>();
       		//txt.text = text; 
		txt.text = CommunicationEvents.ServerAdress;
		
		//SceneManager.LoadScene("Andr_TreeWorld");

	}





   void Update() 
	{
	

	}



}
