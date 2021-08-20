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

    public static bool sendAdd(MMTDeclaration mmtDecl, out string uri)
    {
        string body = MMTSymbolDeclaration.ToJson(mmtDecl);
        return sendAdd(CommunicationEvents.ServerAdress + "/fact/add", body, out uri);
    }

    public static bool sendAdd(string path, string body, out string uri)
    {
        if (!CommunicationEvents.ServerRunning)
        {
            Debug.LogWarning("Server not running");
            uri = null;
            return false;
        }

        if(VerboseURI)
            Debug.Log("Sending to Server:\n" + body);

        //Put constructor parses stringbody to byteArray internally  (goofy workaround)
        UnityWebRequest www = UnityWebRequest.Put(path, body);
        www.method = UnityWebRequest.kHttpVerbPOST;
        www.SetRequestHeader("Content-Type", "application/json");
        www.timeout = 1;

        //TODO: implement real asynchronous communication ...
        AsyncOperation op = www.SendWebRequest();
        while (!op.isDone) ;

        if (www.result == UnityWebRequest.Result.ConnectionError
         || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogWarning(www.error);
            uri = null;
            return false;
        }
        else
        {
            string answer = www.downloadHandler.text;
            AddFactResponse res = JsonUtility.FromJson<AddFactResponse>(answer);

            if (VerboseURI)
                Debug.Log("Server added Fact:\n" + res.uri);

            uri = res.uri;
            return true;
        }
    }
}

[Serializable]
public abstract class Fact
{
    public GameObject Representation;

    public string Id { get { return _URI; } }
    protected string _URI;

    public string Label {
        get { // in case of renamed dependables
            return string.IsNullOrEmpty(_CustomLabel) ? 
                generateLabel() : 
                _CustomLabel;
        }
    }
    protected string _CustomLabel = null;
    private int LabelId = 0;

    protected FactOrganizer _Facts;

    private static int MaxLabelId = 0;
    private static SortedSet<int> UnusedLabelIds = new SortedSet<int>();

    protected Fact(FactOrganizer organizer)
    {
        this._Facts = organizer;
    }

    //TODO: notify about updated dependable Labelnames
    public void rename(string newLabel)
    {
        if (_Facts.ContainsLabel(newLabel))
            return;

        freeLabel();
        _CustomLabel = newLabel;
    }

    //If FactType depends on other Facts, e.g. AngleFacts depend on 3 PointFacts
    public abstract bool hasDependentFacts();

    public abstract string[] getDependentFactIds();

    public abstract GameObject instantiateDisplay(GameObject prefab, Transform transform);

    public static void Clear()
    {
        MaxLabelId = 0;
        UnusedLabelIds.Clear();
    }

    public virtual void delete(bool keep_clean = true)
    {
        //TODO: MMT: delete over there

        if (keep_clean)
            freeLabel();

        if (VerboseURI)
            Debug.Log("Server removed Fact:\n" + this.Id);
    }

    public abstract bool Equivalent(Fact f2);
    
    public abstract bool Equivalent(Fact f1, Fact f2);

    public abstract override int GetHashCode();

    // TODO? only get _Fact to freeLabel/
    public void freeLabel()
    {
        if (LabelId > 0)
        {
            UnusedLabelIds.Add(LabelId);
            // store Label for name-persistance
            LabelId = -LabelId;
        }

        if (!string.IsNullOrEmpty(_CustomLabel))
            _CustomLabel = null;
    }

    protected virtual string generateLabel()
    {
        if (LabelId == 0)
            if (UnusedLabelIds.Count == 0)
                LabelId = ++MaxLabelId;
            else
            {
                LabelId = UnusedLabelIds.Min;
                UnusedLabelIds.Remove(LabelId);
            }

        else if (LabelId < 0)
        // reload Label if possible
        {
            LabelId = -LabelId;
            UnusedLabelIds.Remove(LabelId);
        }

        return ((char)(64 + LabelId)).ToString();
    }
}

public abstract class FactWrappedCRTP<T>: Fact where T: FactWrappedCRTP<T>
{
    protected FactWrappedCRTP(FactOrganizer organizer) : base(organizer) { }

    public override bool Equivalent(Fact f2)
    {
        return Equivalent(this, f2);
    }

