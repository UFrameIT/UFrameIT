using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;
using static CommunicationEvents;
using static UIconfig;


//Uploading Files from StreamingAsset folder to the Persistent Folder for Android Devices
public static class StreamingAssetLoader
{



    //""
    public static string file_1_path = "";
    public static string file_1 = "scrolls.json";

    //Stages
    public static string file_2_path = "Stages";
    public static string file_2 = "TechDemo A.JSON";

    public static string file_3_path = "Stages";
    public static string file_3 = "TechDemo B.JSON";


    //ValidationSets
    public static string file_4_path = "Stages/ValidationSets";
    public static string file_4 = "TechDemo A_val.JSON";

    public static string file_5_path = "Stages/ValidationSets";
    public static string file_5 = "TechDemo B_val.JSON";

    //FactStateMachines
    public static string file_6_path = "Stages/ValidationSets/FactStateMachines";
    public static string file_6 = "TechDemo A_sol.JSON";

    public static string file_7_path = "Stages/ValidationSets/FactStateMachines";
    public static string file_7 = "TechDemo B_sol.JSON";


    public static string file_8_path = "Config";
    public static string file_8 = "Network.JSON";

    public static string file_9_path = "";
    public static string file_9 = "";

    public static string file_10_path = "";
    public static string file_10 = "";


    public static bool checkPDP()
    {

        string filePath = Application.persistentDataPath + "/Config/Network.json";
        if (System.IO.File.Exists(filePath))
        {
            return true;
        }

        return false;
    }




    public static void NetworkJSON_Save()
    {
        NetworkJSON myObject = new NetworkJSON();
        //MyClass myObject = new MyClass();
        myObject.newIP = CommunicationEvents.newIP;
        myObject.lastIP = CommunicationEvents.lastIP;
        myObject.IPslot1 = CommunicationEvents.IPslot1;
        myObject.IPslot2 = CommunicationEvents.IPslot2;
        myObject.IPslot3 = CommunicationEvents.IPslot3;
        myObject.selecIP = CommunicationEvents.selecIP;
        myObject.ControlMode = UIconfig.controlMode;
        myObject.TouchMode = UIconfig.touchControlMode;
        myObject.TAvisibility = UIconfig.TAvisibility;
        myObject.autoOSrecognition = CommunicationEvents.autoOSrecognition;
        myObject.Opsys = CommunicationEvents.Opsys;
        myObject.FrameITUIversion = UIconfig.FrameITUIversion;
        myObject.InputManagerVersion = UIconfig.InputManagerVersion;
        myObject.colliderScale_all = UIconfig.colliderScale_all;
        myObject.cursorSize = UIconfig.cursorSize;
        myObject.camRotatingSensitivity = UIconfig.camRotatingSensitivity;




        //Data storage
        SafeCreateDirectory(Application.persistentDataPath + "/Config");
        //string json = JsonUtility.ToJson(date);
        string json = JsonUtility.ToJson(myObject);
        StreamWriter Writer = new StreamWriter(Application.persistentDataPath + "/Config/Network.json");
        Writer.Write(json);
        Writer.Flush();
        Writer.Close();
    }
    public static DirectoryInfo SafeCreateDirectory(string path)
    {
        //Generate if you don't check if the directory exists
        if (Directory.Exists(path))
        {
            return null;
        }
        return Directory.CreateDirectory(path);
    }





    public static void ResetPlayerConfig()
    {
        RereadFiles_PersistentDataPath();
        NetworkJSON_Load();
    }

    public static void ResetDataPath()
    {
        RereadFiles_DataPath();
    }

    public static void ResetStreamingAsset()
    {

        RereadFiles_PersistentDataPath();
        RereadFiles_DataPath();
        NetworkJSON_Load();
        //CSform.CheckIPAdr();
    }



