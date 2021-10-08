using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using static JSONManager;
using static CommunicationEvents;


public class ParsingDictionary {
    //TODO? get rid of this, use reflection? instead, if possible
    //TODO: docu
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

/// <summary>
/// class to Read AddFact Responses.
/// </summary>
// TODO: docu
public class AddFactResponse
{
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

/// <summary>
/// %Fact representation of Unity; mostly mirrors Facts of MMT.
/// </summary>
public abstract class Fact
{
    /// <summary>
    /// Reference to <c>GameObject</c> that represents this Fact in the GameWorld.
    /// </summary>
    /// <seealso cref="FactObject"/>
    [JsonIgnore]
    public GameObject Representation;

    /// <value>
    /// Unique Id. e.g.: MMT URI
    /// </value>
    public string Id { 
        get { return _URI; }
        set { if (_URI == null) _URI = value; } // needed for JSON
    }

    /// <summary>
    /// MMT URI
    /// </summary>
    protected string _URI;

    /// <value>
    /// <c>get</c> initiates and subsequently updates a human readable name. <remarks>Should be called once a constructor call to be initiated.</remarks>
    /// <c>set</c> calls <see cref="rename(string)"/>
    /// </value>
    public string Label {
        get { // in case of renamed dependables
            return hasCustomLabel && _CustomLabel != null ?
                _CustomLabel :
                generateLabel();
        }
        set { rename(value); }
    }

    /// <value>
    /// Is true if Fact has a custom <see cref="Label"/> which is not <c>null</c> or <c>""</c>.
    /// </value>
    public bool hasCustomLabel {
        get { return LabelId < 0; }
    }
    /// <summary>
    /// Stores custom <see cref="Label"/> if set.
    /// </summary>
    protected string _CustomLabel = null;

    /// <summary>
    /// Counter to organize auto generated <see cref="Label"/>.
    /// Set to negative, if custom \ref Label is assigned.
    /// </summary>
    // property for JSON to set AFTER Label => declare AFTER Label
    public int LabelId { get; set; }

    /// <summary>
    /// Reference to <see cref="FactOrganizer"/> in which this Fact and all its <see cref="getDependentFactIds">depending Facts</see> are beeing organized.
    /// </summary>
    protected FactOrganizer _Facts;

    /// <summary>
    /// Only being used by [JsonReader](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonReader.htm) to initiate empty \ref Fact "Facts".
    /// <seealso cref="JSONManager"/>
    /// </summary>
    protected Fact()
    {
        this._Facts = new FactOrganizer();
        LabelId = 0;
    }

