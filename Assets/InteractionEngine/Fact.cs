using UnityEngine;
using UnityEngine.Networking;

public abstract class Fact
{
    public int Id;
    public GameObject Representation;
    public string backendURI;
    public string backendValueURI; // supposed to be null, for Facts without values eg. Points, OpenLines, OnLineFacts...

    public string format(float t)
    {
        return t.ToString("0.0000").Replace(',', '.');
    }
}

public abstract class DirectedFact : Fact
{
    public DirectedFact flippedFact;
}

public class AddFactResponse
{
    //class to Read AddFact Responses.
    public string factUri;
    public string factValUri;

    public static AddFactResponse sendAdd(string path, string body)
    {
        if (!CommunicationEvents.ServerRunning)
        {
            Debug.LogWarning("Server not running");
            return new AddFactResponse();
        }
        Debug.Log(body);
        //Put constructor parses stringbody to byteArray internally  (goofy workaround)
        UnityWebRequest www = UnityWebRequest.Put(path, body);
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.SetRequestHeader("Content-Type", "application/json");
        www.timeout = 1;
        //TODO: implement real asynchronous communication ...
        AsyncOperation op = www.Send();
        while (!op.isDone) { }
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogWarning(www.error);
            return new AddFactResponse();
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

    public PointFact(int i, Vector3 P, Vector3 N)
    {
        this.Id = i;
        this.Point = P;
        this.Normal = N;

        string body = @"{ ""a"":" + format(P.x) + @"," + @"""b"":" + format(P.y) + @"," + @"""c"":" + format(P.y) + "}";
        Debug.Log(body);
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/vector", body);
        this.backendURI = res.factUri;

    }

    public PointFact(int i, float a, float b, float c, string uri)
    {
        this.Id = i;
        this.Point = new Vector3(a, b, c);
        this.Normal = new Vector3(0, 1, 0);
        this.backendURI = uri;

    }

}




public class LineFact : DirectedFact
{
    //Id's of the 2 Point-Facts that are connected
    public int Pid1, Pid2;

    //only for temporary Use of LineFacts.
    public LineFact() { }

    public LineFact(int i, int pid1, int pid2)
    {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        PointFact pf1 = CommunicationEvents.Facts.Find((x => x.Id == pid1)) as PointFact;
        PointFact pf2 = CommunicationEvents.Facts.Find((x => x.Id == pid2)) as PointFact;
        string p1URI = pf1.backendURI;
        string p2URI = pf2.backendURI;
        float v = (pf1.Point - pf2.Point).magnitude;
        string body = @"{ ""pointA"":""" + p1URI + @"""," + @"""pointB"":""" + p2URI + @"""," + @"""value"":" + format(v) + "}";
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/distance", body);
        this.backendURI = res.factUri;
        this.backendValueURI = res.factValUri;
        this.flippedFact = new LineFact(pid2, pid1);
    }

    // to create flipped fact
    public LineFact(int pid1, int pid2)
    {
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        PointFact pf1 = CommunicationEvents.Facts.Find((x => x.Id == pid1)) as PointFact;
        PointFact pf2 = CommunicationEvents.Facts.Find((x => x.Id == pid2)) as PointFact;
        string p1URI = pf1.backendURI;
        string p2URI = pf2.backendURI;
        float v = (pf1.Point - pf2.Point).magnitude;
        string body = @"{ ""pointA"":""" + p1URI + @"""," + @"""pointB"":""" + p2URI + @"""," + @"""value"":" + format(v) + "}";
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/distance", body);
        this.backendURI = res.factUri;
        this.backendValueURI = res.factValUri;
    }

    //pushout return
    public LineFact(int i, int pid1, int pid2, string uri, string valuri)
    {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        this.backendURI = uri;
        this.backendValueURI = valuri;
        this.flippedFact = new LineFact(pid2, pid1);

    }


}

public class OpenLineFact : Fact
{
    //R: this is called RayFact for now (see below), feel free to change
    //an infinite Line through the Points Pid1 and Pid2
    public int Pid1, Pid2;
}

