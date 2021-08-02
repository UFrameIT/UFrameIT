using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using static JSONManager;
using static CommunicationEvents;

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

public abstract class Fact
{
    public GameObject Representation;

    public string backendURI;
    public string Label;
    public int Id
    {
        get { return _id; }
        set
        {
            _id = value;
            Label = getLetter(_id);
        }
    }
    private int _id;


    public void rename(string newLabel)
    {
        this.Label = newLabel;
    }

    //If FactType depends on other Facts, e.g. AngleFacts depend on 3 PointFacts
    public abstract Boolean hasDependentFacts();

    public abstract int[] getDependentFactIds();

    public abstract GameObject instantiateDisplay(GameObject prefab, Transform transform);

    public virtual void delete()
    {
        //TODO: MMT
    }

    public abstract bool Equivalent(Fact f2);
    
    public abstract bool Equivalent(Fact f1, Fact f2);

    public abstract override int GetHashCode();

    public static string getLetter(int Id)
    {
        return ((Char)(64 + Id + 1)).ToString();
    }
}

public abstract class FactWrappedCRTP<T>: Fact where T: FactWrappedCRTP<T>
{
    public override bool Equivalent(Fact f2)
    {
        return Equivalent(this, f2);
    }

    public override bool Equivalent(Fact f1, Fact f2)
    {
        return EquivalentWrapped((T)f1, (T)f2);
    }

    protected abstract bool EquivalentWrapped(T f1, T f2);
}

public abstract class AbstractLineFact: FactWrappedCRTP<AbstractLineFact>
{
    //Id's of the 2 Point-Facts that are connected
    public int Pid1, Pid2;
    // normalized Direction from Pid1 to Pid2
    public Vector3 Dir;


    //only for temporary Use of LineFacts.
    public AbstractLineFact() { }

    //public AbstractLineFact(int i, int pid1, int pid2);

    public AbstractLineFact(int pid1, int pid2, string backendURI)
    {
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        PointFact pf1 = Facts[pid1] as PointFact;
        PointFact pf2 = Facts[pid2] as PointFact;
        this.Dir = (pf2.Point - pf1.Point).normalized;
        this.backendURI = backendURI;
    }

    public override bool hasDependentFacts()
    {
        return true;
    }

    public override int[] getDependentFactIds()
    {
        return new int[] { Pid1, Pid2 };
    }

    public override int GetHashCode()
    {
        return this.Pid1 ^ this.Pid2;
    }
}

public abstract class AbstractLineFactWrappedCRTP<T>: AbstractLineFact where T: AbstractLineFactWrappedCRTP<T>
{
    //only for temporary Use of LineFacts.
    public AbstractLineFactWrappedCRTP() { }

    public AbstractLineFactWrappedCRTP (int pid1, int pid2, string backendURI) : base(pid1, pid2, backendURI) { }

    protected override bool EquivalentWrapped(AbstractLineFact f1, AbstractLineFact f2)
    {
        return EquivalentWrapped((T)f1, (T)f2);
    }

    protected abstract bool EquivalentWrapped(T f1, T f2);
}


//I am not sure if we ever need to attach these to an object, so one script for all for now...
public class PointFact : FactWrappedCRTP<PointFact>
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
        return new int[] { }; ;
    }

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(this.Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }
    public override int GetHashCode()
    {
        return this.Point.GetHashCode() ^ this.Normal.GetHashCode();
    }

    protected override bool EquivalentWrapped(PointFact f1, PointFact f2)
    {
        return f1.Point == f2.Point;
    }

}

public class LineFact : AbstractLineFactWrappedCRTP<LineFact>
{
    public LineFact(int pid1, int pid2, string backendURI) : base(pid1, pid2, backendURI) { }

    public LineFact(int i, int pid1, int pid2)
    {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        PointFact pf1 = Facts[pid1] as PointFact;
        PointFact pf2 = Facts[pid2] as PointFact;
        this.Dir = (pf2.Point - pf1.Point).normalized;

        //Label is currently set to Fact.setId
        //Set Label to StringConcatenation of Points
        this.Label = pf1.Label + pf2.Label;

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

    public static LineFact parseFact(Scroll.ScrollFact fact)
    {
        String uri = fact.@ref.uri;
        String pointAUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;
        String pointBUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[1]).uri;

        if (Facts.searchURI(pointAUri, out int pid1)
         && Facts.searchURI(pointBUri, out int pid2))
            return new LineFact(pid1, pid2, uri);

        //If dependent facts do not exist return null
        else {
            return null;
        }
    }

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(Facts[this.Pid1].Id);
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(Facts[this.Pid2].Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    protected override bool EquivalentWrapped(LineFact f1, LineFact f2)
    {
        if ((f1.Pid1 == f2.Pid1 && f1.Pid2 == f2.Pid2))// || 
            //(f1.Pid1 == f2.Pid2 && f1.Pid2 == f2.Pid1))
            return true;

        PointFact p1f1 = (PointFact)Facts[f1.Pid1];
        PointFact p2f1 = (PointFact)Facts[f1.Pid2];
        PointFact p1f2 = (PointFact)Facts[f2.Pid1];
        PointFact p2f2 = (PointFact)Facts[f2.Pid2];

        return (p1f1.Equivalent(p1f2) && p2f1.Equivalent(p2f2))
            ;//|| (p1f1.Equivalent(p2f2) && p2f1.Equivalent(p1f2));
    }
}

public class RayFact : AbstractLineFactWrappedCRTP<RayFact>
{
    RayFact(int pid1, int pid2, string backendURI) : base(pid1, pid2, backendURI) { }