    /// <summary>
    /// Standard base-constructor.
    /// </summary>
    /// <param name="organizer"><see cref="_Facts"/></param>
    protected Fact(FactOrganizer organizer)
    {
        this._Facts = organizer;
        LabelId = 0;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="organizer"><see cref="_Facts"/></param>
    protected Fact(Fact fact, FactOrganizer organizer)
    {
        this._Facts = organizer;
        LabelId = fact.LabelId;

        if (hasCustomLabel)
            _CustomLabel = fact.Label;
    }

    /// <summary>
    /// Assignes a custom <see cref="Label"/>, if <paramref name="newLabel"/> is not yet taken;
    /// or clears custom <see cref="Label"/>.
    /// </summary>
    /// <param name="newLabel">To be new <see cref="Label"/>. To reset to auto-generation set to <c>null</c> or <c>""</c>.</param>
    /// <returns></returns>

    //TODO: notify about updated dependable Labelnames for UI
    //TODO: check for colissions with not yet generated names
    public bool rename(string newLabel)
    // returns true if succeded
    {
        if (string.IsNullOrEmpty(newLabel))
        // switch back to autogenerated
        {
            generateLabel();
            _CustomLabel = null;
            return true;
        }
        else
        // set CustomLabel if available
        {
            if (_Facts.ContainsLabel(newLabel))
                return false;

            freeAutoLabel();
            _CustomLabel = newLabel;

            return true;
        }
    }

    /// <returns><see langword="true"/> if Fact depends on other \ref Fact "Facts"; equivalent to <see cref="getDependentFactIds"/> returns non empty array</returns>
    public abstract bool hasDependentFacts();

    /// <returns> array of Fact <see cref="Id"> Ids </see> on which this Fact depends.</returns>
    /// <example><see cref="AngleFact"/> needs 3 <see cref="PointFact"/>s to be defined.</example>
    public abstract string[] getDependentFactIds();

    /// <summary>
    /// Initiates a <paramref name="prefab"/> at <paramref name="transform"/> e.g. by setting <see cref="Label"/>.
    /// </summary>
    /// <remarks>Does not set <see cref="Representation"/>.</remarks>
    /// <param name="prefab"><c>GameObject</c> Prefab that will represent this Fact</param>
    /// <param name="transform"><c>Transform</c> where to initiate <paramref name="prefab"/></param>
    /// <returns></returns>

    // TODO: set Representation here instead of ...
    public abstract GameObject instantiateDisplay(GameObject prefab, Transform transform);

    /// <summary>
    /// Frees ressources e.g. <see cref="Label"/> and will eventually delete %Fact Server-Side in far-near future when feature is supported.
    /// </summary>
    /// <param name="keep_clean">when set to <c>true</c> will upkeep <see cref="Label"/> organization.</param>

    // TODO? replace by ~Fact() { }
    public virtual void delete(bool keep_clean = true)
    {
        //TODO: MMT: delete over there

        if (keep_clean)
            freeAutoLabel();

        if (VerboseURI)
            Debug.Log("Server removed Fact:\n" + this.Id);
    }

    /// <summary>
    /// Compares \ref Fact "this" against <paramref name="f2"/>.
    /// </summary>
    /// <param name="f2">Fact to compare to</param>
    /// <returns><c>true</c> if <paramref name="f2"/> is semantical very similar to \ref Fact "this"</returns>
    public abstract bool Equivalent(Fact f2);

    /// <summary>
    /// Compares <paramref name="f1"/> against <paramref name="f2"/>.
    /// </summary>
    /// <param name="f1">Fact to compare to</param>
    /// <param name="f2">Fact to compare to</param>
    /// <returns><c>true</c> if <paramref name="f2"/> is semantical very similar to <paramref name="f1"/></returns>
    public abstract bool Equivalent(Fact f1, Fact f2);

    /// <summary>
    /// canonical
    /// </summary>
    /// <returns>unique-ish Hash</returns>
    public abstract override int GetHashCode();

    /// <summary>
    /// auto-generates <see cref="Label"/> using generation variable(s) e.g. <see cref="LabelId"/>;
    /// if custom <see cref="Label"/> is set, tries to restore original generated <see cref="Label"/> **without** resetting <see cref="_CustomLabel"/>. If original <see cref="Label"/> is already taken, a new one will be generated.
    /// </summary>
    /// <returns>auto-generated <see cref="Label"/></returns>
    protected virtual string generateLabel()
    {
        if (LabelId < 0)
        // reload Label if possible
            LabelId = _Facts.UnusedLabelIds.Remove(-LabelId) ? -LabelId : 0;

        if (LabelId == 0)
            if (_Facts.UnusedLabelIds.Count == 0)
                LabelId = ++_Facts.MaxLabelId;
            else
            {
                LabelId = _Facts.UnusedLabelIds.Min;
                _Facts.UnusedLabelIds.Remove(LabelId);
            }

        return ((char)(64 + LabelId)).ToString();
    }

    /// <summary>
    /// Parses <see cref="Scroll.ScrollFact"/> to actual Fact
    /// </summary>
    /// <param name="fact">instance to be parsed</param>
    /// <returns>parsed Fact</returns>
    public static Fact parseFact(Scroll.ScrollFact fact)
    {
        return null;
    }

    /// <summary>
    /// Tells <see cref="_Facts"/> that \ref Fact "this" no longer uses auto-generated <see cref="Label"/>, but remembers current generation variable(s).
    /// </summary>

    // TODO? only get _Fact to freeLabel/
    public /*protected internal*/ void freeAutoLabel()
    {
        if (LabelId > 0)
        {
            _Facts.UnusedLabelIds.Add(LabelId);
            // store Label for name-persistance
            LabelId = -LabelId;
        }
    }
}

/// <summary>
/// Implements CRTP for <see cref="Fact"/>; Escalates constructors;
/// </summary>
/// <typeparam name="T">class, which inherits from FactWrappedCRTP</typeparam>
public abstract class FactWrappedCRTP<T>: Fact where T: FactWrappedCRTP<T>
{
    /// <summary>\copydoc Fact.Fact()</summary>
    protected FactWrappedCRTP() : base() { }

