using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using static JSONManager;

public class ParsingDictionary {

    public static Dictionary<string, Func<Scroll.ScrollFact, Fact>> parseFactDictionary = new Dictionary<string, Func<Scroll.ScrollFact, Fact>>() {
        {MMTURIs.Point, PointFact.parseFact},
        {MMTURIs.Metric, LineFact.parseFact},
        {MMTURIs.Angle, AngleFact.parseFact},
        {MMTURIs.LineType, RayFact.parseFact},
        {MMTURIs.OnLine, OnLineFact.parseFact},
        //90Degree-Angle
        {MMTURIs.Eq, AngleFact.parseFact}
    };

}

public abstract class Fact
{
    private int _id;
    public string Label;
    public int Id
    {
        get { return _id; }
        set
        {
           // if (_id == value) return;
            _id = value;
            Label= ((Char)(64 + _id + 1)).ToString();
        }
    }
    public GameObject Representation;
    public string backendURI;

    public string format(float t)
    {
        return t.ToString("0.0000").Replace(',', '.');
    }

    //If FactType depends on other Facts, e.g. AngleFacts depend on 3 PointFacts
    public abstract Boolean hasDependentFacts();

    public abstract int[] getDependentFactIds();

    public abstract GameObject instantiateRepresentation(GameObject prefab, Transform transform);

    public abstract override bool Equals(System.Object obj);

    public abstract override int GetHashCode();

    public static string getLetter(int Id)
    {
        return ((Char)(64 + Id + 1)).ToString();
    }
}

public abstract class DirectedFact : Fact
{
    public DirectedFact flippedFact;
}

public class AddFactResponse
{
    //class to Read AddFact Responses.
    // public string factUri;
    // public string factValUri;
    public string uri;

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
        AsyncOperation op = www.SendWebRequest();
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

        List<MMTTerm> arguments = new List<MMTTerm>
        {
            new OMF(P.x),
            new OMF(P.y),
            new OMF(P.z)
        };

        //OMS constructor generates full URI
        MMTTerm tp = new OMS(MMTURIs.Point);
        MMTTerm df = new OMA(new OMS(MMTURIs.Tuple), arguments);

        //TODO: rework fact list + labeling
        MMTSymbolDeclaration mmtDecl = new MMTSymbolDeclaration(this.Label, tp, df);
        string body = MMTSymbolDeclaration.ToJson(mmtDecl);

