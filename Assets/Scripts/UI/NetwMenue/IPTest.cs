using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;

using System.IO; //


public class MyClass
{
	public int level;
	public float timeElapsed;
	public string playerName;
}



public class IPTest : MonoBehaviour
{

	//public Button AfterIPset1; //andr
	//public Text buttonText;

	string path2; 
    

  

    void Start()
    {


		path2 = Application.persistentDataPath;

		//remove: \files
		if (!string.IsNullOrEmpty(path2))
		{
			path2= path2.TrimEnd(path2[path2.Length - 6]);
		}




		path2 = path2 + "/file";
		path2 = path2 + "/player.fun";

	}


   public void SetText(string text)
	{
		Text txt = transform.Find("Text").GetComponent<Text>();
		//txt.text = text; 


		//txt.text = CommunicationEvents.ServerAdress;
		//txt.text = ; 
		txt.text = Score_Load("test");
		//txt.text =  "jar:file://" + Application.dataPath + "!/assets/" + "1/scrolls.json";


		//SceneManager.LoadScene("Andr_TreeWorld");

	}




	public string Score_Load(string Directory_path)
	{
		//Data acquisition
		//var reader = new StreamReader(Application.persistentDataPath + "/" + Directory_path + "/date.json");
		//var reader = new StreamReader(Application.persistentDataPath + "/scrolls.json");
		//var reader = new StreamReader(Application.persistentDataPath + "/1/scrolls.json");
		var reader = new StreamReader(Application.persistentDataPath + "/test3/test7.json");
		//var reader = new StreamReader(Application.persistentDataPath + "Stages/factStateMAchines/TechDemo B_val.JSON");
		//var reader = new StreamReader(Application.persistentDataPath + "/Stages/TechDemo B.JSON");
		string json = reader.ReadToEnd();
		reader.Close();

		//SampleData mySampleFile = JsonUtility.FromJson<SampleData>(jsonStr);
		return json;//Convert for ease of use
	}






	void Update() 
	{
	

	}



}