    /// <summary>\copydoc Fact.Fact(FactOrganizer)</summary>
    protected FactWrappedCRTP(FactOrganizer organizer) : base(organizer) { }

    /// <summary>\copydoc Fact.Fact(Fact, FactOrganizer)</summary>
    protected FactWrappedCRTP(FactWrappedCRTP<T> fact, FactOrganizer organizer) : base(fact, organizer) { }

    /// \copydoc Fact.Equivalent(Fact)
    public override bool Equivalent(Fact f2)
    {
        return Equivalent(this, f2);
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    public override bool Equivalent(Fact f1, Fact f2)
    {
        return f1.GetType() == f2.GetType() && EquivalentWrapped((T)f1, (T)f2);
    }

    /// <summary>CRTP step of <see cref="Equivalent(Fact)"/> and <see cref="Equivalent(Fact, Fact)"/></summary>
    protected abstract bool EquivalentWrapped(T f1, T f2);
}

/// <summary>
/// Base-class for 1D-Facts
/// </summary>
public abstract class AbstractLineFact: FactWrappedCRTP<AbstractLineFact>
{
    /// @{ <summary>
    /// One <see cref="Fact.Id">Id</see> of two <see cref="PointFact"/> defining <see cref="Dir"/>.
    /// </summary>
    public string Pid1, Pid2;
    /// @}

    /// <summary>
    /// Normalized Direction from <see cref="Pid1"/> to <see cref="Pid2"/>.
    /// </summary>
    public Vector3 Dir;