    public static void RereadFiles_PersistentDataPath()
    {
        RereadFileUWR(StreamingAssetLoader.file_8_path, StreamingAssetLoader.file_8, 1);
        RereadFileUWR(StreamingAssetLoader.file_1_path, StreamingAssetLoader.file_1, 1);
    }
    public static void RereadFiles_DataPath()
    {
        RereadFileUWR(StreamingAssetLoader.file_2_path, StreamingAssetLoader.file_2, 0);
        RereadFileUWR(StreamingAssetLoader.file_3_path, StreamingAssetLoader.file_3, 0);
        RereadFileUWR(StreamingAssetLoader.file_4_path, StreamingAssetLoader.file_4, 0);
        RereadFileUWR(StreamingAssetLoader.file_5_path, StreamingAssetLoader.file_5, 0);
        RereadFileUWR(StreamingAssetLoader.file_6_path, StreamingAssetLoader.file_6, 0);
        RereadFileUWR(StreamingAssetLoader.file_7_path, StreamingAssetLoader.file_7, 0);
        RereadFileUWR(StreamingAssetLoader.file_9_path, StreamingAssetLoader.file_9, 0);
        RereadFileUWR(StreamingAssetLoader.file_10_path, StreamingAssetLoader.file_10, 0);
    }


    public static void NetworkJSON_Load()
    {
        var reader = new StreamReader(Application.persistentDataPath + "/Config/Network.JSON");
        string json = reader.ReadToEnd();
        reader.Close();

        NetworkJSON myObjs = JsonUtility.FromJson<NetworkJSON>(json);
        if (string.IsNullOrEmpty(myObjs.newIP))
        {
            CommunicationEvents.newIP = "";
        }
        else
        {
            CommunicationEvents.newIP = myObjs.newIP;
        }
        if (string.IsNullOrEmpty(myObjs.lastIP))
        {
            CommunicationEvents.lastIP = "";
        }
        else
        {
            CommunicationEvents.lastIP = myObjs.lastIP;
        }
        if (string.IsNullOrEmpty(myObjs.IPslot1))
        {
            CommunicationEvents.IPslot1 = "";
        }
        else
        {
            CommunicationEvents.IPslot1 = myObjs.IPslot1;//myObjs.IPslot1;
        }
        if (string.IsNullOrEmpty(myObjs.IPslot2))
        {
            CommunicationEvents.IPslot2 = "";//"Empty";
        }
        else
        {
            CommunicationEvents.IPslot2 = myObjs.IPslot2;
        }
        if (string.IsNullOrEmpty(myObjs.IPslot3))
        {
            CommunicationEvents.IPslot3 = "";
        }
        else
        {
            CommunicationEvents.IPslot3 = myObjs.IPslot3;
        }
        if (string.IsNullOrEmpty(myObjs.selecIP))
        {
            CommunicationEvents.selecIP = "";
        }
        else
        {
            CommunicationEvents.selecIP = myObjs.selecIP;
        }
        if (false)
        {

        }
        else
        {
            UIconfig.controlMode = myObjs.ControlMode;
        }
        if (false)
        {

        }
        else
        {
            UIconfig.touchControlMode = myObjs.TouchMode;
        }
        if (false)
        {

        }
        else
        {
            UIconfig.TAvisibility = myObjs.TAvisibility;
        }
        if (false)
        {

        }
        else
        {
            CommunicationEvents.autoOSrecognition = myObjs.autoOSrecognition;
        }
        if (false)
        {

        }
        else
        {
            CommunicationEvents.Opsys = myObjs.Opsys;
        }
        if (false)
        {

        }
        else
        {
            UIconfig.FrameITUIversion = myObjs.FrameITUIversion;
        }
        if (false)
        {

        }
        else
        {
            UIconfig.InputManagerVersion = myObjs.InputManagerVersion;
        }
        if (false)
        {

        }
        else
        {
            UIconfig.colliderScale_all = myObjs.colliderScale_all;
        }
        if (false)
        {

        }
        else
        {
            UIconfig.cursorSize = myObjs.cursorSize;
        }
        if (false)
        {

        }
        else
        {
            UIconfig.camRotatingSensitivity = myObjs.camRotatingSensitivity;
        }

    }