    public override bool Equivalent(Fact f1, Fact f2)
    {
        return f1.GetType() == f2.GetType() && EquivalentWrapped((T)f1, (T)f2);
    }

    protected abstract bool EquivalentWrapped(T f1, T f2);
}

public abstract class AbstractLineFact: FactWrappedCRTP<AbstractLineFact>
{
    //Id's of the 2 Point-Facts that are connected
    public string Pid1, Pid2;
    // normalized Direction from Pid1 to Pid2
    public Vector3 Dir;


    //only for temporary Use of LineFacts.
    //public AbstractLineFact() { }

    public AbstractLineFact(string pid1, string pid2, FactOrganizer organizer): base(organizer)
    {
        set_public_members(pid1, pid2);
    }

    public AbstractLineFact(string pid1, string pid2, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        set_public_members(pid1, pid2);
        this._URI = backendURI;
    }

    private void set_public_members(string pid1, string pid2)
    {
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        PointFact pf1 = _Facts[pid1] as PointFact;
        PointFact pf2 = _Facts[pid2] as PointFact;
        this.Dir = (pf2.Point - pf1.Point).normalized;
    }

    public override bool hasDependentFacts()
    {
        return true;
    }

    public override string[] getDependentFactIds()
    {
        return new string[] { Pid1, Pid2 };
    }

    public override int GetHashCode()
    {
        return this.Pid1.GetHashCode() ^ this.Pid2.GetHashCode();
    }
}

public abstract class AbstractLineFactWrappedCRTP<T>: AbstractLineFact where T: AbstractLineFactWrappedCRTP<T>
{
    //only for temporary Use of LineFacts.
    //public AbstractLineFactWrappedCRTP() { }

    public AbstractLineFactWrappedCRTP (string pid1, string pid2, FactOrganizer organizer) : base(pid1, pid2, organizer) { }

    public AbstractLineFactWrappedCRTP (string pid1, string pid2, string backendURI, FactOrganizer organizer) : base(pid1, pid2, backendURI, organizer) { }

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


    public PointFact(Vector3 P, Vector3 N, FactOrganizer organizer) : base(organizer)
    {
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
        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    public PointFact(float a, float b, float c, string uri, FactOrganizer organizer) : base(organizer)
    {
        this.Point = new Vector3(a, b, c);
        this.Normal = new Vector3(0, 1, 0);
        this._URI = uri;
    }

    public static PointFact parseFact(Scroll.ScrollFact fact) {
        String uri = fact.@ref.uri;
        OMA df = (OMA)((Scroll.ScrollSymbolFact)fact).df;
        if (df != null)
        {
            float a = (float)((OMF)df.arguments[0]).f;
            float b = (float)((OMF)df.arguments[1]).f;
            float c = (float)((OMF)df.arguments[2]).f;
            return new PointFact(a, b, c, uri, LevelFacts);
        }
        else {
            return null;
        }
    }

    public override Boolean hasDependentFacts() {
        return false;
    }

    public override string[] getDependentFactIds() {
        return new string[] { }; ;
    }

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = this.Label;
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
    public float Distance;

    public LineFact(string pid1, string pid2, string backendURI, FactOrganizer organizer) : base(pid1, pid2, backendURI, organizer)
    {
        SetDistance();
    }

    public LineFact(string pid1, string pid2, FactOrganizer organizer) : base(pid1, pid2, organizer)
    {
        SetDistance();

        PointFact pf1 = _Facts[pid1] as PointFact;
        PointFact pf2 = _Facts[pid2] as PointFact;

        string p1URI = pf1.Id;
        string p2URI = pf2.Id;
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
        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    public static LineFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        string pointAUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;
        string pointBUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[1]).uri;

        if (LevelFacts.ContainsKey(pointAUri)
         && LevelFacts.ContainsKey(pointBUri))
            return new LineFact(pointAUri, pointBUri, uri, LevelFacts);

        //If dependent facts do not exist return null
        else {
            return null;
        }
    }
    protected override string generateLabel()
    {
        return "[" + _Facts[Pid1].Label + _Facts[Pid2].Label + "]";
    }

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid1].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid2].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    protected override bool EquivalentWrapped(LineFact f1, LineFact f2)
    {
        if ((f1.Pid1 == f2.Pid1 && f1.Pid2 == f2.Pid2))// || 
            //(f1.Pid1 == f2.Pid2 && f1.Pid2 == f2.Pid1))
            return true;

        PointFact p1f1 = (PointFact)_Facts[f1.Pid1];
        PointFact p2f1 = (PointFact)_Facts[f1.Pid2];
        PointFact p1f2 = (PointFact)_Facts[f2.Pid1];
        PointFact p2f2 = (PointFact)_Facts[f2.Pid2];

        return (p1f1.Equivalent(p1f2) && p2f1.Equivalent(p2f2))
            ;//|| (p1f1.Equivalent(p2f2) && p2f1.Equivalent(p1f2));
    }

    private void SetDistance()
    {
        this.Distance = Vector3.Distance(((PointFact)_Facts[Pid1]).Point, ((PointFact)_Facts[Pid2]).Point);
    }
}