    /// <summary>
    /// \copydoc Fact.Fact()
    /// </summary>
    protected AbstractLineFact() : base()
    {
        Pid1 = null;
        Pid2 = null;
        Dir = Vector3.zero;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    protected AbstractLineFact(AbstractLineFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        set_public_members(old_to_new[fact.Pid1], old_to_new[fact.Pid2]);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="pid1">sets <see cref="AbstractLineFact.Pid1"/></param>
    /// <param name="pid2">sets <see cref="AbstractLineFact.Pid2"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    protected AbstractLineFact(string pid1, string pid2, FactOrganizer organizer): base(organizer)
    {
        set_public_members(pid1, pid2);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="pid2">sets <see cref="Pid2"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    protected AbstractLineFact(string pid1, string pid2, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        set_public_members(pid1, pid2);
        this._URI = backendURI;
    }

    /// <summary>
    /// Initiates <see cref="Pid1"/>, <see cref="Pid2"/>, <see cref="Dir"/>
    /// </summary>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="pid2">sets <see cref="Pid2"/></param>
    private void set_public_members(string pid1, string pid2)
    {
        this.Pid1 = pid1;
        this.Pid2 = pid2;
        PointFact pf1 = _Facts[pid1] as PointFact;
        PointFact pf2 = _Facts[pid2] as PointFact;
        this.Dir = (pf2.Point - pf1.Point).normalized;
    }

    /// \copydoc Fact.hasDependentFacts
    public override bool hasDependentFacts()
    {
        return true;
    }

    /// \copydoc Fact.getDependentFactIds
    public override string[] getDependentFactIds()
    {
        return new string[] { Pid1, Pid2 };
    }

    /// \copydoc Fact.GetHashCode
    public override int GetHashCode()
    {
        return this.Pid1.GetHashCode() ^ this.Pid2.GetHashCode();
    }
}

/// <summary>
/// Implements CRTP for <see cref="AbstractLineFact"/>; Escalates constructors;
/// </summary>
/// <typeparam name="T">class, which inherits from AbstractLineFactWrappedCRTP</typeparam>
public abstract class AbstractLineFactWrappedCRTP<T>: AbstractLineFact where T: AbstractLineFactWrappedCRTP<T>
{
    /// <summary>\copydoc Fact.Fact</summary>
    protected AbstractLineFactWrappedCRTP () : base() { }

    /// <summary>\copydoc AbstractLineFact.AbstractLineFact(AbstractLineFact, Dictionary{string, string}, FactOrganizer)</summary>
    protected AbstractLineFactWrappedCRTP (AbstractLineFactWrappedCRTP<T> fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, old_to_new, organizer) { }

    /// <summary>\copydoc AbstractLineFact.AbstractLineFact(string, string, FactOrganizer)</summary>
    protected AbstractLineFactWrappedCRTP (string pid1, string pid2, FactOrganizer organizer) : base(pid1, pid2, organizer) { }

    /// <summary>\copydoc AbstractLineFact.AbstractLineFact(string, string, string, FactOrganizer)</summary>
    protected AbstractLineFactWrappedCRTP (string pid1, string pid2, string backendURI, FactOrganizer organizer) : base(pid1, pid2, backendURI, organizer) { }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(AbstractLineFact f1, AbstractLineFact f2)
    {
        return EquivalentWrapped((T)f1, (T)f2);
    }

    /// <summary>CRTP step of <see cref="EquivalentWrapped(AbstractLineFact, AbstractLineFact)"/></summary>
    protected abstract bool EquivalentWrapped(T f1, T f2);
}

/// <summary>
/// Point in 3D Space
/// </summary>
public class PointFact : FactWrappedCRTP<PointFact>
{
    /// <summary> Position </summary>
    public Vector3 Point;
    /// <summary> Orientation for <see cref="Fact.Representation"/> </summary>
    public Vector3 Normal;


    /// <summary> \copydoc Fact.Fact </summary>
    public PointFact() : base()
    {
        this.Point = Vector3.zero;
        this.Normal = Vector3.zero;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public PointFact(PointFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(fact.Point, fact.Normal);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="P">sets <see cref="Point"/></param>
    /// <param name="N">sets <see cref="Normal"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public PointFact(Vector3 P, Vector3 N, FactOrganizer organizer) : base(organizer)
    {
        init(P, N);
    }

    /// <summary>
    /// Initiates <see cref="Point"/>, <see cref="Normal"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="P">sets <see cref="Point"/></param>
    /// <param name="N">sets <see cref="Normal"/></param>
    private void init(Vector3 P, Vector3 N)
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

        MMTSymbolDeclaration mmtDecl = new MMTSymbolDeclaration(this.Label, tp, df);
        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// <see cref="Normal"/> set to <c>Vector3.up</c>
    /// </summary>
    /// <param name="a">sets <c>x</c> coordinate of <see cref="Point"/></param>
    /// <param name="b">sets <c>y</c> coordinate of <see cref="Point"/></param>
    /// <param name="c">sets <c>z</c> coordinate of <see cref="Point"/></param>
    /// <param name="uri">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public PointFact(float a, float b, float c, string uri, FactOrganizer organizer) : base(organizer)
    {
        this.Point = new Vector3(a, b, c);
        this.Normal = Vector3.up;
        this._URI = uri;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static PointFact parseFact(Scroll.ScrollFact fact) {
        String uri = fact.@ref.uri;
        OMA df = (OMA)((Scroll.ScrollSymbolFact)fact).df;
        if (df != null)
        {
            float a = (float)((OMF)df.arguments[0]).f;
            float b = (float)((OMF)df.arguments[1]).f;
            float c = (float)((OMF)df.arguments[2]).f;
            return new PointFact(a, b, c, uri, StageStatic.stage.factState);
        }
        else {
            return null;
        }
    }

    /// \copydoc Fact.hasDependentFacts
    public override Boolean hasDependentFacts() {
        return false;
    }

    /// \copydoc Fact.getDependentFactIds
    public override string[] getDependentFactIds() {
        return new string[] { };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = this.Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    public override int GetHashCode()
    {
        return this.Point.GetHashCode() ^ this.Normal.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(PointFact f1, PointFact f2)
    {
        return Math3d.IsApproximatelyEqual(f1.Point, f2.Point);
    }

}

/// <summary>
/// Line within 3D Space of finite length
/// </summary>
public class LineFact : AbstractLineFactWrappedCRTP<LineFact>
{
    /// <summary> Distance between <see cref="AbstractLineFact.Pid1"/> and <see cref="AbstractLineFact.Pid2"/></summary>
    public float Distance;

    /// <summary> \copydoc Fact.Fact </summary>
    public LineFact() : base()
    {
        Distance = 0;
    }

    /// <summary> \copydoc AbstractLineFact.AbstractLineFact(AbstractLineFact, Dictionary<string, string>, FactOrganizer) </summary>
    public LineFact(LineFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, old_to_new, organizer)
    {
        init(old_to_new[fact.Pid1], old_to_new[fact.Pid2]);
    }

    /// <summary> \copydoc AbstractLineFact.AbstractLineFact(string, string, string, FactOrganizer) </summary>
    public LineFact(string pid1, string pid2, string backendURI, FactOrganizer organizer) : base(pid1, pid2, backendURI, organizer)
    {
        SetDistance();
        _ = this.Label;
    }

    /// <summary> \copydoc AbstractLineFact.AbstractLineFact(string, string, FactOrganizer) </summary>
    public LineFact(string pid1, string pid2, FactOrganizer organizer) : base(pid1, pid2, organizer)
    {
        init(pid1, pid2);
    }

    /// <summary>
    /// Initiates <see cref="AbstractLineFact.Pid1"/>, <see cref="AbstractLineFact.Pid2"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="pid1">sets <see cref="AbstractLineFact.Pid1"/></param>
    /// <param name="pid2">sets <see cref="AbstractLineFact.Pid2"/></param>
    private void init(string pid1, string pid2)
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

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static LineFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        string pointAUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;
        string pointBUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[1]).uri;

        if (StageStatic.stage.factState.ContainsKey(pointAUri)
         && StageStatic.stage.factState.ContainsKey(pointBUri))
            return new LineFact(pointAUri, pointBUri, uri, StageStatic.stage.factState);

        //If dependent facts do not exist return null
        else {
            return null;
        }
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return "[" + _Facts[Pid1].Label + _Facts[Pid2].Label + "]";
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid1].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid2].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
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

    /// <summary> Calculates and sets <see cref="Distance"/>; <remarks> <see cref="AbstractLineFact.Pid1"/> and <see cref="AbstractLineFact.Pid2"/> needs to be set first.</remarks></summary>
    private void SetDistance()
    {
        this.Distance = Vector3.Distance(((PointFact)_Facts[Pid1]).Point, ((PointFact)_Facts[Pid2]).Point);
    }
}

/// <summary>
/// Ray within 3D Space of infinite length
/// </summary>
public class RayFact : AbstractLineFactWrappedCRTP<RayFact>
{
    /// <summary> \copydoc Fact.Fact </summary>
    public RayFact() : base() { }

    /// <summary> \copydoc AbstractLineFact.AbstractLineFact(AbstractLineFact, Dictionary<string, string>, FactOrganizer) </summary>
    public RayFact(RayFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, old_to_new, organizer)
    {
        init(old_to_new[fact.Pid1], old_to_new[fact.Pid2]);
    }

    /// <summary> \copydoc AbstractLineFact.AbstractLineFact(string, string, string, FactOrganizer) </summary>
    public RayFact(string pid1, string pid2, string backendURI, FactOrganizer organizer) : base(pid1, pid2, backendURI, organizer)
    {
        _ = this.Label;
    }

    /// <summary> \copydoc AbstractLineFact.AbstractLineFact(string, string, FactOrganizer) </summary>
    public RayFact(string pid1, string pid2, FactOrganizer organizer) : base(pid1, pid2, organizer)
    {
        init(pid1, pid2);
    }

    /// <summary>
    /// Initiates <see cref="AbstractLineFact.Pid1"/>, <see cref="AbstractLineFact.Pid2"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="pid1">sets <see cref="AbstractLineFact.Pid1"/></param>
    /// <param name="pid2">sets <see cref="AbstractLineFact.Pid2"/></param>
    private void init(string pid1, string pid2)
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

        MMTSymbolDeclaration mmtDecl = new MMTSymbolDeclaration(this.Label, tp, df);
        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static RayFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        if ((OMA)((Scroll.ScrollSymbolFact)fact).df != null)
        {
            string pointAUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[0]).uri;
            string pointBUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[1]).uri;

            if (StageStatic.stage.factState.ContainsKey(pointAUri)
             && StageStatic.stage.factState.ContainsKey(pointBUri))
                return new RayFact(pointAUri, pointBUri, uri, StageStatic.stage.factState);

            //If dependent facts do not exist return null
        }
        return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return "–" + _Facts[Pid1].Label + _Facts[Pid2].Label + "–";
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = this.Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
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

/// <summary>
/// A <see cref="PointFact"/> on a <see cref="AbstractLineFact"/>
/// </summary>
public class OnLineFact : FactWrappedCRTP<OnLineFact>
{
    public string
        /// <summary> <see cref="PointFact"/>.<see cref="Fact.Id">Id</see> </summary>
        Pid,
        /// <summary> <see cref="AbstractLineFact"/>.<see cref="Fact.Id">Id</see> </summary>
        Rid;

    /// <summary> \copydoc Fact.Fact </summary>
    public OnLineFact() : base()
    {
        this.Pid = null;
        this.Rid = null;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public OnLineFact(OnLineFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Pid], old_to_new[fact.Rid]);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="pid">sets <see cref="Pid"/></param>
    /// <param name="rid">sets <see cref="Rid"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public OnLineFact(string pid, string rid, FactOrganizer organizer) : base(organizer)
    {
        init(pid, rid);
    }

    /// <summary>
    /// Initiates <see cref="Pid"/>, <see cref="Rid"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="pid">sets <see cref="Pid"/></param>
    /// <param name="rid">sets <see cref="Rid"/></param>
    private void init(string pid, string rid)
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

        MMTSymbolDeclaration mmtDecl = new MMTSymbolDeclaration(this.Label, tp, df);
        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="pid">sets <see cref="Pid"/></param>
    /// <param name="rid">sets <see cref="Rid"/></param>
    /// <param name="uri">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public OnLineFact(string pid, string rid, string uri, FactOrganizer organizer) : base(organizer)
    {
        this.Pid = pid;
        this.Rid = rid;
        this._URI = uri;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static OnLineFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        string lineUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0]).uri;
        string pointUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).uri;

        if (StageStatic.stage.factState.ContainsKey(pointUri)
         && StageStatic.stage.factState.ContainsKey(lineUri))
            return new OnLineFact(pointUri, lineUri, uri, StageStatic.stage.factState);

        //If dependent facts do not exist return null
        else
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return _Facts[Pid].Label + "∈" + _Facts[Rid].Label;
    }

    /// \copydoc Fact.hasDependentFacts
    public override Boolean hasDependentFacts()
    {
        return true;
    }

    /// \copydoc Fact.getDependentFactIds
    public override string[] getDependentFactIds()
    {
        return new string[] { Pid, Rid };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Rid].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    public override int GetHashCode()
    {
        return this.Pid.GetHashCode() ^ this.Rid.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
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

/// <summary>
/// Angle comprised of three <see cref="PointFact">PointFacts</see> [A,B,C]
/// </summary>
public class AngleFact : FactWrappedCRTP<AngleFact>
{
    /// @{ <summary>
    /// One <see cref="Fact.Id">Id</see> of three <see cref="PointFact">PointFacts</see> defining Angle [<see cref="Pid1"/>, <see cref="Pid2"/>, <see cref="Pid3"/>].
    /// </summary>
    public string Pid1, Pid2, Pid3;
    /// @}

    /// <summary> <see langword="true"/>, if AngleFact is approximately 90° or 270°</summary>
    public bool is_right_angle;


    /// <summary> \copydoc Fact.Fact </summary>
    public AngleFact() : base()
    {
        this.Pid1 = null;
        this.Pid2 = null;
        this.Pid3 = null;
        this.is_right_angle = false;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public AngleFact(AngleFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Pid1], old_to_new[fact.Pid2], old_to_new[fact.Pid3]);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="pid2">sets <see cref="Pid2"/></param>
    /// <param name="pid3">sets <see cref="Pid3"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public AngleFact(string pid1, string pid2, string pid3, FactOrganizer organizer) : base(organizer)
    {
        init(pid1, pid2, pid3);
    }

    /// <summary>
    /// Initiates <see cref="Pid1"/>, <see cref="Pid2"/>, <see cref="Pid3"/>, <see cref="is_right_angle"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="pid2">sets <see cref="Pid2"/></param>
    /// <param name="pid3">sets <see cref="Pid3"/></param>
    private void init(string pid1, string pid2, string pid3)
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

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="Pid1">sets <see cref="Pid1"/></param>
    /// <param name="Pid2">sets <see cref="Pid2"/></param>
    /// <param name="Pid3">sets <see cref="Pid3"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public AngleFact(string Pid1, string Pid2, string Pid3, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Pid1 = Pid1;
        this.Pid2 = Pid2;
        this.Pid3 = Pid3;

        GetAngle();

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static AngleFact parseFact(Scroll.ScrollFact fact)
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

        if (StageStatic.stage.factState.ContainsKey(pointAUri)
         && StageStatic.stage.factState.ContainsKey(pointBUri)
         && StageStatic.stage.factState.ContainsKey(pointCUri))

            return new AngleFact(pointAUri, pointBUri, pointCUri, uri, StageStatic.stage.factState);

        else    //If dependent facts do not exist return null
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return (is_right_angle ? "⊾" : "∠") + _Facts[Pid1].Label + _Facts[Pid2].Label + _Facts[Pid3].Label;
    }

    /// <summary>
    /// Computes smallest angle and sets <see cref="is_right_angle"/>
    /// </summary>
    /// <returns>Smallets angle between [<see cref="Pid1"/>, <see cref="Pid2"/>] and [<see cref="Pid2"/>, <see cref="Pid3"/>]</returns>
    private float GetAngle()
    {
        PointFact pf1 = _Facts[Pid1] as PointFact;
        PointFact pf2 = _Facts[Pid2] as PointFact;
        PointFact pf3 = _Facts[Pid3] as PointFact;

        float v = Vector3.Angle((pf1.Point - pf2.Point), (pf3.Point - pf2.Point));
        this.is_right_angle = Mathf.Abs(v - 90.0f) < 0.01;

        return is_right_angle ? 90f : v;
    }

    /// <summary>
    /// Constructs struct for right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="val">Angle == 90f, _not checked_</param>
    /// <param name="p1URI"><see cref="Pid1"/></param>
    /// <param name="p2URI"><see cref="Pid2"/></param>
    /// <param name="p3URI"><see cref="Pid3"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
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

    /// <summary>
    /// Constructs struct for not-right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="val">Angle != 90f, _not checked_</param>
    /// <param name="p1URI"><see cref="Pid1"/></param>
    /// <param name="p2URI"><see cref="Pid2"/></param>
    /// <param name="p3URI"><see cref="Pid3"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
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

    /// \copydoc Fact.hasDependentFacts
    public override Boolean hasDependentFacts()
    {
        return true;
    }

    /// \copydoc Fact.getDependentFactIds
    public override string[] getDependentFactIds()
    {
        return new string[] { Pid1, Pid2, Pid3 };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform) {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid1].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid2].Label;
        obj.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid3].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    public override int GetHashCode()
    {
        return this.Pid1.GetHashCode() ^ this.Pid2.GetHashCode() ^ this.Pid3.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
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