        AddFactResponse res = AddFactResponse.sendAdd(CommunicationEvents.ServerAdress+"/fact/add", body);
        this.backendURI = res.uri;
        Debug.Log(this.backendURI);
    }

    public PointFact(float a, float b, float c, string uri)
    {
        this.Point = new Vector3(a, b, c);
        this.Normal = new Vector3(0, 1, 0);
        this.backendURI = uri;
    }

    public static PointFact parseFact(Scroll.ScrollFact fact) {
        String uri = fact.@ref.uri;
        OMA df = (OMA)((Scroll.ScrollSymbolFact)fact).df;
        if (df != null)
        {
            float a = (float)((OMF)df.arguments[0]).f;
            float b = (float)((OMF)df.arguments[1]).f;
            float c = (float)((OMF)df.arguments[2]).f;
            return new PointFact(a, b, c, uri);
        }
        else {
            return null;
        }
    }

    public override Boolean hasDependentFacts() {
        return false;
    }

    public override int[] getDependentFactIds() {
        return null;
    }

    public override GameObject instantiateRepresentation(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(this.Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            PointFact p = (PointFact)obj;
            return this.Point.Equals(p.Point) && this.Normal.Equals(p.Normal);
        }
    }

    public override int GetHashCode()
    {
        return this.Point.GetHashCode() ^ this.Normal.GetHashCode();
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

        //Label is currently set to Fact.setId
        //Set Label to StringConcatenation of Points
        this.Label = "|" + pf1.Label + pf2.Label + "|";

        string p1URI = pf1.backendURI;
        string p2URI = pf2.backendURI;
        float v = (pf1.Point - pf2.Point).magnitude;

        MMTTerm lhs =
            new OMA(
                new OMS(MMTURIs.Metric),
                new List<MMTTerm> {
                    new OMS(p1URI),
                    new OMS(p2URI)
                }
            );

        MMTTerm valueTp = new OMS(MMTURIs.RealLit);
        MMTTerm value = new OMF(v);

        //see point label
        MMTValueDeclaration mmtDecl = new MMTValueDeclaration(this.Label, lhs, valueTp, value);
        string body = MMTDeclaration.ToJson(mmtDecl);
        AddFactResponse res = AddFactResponse.sendAdd(CommunicationEvents.ServerAdress + "/fact/add", body);
        this.backendURI = res.uri;
        Debug.Log(this.backendURI);
    }

    public LineFact(int Pid1, int Pid2, string backendURI)
    {
        this.Pid1 = Pid1;
        this.Pid2 = Pid2;
        this.backendURI = backendURI;
    }

    public static LineFact parseFact(Scroll.ScrollFact fact)
    {
        String uri = fact.@ref.uri;
        String pointAUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;
        String pointBUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[1]).uri;
        if (CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointAUri)) &&
            CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointBUri)))
        {
            int pid1 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointAUri)).Id;
            int pid2 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointBUri)).Id;
            return new LineFact(pid1, pid2, uri);
        }
        //If dependent facts do not exist return null
        else {
            return null;
        }
    }

    public override Boolean hasDependentFacts()
    {
        return true;
    }

    public override int[] getDependentFactIds()
    {
        return new int[] { Pid1, Pid2 };
    }

    public override GameObject instantiateRepresentation(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[this.Pid1].Id);
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[this.Pid2].Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            LineFact l = (LineFact)obj;
            return this.Pid1.Equals(l.Pid1) && this.Pid2.Equals(l.Pid2);
        }
    }

    public override int GetHashCode()
    {
        return this.Pid1 ^ this.Pid2;
    }
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

        //Set Label to StringConcatenation of Points
        this.Label = pf1.Label + pf2.Label;

        List<MMTTerm> arguments = new List<MMTTerm>
        {
            new OMS(p1URI),
            new OMS(p2URI)
        };

        //OMS constructor generates full URI
        MMTTerm tp = new OMS(MMTURIs.LineType);
        MMTTerm df = new OMA(new OMS(MMTURIs.LineOf), arguments);

        //TODO: rework fact list + labeling
        MMTSymbolDeclaration mmtDecl = new MMTSymbolDeclaration(this.Label, tp, df);
        string body = MMTSymbolDeclaration.ToJson(mmtDecl);

        AddFactResponse res = AddFactResponse.sendAdd(CommunicationEvents.ServerAdress + "/fact/add", body);
        this.backendURI = res.uri;
        Debug.Log(this.backendURI);
    }

    public RayFact(int pid1, int pid2, string uri)
    {
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        this.backendURI = uri;
    }

    public static RayFact parseFact(Scroll.ScrollFact fact)
    {
        String uri = fact.@ref.uri;
        if ((OMA)((Scroll.ScrollSymbolFact)fact).df != null)
        {
            String pointAUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[0]).uri;
            String pointBUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[1]).uri;
            if (CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointAUri)) &&
                CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointBUri)))
            {
                int pid1 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointAUri)).Id;
                int pid2 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointBUri)).Id;
                return new RayFact(pid1, pid2, uri);
            }
            //If dependent facts do not exist return null
            else
            {
                return null;
            }
        }
        else {
            return null;
        }
    }

    public override Boolean hasDependentFacts()
    {
        return true;
    }

    public override int[] getDependentFactIds()
    {
        return new int[] { Pid1, Pid2 };
    }

    public override GameObject instantiateRepresentation(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(this.Id);
        //obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[f.Pid2].Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            RayFact r = (RayFact)obj;
            return this.Pid1.Equals(r.Pid1) && this.Pid2.Equals(r.Pid2);
        }
    }

    public override int GetHashCode()
    {
        return this.Pid1 ^ this.Pid2;
    }
}


