using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;



public class FindIP : MonoBehaviour
{
      
    //public Button AfterIPset1; //andr
    //public Button TextSlot1;
    
      

    public InputField mainInputField;


  

    void Start()
    {
	
	Screen.orientation = ScreenOrientation.LandscapeLeft; 
	//mainInputField.text = "Enter IP Here...";

	
	GameObject.Find("IPSlot1").GetComponentInChildren<Text>().text = "Press to Start with " + CommunicationEvents.ServerAddress1;
	GameObject.Find("IPSlot2").GetComponentInChildren<Text>().text = "Press to Start with " + CommunicationEvents.ServerAddress2;
	GameObject.Find("InfoIP1").GetComponentInChildren<Text>().text = "Please select the IP-Address of your Server. \nFor a custom-address use format: ipaddress:port ";
	
	//Button myBtn = go.GetComponent<Button>();
	

    }


   public void SetText(string text)
	{
		Text txt = transform.Find("Text").GetComponent<Text>();
       		//txt.text = text; 
		txt.text = mainInputField.text;
		CommunicationEvents.ServerAdress = "http://" + mainInputField.text;
		SceneManager.LoadScene("Andr_TreeWorld");

	}




   public void _TestInput() 
   {
		
		CommunicationEvents.ServerAdress = mainInputField.text;       

   }

  public void Slot1() 
   {
		
		CommunicationEvents.ServerAdress = "http://" +  CommunicationEvents.ServerAddress1;
		SceneManager.LoadScene("Andr_TreeWorld");       
   }

  public void Slot2() 
   {
		
		CommunicationEvents.ServerAdress = "http://" +  CommunicationEvents.ServerAddress2;
		SceneManager.LoadScene("Andr_TreeWorld");       
   }


   void Update() 
	{
		

	}



}