public class RayFact : Fact
{
    //Id's of the 2 Point-Facts that are connected
    public int Pid1, Pid2;

    //only for temporary Use of LineFacts.
    public RayFact() { }

    public RayFact(int i, int pid1, int pid2)
    {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        PointFact pf1 = CommunicationEvents.Facts.Find((x => x.Id == pid1)) as PointFact;
        PointFact pf2 = CommunicationEvents.Facts.Find((x => x.Id == pid2)) as PointFact;
        string p1URI = pf1.backendURI;
        string p2URI = pf2.backendURI;
        //TODO: fix body
        string body = @"{ ""base"":""" + p1URI + @"""," + @"""second"":""" + p2URI + @"""" + "}";
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/line", body);
        this.backendURI = res.factUri;
        this.backendValueURI = res.factValUri;
    }

    public RayFact(int i, int pid1, int pid2, string uri, string valuri)
    {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        this.backendURI = uri;
        this.backendValueURI = valuri;
    }


}


public class OnLineFact : Fact
{
    //Id's of the Point , and the Id of the Line it sits on
    public int Pid, Lid;

    public OnLineFact(int i, int pid, int lid)
    {
        this.Id = i;
        this.Pid = pid;
        this.Lid = lid;
        PointFact pf = CommunicationEvents.Facts.Find((x => x.Id == pid)) as PointFact;
        RayFact lf = CommunicationEvents.Facts.Find((x => x.Id == lid)) as RayFact;
        string pURI = pf.backendURI;
        string lURI = lf.backendURI;
        string body = @"{ ""vector"":""" + pURI + @"""," + @"""line"":""" + lURI + @"""" + "}";
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/onLine", body);
        this.backendURI = res.factUri;
        this.backendValueURI = res.factValUri;
        Debug.Log("created onLine" + this.backendURI + " " + this.backendValueURI);
    }


}


public class AngleFact : DirectedFact
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

        float v = Vector3.Angle((pf1.Point - pf2.Point), (pf3.Point - pf2.Point));
        if (Mathf.Abs(v - 90.0f) < 0.01) v = 90.0f;
        Debug.Log("angle: " + v);
        string body = @"{" +
          @"""left"":""" + pf1.backendURI + @"""," +
          @"""middle"":""" + pf2.backendURI + @"""," +
          @"""right"":""" + pf3.backendURI + @"""," +
          @"""value"":" + format(v) +
          "}";
        Debug.Log(body);
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/angle", body);
        this.backendURI = res.factUri;
        this.backendValueURI = res.factValUri;
        this.flippedFact = new AngleFact(pid3, pid2, pid1);
    }

    //to create flipped fact
    public AngleFact(int pid1, int pid2, int pid3)
    {
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        this.Pid3 = pid3;
        PointFact pf1 = CommunicationEvents.Facts.Find((x => x.Id == pid1)) as PointFact;
        PointFact pf2 = CommunicationEvents.Facts.Find((x => x.Id == pid2)) as PointFact;
        PointFact pf3 = CommunicationEvents.Facts.Find((x => x.Id == pid3)) as PointFact;

        float v = Vector3.Angle((pf1.Point - pf2.Point), (pf3.Point - pf2.Point));
        if (Mathf.Abs(v - 90.0f) < 0.01) v = 90.0f;
        Debug.Log("angle: " + v);
        string body = @"{" +
          @"""left"":""" + pf1.backendURI + @"""," +
          @"""middle"":""" + pf2.backendURI + @"""," +
          @"""right"":""" + pf3.backendURI + @"""," +
          @"""value"":" + format(v) +
          "}";
        Debug.Log(body);
        AddFactResponse res = AddFactResponse.sendAdd("localhost:8081/fact/add/angle", body);
        this.backendURI = res.factUri;
        this.backendValueURI = res.factValUri;
    }

    //pushout return
    public AngleFact(int i, int pid1, int pid2, int pid3, string uri, string valuri)
    {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        this.Pid3 = pid3;
        this.backendURI = uri;
        this.backendValueURI = valuri;
        this.flippedFact = new AngleFact(pid3, pid2, pid1);
    }
}