public class OnLineFact : Fact
{
    //Id's of the Point and the Line it's on
    public int Pid, Rid;

    public OnLineFact(int i, int pid, int rid)
    {
        this.Id = i;
        this.Pid = pid;
        this.Rid = rid;
        PointFact pf = CommunicationEvents.Facts.Find((x => x.Id == pid)) as PointFact;
        RayFact rf = CommunicationEvents.Facts.Find((x => x.Id == rid)) as RayFact;
        string pURI = pf.backendURI;
        string rURI = rf.backendURI;

        //Set Label to StringConcatenation of Points
        this.Label = pf.Label + " ∊ " + rf.Label;

        List<MMTTerm> innerArguments = new List<MMTTerm>
        {
            new OMS(rURI),
            new OMS(pURI)
        };

        List<MMTTerm> outerArguments = new List<MMTTerm>
        {
            new OMA(new OMS(MMTURIs.OnLine), innerArguments)
        };

        //OMS constructor generates full URI
        MMTTerm tp = new OMA(new OMS(MMTURIs.Ded), outerArguments);
        MMTTerm df = null;

        //TODO: rework fact list + labeling
        MMTSymbolDeclaration mmtDecl = new MMTSymbolDeclaration(this.Label, tp, df);
        string body = MMTSymbolDeclaration.ToJson(mmtDecl);

        AddFactResponse res = AddFactResponse.sendAdd(CommunicationEvents.ServerAdress + "/fact/add", body);
        this.backendURI = res.uri;
        Debug.Log(this.backendURI);
    }

    public OnLineFact(int pid, int rid, string uri) {
        this.Pid = pid;
        this.Rid = rid;
        this.backendURI = uri;
    }

