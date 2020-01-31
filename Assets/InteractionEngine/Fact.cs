using UnityEngine;
using UnityEngine.Networking;

public abstract class Fact
{
    public int Id;
    public GameObject Representation;
    public string backendURI;
    public string backendValueURI; // supposed to be null, for Facts without values eg. Points, OpenLines, OnLineFacts...
}

public class AddFactResponse
{
    //class to Read AddFact Responses.
    public string factUri;
    public string factValUri;

    public static AddFactResponse sendAdd(string path, string body) {
        Debug.Log(body);
        //Put constructor parses stringbody to byteArray internally  (goofy workaround)
        UnityWebRequest www = UnityWebRequest.Put(path, body);
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.SetRequestHeader("Content-Type", "application/json");
        
        AsyncOperation op = www.Send();
        while (!op.isDone) { }
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            return null;
        }
        else
        {
            string answer = www.downloadHandler.text;
            return JsonUtility.FromJson<AddFactResponse>(answer);
        }
    }
}

//I am not sure if we ever need to attach these to an object, so one script for all for now...
public class PointFact : Fact
{
    public Vector3 Point;
    public Vector3 Normal;

    public PointFact(int i,Vector3 P, Vector3 N) {
        this.Id = i;
        this.Point = P;
        this.Normal = N;

        string body = @"{ ""a"":" +P.x + @"," + @"""b"":" +P.y + @","+@"""c"":" + P.z + "}";
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/vector", body);
        this.backendURI = res.factUri;

    }

    public PointFact(int i, float a, float b, float c, string uri)
    {
        this.Id = i;
        this.Point = new Vector3(a,b,c);
        this.Normal = new Vector3(0,1,0);
        this.backendURI = uri;

    }

}


public class OpenLineFact : Fact
{
    //an infinite Line through the Points Pid1 and Pid2
    public int Pid1, Pid2;
}

public class LineFact : Fact
{
    //Id's of the 2 Point-Facts that are connected
    public int Pid1, Pid2;

    //only for temporary Use of LineFacts.
    public LineFact() { }

    public LineFact(int i, int pid1, int pid2) {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        PointFact pf1 = CommunicationEvents.Facts.Find((x => x.Id == pid1)) as PointFact;
        PointFact pf2 = CommunicationEvents.Facts.Find((x => x.Id == pid2)) as PointFact;
        string p1URI = pf1.backendURI;
        string p2URI = pf2.backendURI;
        double v = (pf1.Point - pf2.Point).magnitude;
        string body = @"{ ""pointA"":""" + p1URI + @"""," + @"""pointB"":""" + p2URI + @"""," + @"""value"":" + v + "}";
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/distance", body);
        this.backendURI = res.factUri;
        this.backendValueURI = res.factValUri;
    }

    public LineFact(int i, int pid1, int pid2, string uri, string valuri) {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        this.backendURI = uri;
        this.backendValueURI = valuri;
    }

    
}

public class AngleFact : Fact
{
    //Id's of the 3 Point-Facts, where Pid2 is the point, where the angle is
    public int Pid1, Pid2, Pid3;

    //only for temporary Use of AngleFacts
    public AngleFact() { }

    public AngleFact(int i, int pid1, int pid2, int pid3)
    {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        this.Pid3 = pid3;
        PointFact pf1 = CommunicationEvents.Facts.Find((x => x.Id == pid1)) as PointFact;
        PointFact pf2 = CommunicationEvents.Facts.Find((x => x.Id == pid2)) as PointFact;
        PointFact pf3 = CommunicationEvents.Facts.Find((x => x.Id == pid3)) as PointFact;
        double v = Vector3.Angle((pf1.Point - pf2.Point), (pf1.Point - pf2.Point));
        string body = @"{" +
          @"""left"":""" + pf1.backendURI + @"""," +
          @"""middle"":""" + pf2.backendURI + @"""," +
          @"""right"":""" + pf3.backendURI + @"""," +
          @"""value"":" + v +
          "}";
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/angle", body);
        this.backendURI = res.factUri;
        this.backendValueURI = res.factValUri;
    }

    public AngleFact(int i, int pid1, int pid2, int pid3, string uri, string valuri)
    {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        this.Pid3 = pid3;
        this.backendURI = uri;
        this.backendValueURI = valuri;
    }
}
public class OnLineFact : Fact
{
    //Id's of the Point , and the Id of the Line it sits on
    public int Pid1, Lid2;
}