public class RayFact : AbstractLineFactWrappedCRTP<RayFact>
{
    public RayFact(string pid1, string pid2, string backendURI, FactOrganizer organizer) : base(pid1, pid2, backendURI, organizer) { }

    public RayFact(string pid1, string pid2, FactOrganizer organizer) : base(pid1, pid2, organizer)
    {
        PointFact pf1 = _Facts[pid1] as PointFact;
        PointFact pf2 = _Facts[pid2] as PointFact;

        string p1URI = pf1.Id;
        string p2URI = pf2.Id;

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
        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    public static RayFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        if ((OMA)((Scroll.ScrollSymbolFact)fact).df != null)
        {
            string pointAUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[0]).uri;
            string pointBUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[1]).uri;

            if (LevelFacts.ContainsKey(pointAUri)
             && LevelFacts.ContainsKey(pointBUri))
                return new RayFact(pointAUri, pointBUri, uri, LevelFacts);

            //If dependent facts do not exist return null
        }
        return null;
    }

    protected override string generateLabel()
    {
        return "–" + _Facts[Pid1].Label + _Facts[Pid2].Label + "–";
    }

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = this.Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    protected override bool EquivalentWrapped(RayFact f1, RayFact f2)
    {
        if (!Math3d.IsApproximatelyParallel(f1.Dir, f2.Dir))
            return false;

        PointFact p1f1 = (PointFact)_Facts[f1.Pid1];
        PointFact p1f2 = (PointFact)_Facts[f2.Pid1];
        PointFact p2f2 = (PointFact)_Facts[f2.Pid2];

        return Math3d.IsPointApproximatelyOnLine(p1f1.Point, f1.Dir, p1f2.Point)
            && Math3d.IsPointApproximatelyOnLine(p1f1.Point, f1.Dir, p2f2.Point);
    }
}

public class OnLineFact : FactWrappedCRTP<OnLineFact>
{
    //Id's of the Point and the Line it's on
    public string Pid, Rid;

    public OnLineFact(string pid, string rid, FactOrganizer organizer) : base(organizer)
    {
        this.Pid = pid;
        this.Rid = rid;

        PointFact pf = _Facts[pid] as PointFact;
        RayFact rf = _Facts[rid] as RayFact;
        string pURI = pf.Id;
        string rURI = rf.Id;

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
        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    public OnLineFact(string pid, string rid, string uri, FactOrganizer organizer) : base(organizer)
    {
        this.Pid = pid;
        this.Rid = rid;
        this._URI = uri;
    }

    public static OnLineFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        string lineUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0]).uri;
        string pointUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).uri;

        if (LevelFacts.ContainsKey(pointUri)
         && LevelFacts.ContainsKey(lineUri))
            return new OnLineFact(pointUri, lineUri, uri, LevelFacts);

        //If dependent facts do not exist return null
        else
            return null;
    }
    protected override string generateLabel()
    {
        return _Facts[Pid].Label + "∈" + _Facts[Rid].Label;
    }

    public override Boolean hasDependentFacts()
    {
        return true;
    }

    public override string[] getDependentFactIds()
    {
        return new string[] { Pid, Rid };
    }

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Rid].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    public override int GetHashCode()
    {
        return this.Pid.GetHashCode() ^ this.Rid.GetHashCode();
    }

    protected override bool EquivalentWrapped(OnLineFact f1, OnLineFact f2)
    {
        if (f1.Pid == f2.Pid && f1.Rid == f2.Rid)
            return true;

        PointFact pf1 = (PointFact)_Facts[f1.Pid];
        RayFact rf1 = (RayFact)_Facts[f1.Rid];
        PointFact pf2 = (PointFact)_Facts[f2.Pid];
        RayFact rf2 = (RayFact)_Facts[f2.Rid];

        return pf1.Equivalent(pf2) && rf1.Equivalent(rf2);
    }
}