    public RayFact(int i, int pid1, int pid2)
    {
        this.Id = i;
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        PointFact pf1 = Facts[pid1] as PointFact;
        PointFact pf2 = Facts[pid2] as PointFact;
        this.Dir = (pf2.Point - pf1.Point).normalized;

        string p1URI = pf1.backendURI;
        string p2URI = pf2.backendURI;

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

    public static RayFact parseFact(Scroll.ScrollFact fact)
    {
        String uri = fact.@ref.uri;
        if ((OMA)((Scroll.ScrollSymbolFact)fact).df != null)
        {
            String pointAUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[0]).uri;
            String pointBUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[1]).uri;

            if (Facts.searchURI(pointAUri, out int pid1)
             && Facts.searchURI(pointBUri, out int pid2))
                return new RayFact(pid1, pid2, uri);

            //If dependent facts do not exist return null
        }
        return null;
    }

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(this.Id);
        //obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(Facts2[f.Pid2].Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    protected override bool EquivalentWrapped(RayFact f1, RayFact f2)
    {
        if (!Math3d.IsApproximatelyParallel(f1.Dir, f2.Dir))
            return false;

        PointFact p1f1 = (PointFact)Facts[f1.Pid1];
        PointFact p1f2 = (PointFact)Facts[f2.Pid1];
        PointFact p2f2 = (PointFact)Facts[f2.Pid2];

        return Math3d.IsPointApproximatelyOnLine(p1f1.Point, f1.Dir, p1f2.Point)
            && Math3d.IsPointApproximatelyOnLine(p1f1.Point, f1.Dir, p2f2.Point);
    }
}

public class OnLineFact : FactWrappedCRTP<OnLineFact>
{
    //Id's of the Point and the Line it's on
    public int Pid, Rid;

    public OnLineFact(int i, int pid, int rid)
    {
        this.Id = i;
        this.Pid = pid;
        this.Rid = rid;
        PointFact pf = Facts[pid] as PointFact;
        RayFact rf = Facts[rid] as RayFact;
        string pURI = pf.backendURI;
        string rURI = rf.backendURI;

        //Set Label to StringConcatenation of Points
        this.Label = pf.Label + " ∈ " + rf.Label;

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

        if (Facts.searchURI(lineUri, out int rid)
         && Facts.searchURI(pointUri, out int pid))
            return new OnLineFact(pid, rid, uri);

        //If dependent facts do not exist return null
        else
            return null;
    }

    public override Boolean hasDependentFacts()
    {
        return true;
    }

    public override int[] getDependentFactIds()
    {
        return new int[] { Pid, Rid };
    }

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(Facts[this.Pid].Id);
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(Facts[this.Rid].Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    public override int GetHashCode()
    {
        return this.Pid ^ this.Rid;
    }

    protected override bool EquivalentWrapped(OnLineFact f1, OnLineFact f2)
    {
        if (f1.Pid == f2.Pid && f1.Rid == f2.Rid)
            return true;

        PointFact pf1 = (PointFact)Facts[f1.Pid];
        RayFact rf1 = (RayFact)Facts[f1.Rid];
        PointFact pf2 = (PointFact)Facts[f2.Pid];
        RayFact rf2 = (RayFact)Facts[f2.Rid];

        return pf1.Equivalent(pf2) && rf1.Equivalent(rf2);
    }
}

public class AngleFact : FactWrappedCRTP<AngleFact>
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
        PointFact pf1 = Facts[pid1] as PointFact;
        PointFact pf2 = Facts[pid2] as PointFact;
        PointFact pf3 = Facts[pid3] as PointFact;

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
        String uri = fact.@ref.uri;
        String pointAUri;
        String pointBUri;
        String pointCUri;

        //If angle is not a 90Degree-Angle
        if (fact.GetType().Equals(typeof(Scroll.ScrollValueFact)))
        {
            pointAUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;
            pointBUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[1]).uri;
            pointCUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[2]).uri;
        }
        //If angle is a 90Degree-Angle
        else {
            pointAUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).arguments[0]).uri;
            pointBUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).arguments[1]).uri;
            pointCUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).arguments[2]).uri;
        }

        if (Facts.searchURI(pointAUri, out int pid1)
         && Facts.searchURI(pointBUri, out int pid2)
         && Facts.searchURI(pointCUri, out int pid3))

            return new AngleFact(pid1, pid2, pid3, uri);

        else    //If dependent facts do not exist return null
            return null;
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

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(Facts[this.Pid1].Id);
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(Facts[this.Pid2].Id);
        obj.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "" + getLetter(Facts[this.Pid3].Id);
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    public override int GetHashCode()
    {
        return this.Pid1 ^ this.Pid2 ^ this.Pid3;
    }

    protected override bool EquivalentWrapped(AngleFact f1, AngleFact f2)
    {
        if ((f1.Pid1 == f2.Pid1 && f1.Pid2 == f2.Pid2 && f1.Pid3 == f2.Pid3))// || 
            //(f1.Pid1 == f2.Pid3 && f1.Pid2 == f2.Pid2 && f1.Pid3 == f2.Pid1))
            return true;

        PointFact p1f1 = (PointFact)Facts[f1.Pid1];
        PointFact p2f1 = (PointFact)Facts[f1.Pid2];
        PointFact p3f1 = (PointFact)Facts[f1.Pid3];
        PointFact p1f2 = (PointFact)Facts[f2.Pid1];
        PointFact p2f2 = (PointFact)Facts[f2.Pid2];
        PointFact p3f2 = (PointFact)Facts[f2.Pid3];

        return (p1f1.Equivalent(p1f2) && p2f1.Equivalent(p2f2) && p3f1.Equivalent(p3f2))
            ;//|| (p1f1.Equivalent(p3f2) && p2f1.Equivalent(p2f2) && p1f1.Equivalent(p3f2));
    }
}