    public static void RereadFileUWR(string pathfolders, string fileName, int toMainpath)
    {
        if (fileName == "")
        {
            return;
        }
        string destpathf = pathfolders;
        string destname = fileName;


        string sourcePath = Path.Combine(Application.streamingAssetsPath, pathfolders);
        sourcePath = Path.Combine(sourcePath, fileName);
        var loadingRequest = UnityWebRequest.Get(sourcePath);
        loadingRequest.SendWebRequest();
        while (!loadingRequest.isDone)
        {
            if (loadingRequest.result == UnityWebRequest.Result.ConnectionError || loadingRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                break;
            }
        }
        if (loadingRequest.result == UnityWebRequest.Result.ConnectionError || loadingRequest.result == UnityWebRequest.Result.ProtocolError)
        {

        }
        else
        {
            //copies and unpacks file from apk to persistentDataPath where it can be accessed
            string destinationPath = "";
            if (toMainpath == 0 && CommunicationEvents.Opsys != 1) { destinationPath = Path.Combine(Application.dataPath, destpathf); }
            else
            {
                destinationPath = Path.Combine(Application.persistentDataPath, destpathf);
            }

            if (Directory.Exists(destinationPath) == false)
            {
               Directory.CreateDirectory(destinationPath);
            }
            File.WriteAllBytes(Path.Combine(destinationPath, destname), loadingRequest.downloadHandler.data);
            



        }
    }


    //WWW has been replaced with UnityWebRequest.
    /*
     public static string RereadFileNA(string pathfolders, string fileName, string destpathf, string destname)
     {
             if (fileName == "")
             {
                 return "noName";
             }




             // copies and unpacks file from apk to persistentDataPath where it can be accessed
             string destinationPath = Path.Combine(Application.persistentDataPath, destpathf);

             if (Directory.Exists(destinationPath) == false)
             {
                 Directory.CreateDirectory(destinationPath);
             }


             destinationPath = Path.Combine(destinationPath, destname);


             string sourcePath = Path.Combine(Application.streamingAssetsPath, pathfolders);
             sourcePath = Path.Combine(sourcePath, fileName);

  #if UNITY_EDITOR
         //string sourcePath = Path.Combine(Application.streamingAssetsPath, pathfolders);
         //sourcePath = Path.Combine(sourcePath, fileName);
  #else
         //string sourcePath = "jar:file://" + Application.dataPath + "!/assets/" + fileName;

  #endif

         //UnityEngine.Debug.Log(string.Format("{0}-{1}-{2}-{3}", sourcePath,  File.GetLastWriteTimeUtc(sourcePath), File.GetLastWriteTimeUtc(destinationPath)));




         //copy whatsoever

         //if DB does not exist in persistent data folder (folder "Documents" on iOS) or source DB is newer then copy it
         //if (!File.Exists(destinationPath) || (File.GetLastWriteTimeUtc(sourcePath) > File.GetLastWriteTimeUtc(destinationPath)))
         if (true)
             {
                 if (sourcePath.Contains("://"))
                 {
                     // Android  
                     WWW www = new WWW(sourcePath);
                     while (!www.isDone) {; }                // Wait for download to complete - not pretty at all but easy hack for now 
                     if (string.IsNullOrEmpty(www.error))
                     {
                         File.WriteAllBytes(destinationPath, www.bytes);
                     }
                     else
                     {
                         Debug.Log("ERROR: the file DB named " + fileName + " doesn't exist in the StreamingAssets Folder, please copy it there.");
                     }
                 }
                 else
                 {
                     // Mac, Windows, Iphone                
                     //validate the existens of the DB in the original folder (folder "streamingAssets")
                     if (File.Exists(sourcePath))
                     {
                         //copy file - alle systems except Android
                         File.Copy(sourcePath, destinationPath, true);
                     }
                     else
                     {
                         Debug.Log("ERROR: the file DB named " + fileName + " doesn't exist in the StreamingAssets Folder, please copy it there.");
                     }
                 }
             }

             StreamReader reader = new StreamReader(destinationPath);
             var jsonString = reader.ReadToEnd();
             reader.Close();


             return jsonString;
     }
    */