    public static OnLineFact parseFact(Scroll.ScrollFact fact)
    {
        String uri = fact.@ref.uri;
        String lineUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0]).uri;
        String pointUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).uri;
        if (CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(lineUri)) &&
            CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointUri)))
        {
            int pid = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointUri)).Id;
            int rid = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(lineUri)).Id;
            return new OnLineFact(pid, rid, uri);
        }
        //If dependent facts do not exist return null
        else
        {
            return null;
        }
    }

    public override Boolean hasDependentFacts()
    {
        return true;
    }

    public override int[] getDependentFactIds()
    {
        return new int[] { Pid, Rid };
    }

    public override GameObject instantiateRepresentation(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[this.Pid].Id);
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[this.Rid].Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            OnLineFact o = (OnLineFact)obj;
            return this.Pid.Equals(o.Pid) && this.Rid.Equals(o.Rid);
        }
    }

    public override int GetHashCode()
    {
        return this.Pid ^ this.Rid;
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

        string p1URI = pf1.backendURI;
        string p2URI = pf2.backendURI;
        string p3URI = pf3.backendURI;
        float v = Vector3.Angle((pf1.Point - pf2.Point), (pf3.Point - pf2.Point));

        MMTDeclaration mmtDecl;

        if (Mathf.Abs(v - 90.0f) < 0.01)
        {
            v = 90.0f;
            //Label is currently set to Fact.setId
            //Set Label to StringConcatenation of Points
            this.Label = "⊾" + pf1.Label + pf2.Label + pf3.Label;
            mmtDecl = generate90DegreeAngleDeclaration(v, p1URI, p2URI, p3URI);
        }
        else
        {
            //Label is currently set to Fact.setId
            //Set Label to StringConcatenation of Points
            this.Label = "∠" + pf1.Label + pf2.Label + pf3.Label;
            mmtDecl = generateNot90DegreeAngleDeclaration(v, p1URI, p2URI, p3URI);
        }

        Debug.Log("angle: " + v);

        string body = MMTDeclaration.ToJson(mmtDecl);

        Debug.Log(body);
        AddFactResponse res = AddFactResponse.sendAdd(CommunicationEvents.ServerAdress+"/fact/add", body);
        this.backendURI = res.uri;
        Debug.Log(this.backendURI);
    }

    public AngleFact(int Pid1, int Pid2, int Pid3, string backendURI)
    {
        this.Pid1 = Pid1;
        this.Pid2 = Pid2;
        this.Pid3 = Pid3;
        this.backendURI = backendURI;
    }

    public static AngleFact parseFact(Scroll.ScrollFact fact)
    {
        String uri;
        String pointAUri;
        String pointBUri;
        String pointCUri;
        int pid1;
        int pid2;
        int pid3;

        //If angle is not a 90Degree-Angle
        if (fact.GetType().Equals(typeof(Scroll.ScrollValueFact)))
        {
            uri = fact.@ref.uri;
            pointAUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;
            pointBUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[1]).uri;
            pointCUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[2]).uri;
            //If dependent facts do not exist return null
            if (!CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointAUri)) |
                !CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointBUri)) |
                !CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointCUri)))
            {
                return null;
            }

            pid1 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointAUri)).Id;
            pid2 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointBUri)).Id;
            pid3 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointCUri)).Id;
        }
        //If angle is a 90Degree-Angle
        else {
            uri = fact.@ref.uri;
            pointAUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).arguments[0]).uri;
            pointBUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).arguments[1]).uri;
            pointCUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).arguments[2]).uri;
            //If dependent facts do not exist return null
            if (!CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointAUri)) |
                !CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointBUri)) |
                !CommunicationEvents.Facts.Exists(x => x.backendURI.Equals(pointCUri)))
            {
                return null;
            }

            pid1 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointAUri)).Id;
            pid2 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointBUri)).Id;
            pid3 = CommunicationEvents.Facts.Find(x => x.backendURI.Equals(pointCUri)).Id;
        }

        return new AngleFact(pid1, pid2, pid3, uri);
    }

    private MMTDeclaration generate90DegreeAngleDeclaration(float val, string p1URI, string p2URI, string p3URI) {

        MMTTerm argument = new OMA(
            new OMS(MMTURIs.Eq),
            new List<MMTTerm> {
                new OMS(MMTURIs.RealLit),
                new OMA(
                    new OMS(MMTURIs.Angle),
                    new List<MMTTerm> {
                        new OMS(p1URI),
                        new OMS(p2URI),
                        new OMS(p3URI)
                    }
                ),
                new OMF(val)
            }
        );
        
        MMTTerm tp = new OMA(new OMS(MMTURIs.Ded), new List<MMTTerm> {argument});
        MMTTerm df = null;

        return new MMTSymbolDeclaration(this.Label, tp, df);
    }

    private MMTDeclaration generateNot90DegreeAngleDeclaration(float val, string p1URI, string p2URI, string p3URI)
    {
        MMTTerm lhs =
            new OMA(
                new OMS(MMTURIs.Angle),
                new List<MMTTerm> {
                    new OMS(p1URI),
                    new OMS(p2URI),
                    new OMS(p3URI)
                }
            );

        MMTTerm valueTp = new OMS(MMTURIs.RealLit);
        MMTTerm value = new OMF(val);
        
        return new MMTValueDeclaration(this.Label, lhs, valueTp, value);
    }

    public override Boolean hasDependentFacts()
    {
        return true;
    }

    public override int[] getDependentFactIds()
    {
        return new int[] { Pid1, Pid2, Pid3 };
    }

    public override GameObject instantiateRepresentation(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[this.Pid1].Id);
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[this.Pid2].Id);
        obj.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(CommunicationEvents.Facts[this.Pid3].Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            AngleFact a = (AngleFact)obj;
            return this.Pid1.Equals(a.Pid1) && this.Pid2.Equals(a.Pid2) && this.Pid3.Equals(a.Pid3);
        }
    }

    public override int GetHashCode()
    {
        return this.Pid1 ^ this.Pid2 ^ this.Pid3;
    }
}