public class AngleFact : FactWrappedCRTP<AngleFact>
{
    //Id's of the 3 Point-Facts, where Pid2 is the point, where the angle is
    public string Pid1, Pid2, Pid3;
    public bool is_right_angle;

    public AngleFact(string pid1, string pid2, string pid3, FactOrganizer organizer) : base(organizer)
    {
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        this.Pid3 = pid3;

        PointFact pf1 = _Facts[pid1] as PointFact;
        PointFact pf2 = _Facts[pid2] as PointFact;
        PointFact pf3 = _Facts[pid3] as PointFact;

        float v = GetAngle(); // sets is_right_angle

        MMTDeclaration mmtDecl;
        string p1URI = pf1.Id;
        string p2URI = pf2.Id;
        string p3URI = pf3.Id;
        if (is_right_angle)
            mmtDecl = generate90DegreeAngleDeclaration(v, p1URI, p2URI, p3URI);
        else
            mmtDecl = generateNot90DegreeAngleDeclaration(v, p1URI, p2URI, p3URI);

        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    public AngleFact(string Pid1, string Pid2, string Pid3, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Pid1 = Pid1;
        this.Pid2 = Pid2;
        this.Pid3 = Pid3;

        GetAngle();

        this._URI = backendURI;
    }

    public static AngleFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        string
            pointAUri,
            pointBUri,
            pointCUri;

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

        if (LevelFacts.ContainsKey(pointAUri)
         && LevelFacts.ContainsKey(pointBUri)
         && LevelFacts.ContainsKey(pointCUri))

            return new AngleFact(pointAUri, pointBUri, pointCUri, uri, LevelFacts);

        else    //If dependent facts do not exist return null
            return null;
    }

    protected override string generateLabel()
    {
        return (is_right_angle ? "⊾" : "∠") + _Facts[Pid1].Label + _Facts[Pid2].Label + _Facts[Pid3].Label;
    }

    private float GetAngle()
    {
        PointFact pf1 = _Facts[Pid1] as PointFact;
        PointFact pf2 = _Facts[Pid2] as PointFact;
        PointFact pf3 = _Facts[Pid3] as PointFact;

        float v = Vector3.Angle((pf1.Point - pf2.Point), (pf3.Point - pf2.Point));
        this.is_right_angle = Mathf.Abs(v - 90.0f) < 0.01;

        return is_right_angle ? 90f : v;
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
                new OMF(val) // 90f
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

    public override string[] getDependentFactIds()
    {
        return new string[] { Pid1, Pid2, Pid3 };
    }

    public override GameObject instantiateDisplay(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid1].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid2].Label;
        obj.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid3].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    public override int GetHashCode()
    {
        return this.Pid1.GetHashCode() ^ this.Pid2.GetHashCode() ^ this.Pid3.GetHashCode();
    }

    protected override bool EquivalentWrapped(AngleFact f1, AngleFact f2)
    {
        if ((f1.Pid1 == f2.Pid1 && f1.Pid2 == f2.Pid2 && f1.Pid3 == f2.Pid3))// || 
            //(f1.Pid1 == f2.Pid3 && f1.Pid2 == f2.Pid2 && f1.Pid3 == f2.Pid1))
            return true;

        PointFact p1f1 = (PointFact)_Facts[f1.Pid1];
        PointFact p2f1 = (PointFact)_Facts[f1.Pid2];
        PointFact p3f1 = (PointFact)_Facts[f1.Pid3];
        PointFact p1f2 = (PointFact)_Facts[f2.Pid1];
        PointFact p2f2 = (PointFact)_Facts[f2.Pid2];
        PointFact p3f2 = (PointFact)_Facts[f2.Pid3];

        return (p1f1.Equivalent(p1f2) && p2f1.Equivalent(p2f2) && p3f1.Equivalent(p3f2))
            ;//|| (p1f1.Equivalent(p3f2) && p2f1.Equivalent(p2f2) && p1f1.Equivalent(p3f2));
    }
}