    public static void RereadFileUW4(string pathfolders, string fileName, string destpathf, string destname)
    {
            if (fileName == "")
            {
                return;
            }


            string sourcePath = Path.Combine(Application.streamingAssetsPath, pathfolders);
            sourcePath = Path.Combine(sourcePath, fileName);
            var loadingRequest = UnityWebRequest.Get(sourcePath);
            loadingRequest.SendWebRequest();
            while (!loadingRequest.isDone)
            {
                if (loadingRequest.result == UnityWebRequest.Result.ConnectionError || loadingRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    break;
                }
            }
            if (loadingRequest.result==UnityWebRequest.Result.ConnectionError || loadingRequest.result==UnityWebRequest.Result.ProtocolError)
            {

            }
            else
            {
                //copies and unpacks file from apk to persistentDataPath where it can be accessed
                string destinationPath = Path.Combine(Application.persistentDataPath, destpathf);

                if (Directory.Exists(destinationPath) == false)
                {
                    Directory.CreateDirectory(destinationPath);
                }
                File.WriteAllBytes(Path.Combine(destinationPath, destname), loadingRequest.downloadHandler.data);
            }




    }
    


    public class MyClass
    {
        public int level;
        public float timeElapsed;
        public string playerName;
    }


    public static void Score_Save(string Directory_path, string date)
    {
        MyClass myObject = new MyClass();
        myObject.level = 1;
        myObject.timeElapsed = 47.5f;
        myObject.playerName = "Dr Charles Francis";

        //Data storage
        SafeCreateDirectory(Application.persistentDataPath + "/" + Directory_path);
        //string json = JsonUtility.ToJson(date);
        string json = JsonUtility.ToJson(myObject);
        StreamWriter Writer = new StreamWriter(Application.persistentDataPath + "/" + Directory_path + "/date.json");
        Writer.Write(json);
        Writer.Flush();
        Writer.Close();



            //RereadFileUW("", "scrolls.json", "test3", "test6.json");
            //RereadFileUW("Stages", "TechDemo A.JSON", "test3", "test7.json");

        /*
        RereadFileUWR("", "scrolls.json");
        RereadFileUWR("Stages", "TechDemo A.JSON");
        RereadFileUWR("Stages", "TechDemo B.JSON");
        RereadFileUWR("Stages/ValidationSets", "TechDemo A_val.JSON");
        RereadFileUWR("Stages/ValidationSets", "TechDemo B_val.JSON");
        RereadFileUWR("Stages/ValidationSets/FactStateMachines", "TechDemo A_val.JSON");
        RereadFileUWR("Stages/ValidationSets/FactStateMachines", "TechDemo B_val.JSON");
        */



    }



    public static string Score_Load(string Directory_path)
    {
        //Data acquisition
        //var reader = new StreamReader(Application.persistentDataPath + "/" + Directory_path + "/date.json");
        //var reader = new StreamReader(Application.persistentDataPath + "/scrolls.json");
        //var reader = new StreamReader(Application.persistentDataPath + "/1/scrolls.json");
        //var reader = new StreamReader(Application.persistentDataPath + "/test3/test7.json");
        //var reader = new StreamReader(Application.persistentDataPath + "Stages/factStateMAchines/TechDemo B_val.JSON");
        //var reader = new StreamReader(Application.persistentDataPath + "/Stages/TechDemo B.JSON");
        var reader = new StreamReader(Application.persistentDataPath + "/Config/Network.JSON");
        string json = reader.ReadToEnd();
        reader.Close();

        //MyClass myObjs = JsonUtility.FromJson<MyClass>(json);

        //SampleData mySampleFile = JsonUtility.FromJson<SampleData>(jsonStr);
        return json;//Convert for ease of use
                    //return myObjs.level.ToString();
    }









}
