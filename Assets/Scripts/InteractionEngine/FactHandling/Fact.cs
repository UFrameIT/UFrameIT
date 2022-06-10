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
        {MMTURIs.LineOf, RayFact.parseFact},

        {MMTURIs.OnLine, OnLineFact.parseFact},
        //90Degree-Angle
        {MMTURIs.Eq, AngleFact.parseFact},
        //Parallel-LineFact
        {MMTURIs.ParallelLine, ParallelLineFact.parseFact},
        //CircleFact
        {MMTURIs.CircleType3d, CircleFact.parseFact},
        {MMTURIs.OnCircle, OnCircleFact.parseFact },
        {MMTURIs.AnglePlaneLine, AngleCircleLineFact.parseFact },
        {MMTURIs.RadiusCircleMetric, RadiusFact.parseFact },
        {MMTURIs.AreaCircle, AreaCircleFact.parseFact },
        {MMTURIs.OrthoCircleLine, OrthogonalCircleLineFact.parseFact },
        {MMTURIs.VolumeCone ,ConeVolumeFact.parseFact  },
        {MMTURIs.TruncatedVolumeCone ,TruncatedConeVolumeFact.parseFact  },
        {MMTURIs.RightAngle, RightAngleFact.parseFact },
        {MMTURIs.CylinderVolume, CylinderVolumeFact.parseFact }




    };
    /// Current solution to retrieve the fact ID from 
    
    public static string MMTermToString (MMTTerm term){
        if(term == null)
            return null;
        // case for OMA 
        if( term is OMA){
            OMA term_casted = (OMA) term;
            string applicant = ((OMS)term_casted.applicant).uri;
            string argument = "";
            for (int i = 0; i < term_casted.arguments.Count; i++) {
                argument = argument+ " " + MMTermToString(term_casted.arguments[i]);
            }
            return " " + applicant + " "+ argument;

        }
        // case for OMS 
        if (term is OMS)
        {
            OMS term_casted = (OMS)term;  
            return term_casted.uri;

        }

        // case for OMF
        if (term is OMF) {
            OMF term_casted = (OMF)term;
            return term_casted.f.ToString();

        }
        


        return "couldn't understand the type";


    }

    public static Dictionary<string, string> parseTermsToId = new Dictionary<string, string>();


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
        string parse_id = ParsingDictionary.MMTermToString(df);
        ParsingDictionary.parseTermsToId[parse_id] = this._URI;


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
        if (df == null)
            return null;

       
        float a = (float)((OMF)df.arguments[0]).f;
        float b = (float)((OMF)df.arguments[1]).f;
        float c = (float)((OMF)df.arguments[2]).f;

        string parse_id = ParsingDictionary.MMTermToString(df);
     
        Debug.Log("point added: " + parse_id);
        if(!ParsingDictionary.parseTermsToId.ContainsKey(parse_id))
                ParsingDictionary.parseTermsToId[parse_id] = uri;


         return new PointFact(a, b, c, uri, StageStatic.stage.factState);
        
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
        string parse_id = ParsingDictionary.MMTermToString(df);
        Debug.Log("point added: " + parse_id);
        ParsingDictionary.parseTermsToId[parse_id] = this._URI;


    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static RayFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;


        // temporary fix to the problem, that actually calculating stuff leads to losing the IDS
        /**  if (fact.kind == "veq") {
              Debug.Log("RayFact veq activated");
              string pointAUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;
              string pointBUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[1]).uri;
              if (StageStatic.stage.factState.ContainsKey(pointAUri)
               && StageStatic.stage.factState.ContainsKey(pointBUri))
                  return new RayFact(pointAUri, pointBUri, uri, StageStatic.stage.factState);

              return null;
          }*/
        if ((OMA)((Scroll.ScrollSymbolFact)fact).df == null)
            return null;
       
        string pointAUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[0]).uri;
        string pointBUri = ((OMS)((OMA)((Scroll.ScrollSymbolFact)fact).df).arguments[1]).uri;

         if (StageStatic.stage.factState.ContainsKey(pointAUri)
             && StageStatic.stage.factState.ContainsKey(pointBUri))
             return new RayFact(pointAUri, pointBUri, uri, StageStatic.stage.factState);

       //If dependent facts do not exist return null
        
        return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
       // return "–" + _Facts[Pid1].Label + _Facts[Pid2].Label + "–";
        return _Facts[Pid1].Label + _Facts[Pid2].Label ;

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

        string lineUri = "";
        string pointUri = "";
        
        if (((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0] is OMS)
        {
            // standard case
            lineUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0]).uri;
            pointUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).uri;
        }
        else {
            // case when line Uri has a projl on the line Argument 
            lineUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0]).arguments[0]).uri;
            pointUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).uri;


        }
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
        if (Mathf.Abs(Mathf.Abs(v) - 90.0f) < 0.1)
        {
            // also generate a RightAngleFact 
            RightAngleFact rightAngle = new RightAngleFact(pid1, pid2, pid3, StageStatic.stage.factState);
            // I have no clue if this is actually necessary
            bool exists;
            StageStatic.stage.factState.Add(rightAngle, out exists, true);
        }


        MMTDeclaration mmtDecl;
        string p1URI = pf1.Id;
        string p2URI = pf2.Id;
        string p3URI = pf3.Id;
       // if (is_right_angle)
       //     mmtDecl = generate90DegreeAngleDeclaration(v, p1URI, p2URI, p3URI);
       // else
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

        float v = GetAngle();

        if (Mathf.Abs(Mathf.Abs(v) - 90.0f) < 0.1)
        {
            // also generate an orthogonal circle line fact

            RightAngleFact rightAngle = new RightAngleFact(Pid1, Pid2, Pid3, StageStatic.stage.factState);
            // is it necessary to make a boolean variable here?
            bool exists;
            StageStatic.stage.factState.Add(rightAngle, out exists, true);
        }



        this._URI = backendURI;
        _ = this.Label;
    }

    public AngleFact(string Pid1, string Pid2, string Pid3,float angle, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Pid1 = Pid1;
        this.Pid2 = Pid2;
        this.Pid3 = Pid3;

        if (Mathf.Abs(Mathf.Abs(angle) - 90.0f) < 0.1)
        {
            // also generate an orthogonal circle line fact

            RightAngleFact rightAngle = new RightAngleFact(Pid1, Pid2, Pid3, StageStatic.stage.factState);
            // I have no clue if this is actually necessary
            bool exists;
            StageStatic.stage.factState.Add(rightAngle, out exists, true);
        }
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

        float angle = 0.0f;
        //If angle is not a 90Degree-Angle
        if (fact.GetType().Equals(typeof(Scroll.ScrollValueFact)))
        {
            OMA df = (OMA)((Scroll.ScrollValueFact)fact).lhs;

            if (df == null)
                return null;

      

            if (((Scroll.ScrollValueFact)fact).value != null)
                angle = ((OMF)(((Scroll.ScrollValueFact)fact).value)).f;


            pointAUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;
            pointBUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[1]).uri;
            pointCUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[2]).uri;

        }
        // this should never happen anymore
        //If angle is a 90Degree-Angle
        else {
            Debug.Log("Angle 90 degrees parsed. This shouldn't happen anymore");
            
            pointAUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).arguments[0]).uri;
            pointBUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).arguments[1]).uri;
            pointCUri = ((OMS)((OMA)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).arguments[2]).uri;
        }

        if (StageStatic.stage.factState.ContainsKey(pointAUri)
         && StageStatic.stage.factState.ContainsKey(pointBUri)
         && StageStatic.stage.factState.ContainsKey(pointCUri))
        {
//                return new AngleFact(pointAUri, pointBUri, pointCUri, uri, StageStatic.stage.factState);
                return new AngleFact(pointAUri, pointBUri, pointCUri,angle, uri, StageStatic.stage.factState);
        }
        else
        {   //If dependent facts do not exist return null
            return null;
        }
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

        return (p1f1.Equivalent(p1f2) && p2f1.Equivalent(p2f2) && p3f1.Equivalent(p3f2));
        //|| (p1f1.Equivalent(p3f2) && p2f1.Equivalent(p2f2) && p1f1.Equivalent(p3f2));
    }
}


/// TODO Work in Progress
/// <summary>
/// Two parallel Lines comprised of two <see cref="LineFact">LineFacts</see> 
/// </summary>
public class ParallelLineFact : FactWrappedCRTP<ParallelLineFact>
{
    /// @{ <summary>
    /// One <see cref="Fact.Id">Id</see> of thwo <see cref="LineFact">PointFacts</see> defining Angle [<see cref="Lid1"/>, <see cref="Lid2"/>].
    /// </summary>
    public string Lid1, Lid2;
    /// @}




    /// <summary> \copydoc Fact.Fact </summary>
    public ParallelLineFact() : base()
    {
        this.Lid1 = null;
        this.Lid2 = null;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public ParallelLineFact(ParallelLineFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Lid1], old_to_new[fact.Lid2]);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="lid1">sets <see cref="Lid1"/></param>
    /// <param name="lid2">sets <see cref="Lid2"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public ParallelLineFact(string lid1, string lid2, FactOrganizer organizer) : base(organizer)
    {
        init(lid1, lid2);
    }

    /// <summary>
    /// Initiates <see cref="Lid1"/>, <see cref="Lid2"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="lid1">sets <see cref="Lid1"/></param>
    /// <param name="lid2">sets <see cref="Lid2"/></param>
    private void init(string lid1, string lid2)
    {
        //TODO must be parallel lines 
        this.Lid1 = lid1;
        this.Lid2 = lid2;

        RayFact lf1 = _Facts[lid1] as RayFact;
        RayFact lf2 = _Facts[lid2] as RayFact;

        MMTDeclaration mmtDecl;
        string l1URI = lf1.Id;
        string l2URI = lf2.Id;
        mmtDecl = generateParallelLineDeclaration( l1URI, l2URI);
    
        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="Lid1">sets <see cref="Lid1"/></param>
    /// <param name="Lid2">sets <see cref="Lid2"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public ParallelLineFact(string Lid1, string Lid2, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Lid1 = Lid1;
        this.Lid2 = Lid2;

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static ParallelLineFact parseFact(Scroll.ScrollFact fact)
    {
        OMA tp = (OMA)((Scroll.ScrollSymbolFact)fact).tp;
        if (tp == null)
            return null;

        string lineAUri = "";
        string lineBUri = "";

        string uri = fact.@ref.uri;
        OMA proof_OMA = (OMA)((Scroll.ScrollSymbolFact)fact).tp; // proof DED
   

        OMA parallel_lines_OMA = (OMA) proof_OMA.arguments[0]; // parallel

        if (parallel_lines_OMA.arguments[0] is OMS) {
            // Normaler Fall 
            lineAUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0]).uri;
            lineBUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).uri;

        }
        // Second case might be redundant by now
        else {
            OMA Projl_line_A_OMA = (OMA)parallel_lines_OMA.arguments[0]; // ProjectL
            lineAUri = ((OMS)Projl_line_A_OMA.arguments[0]).uri;
            OMA Projl_line_B_OMA = (OMA)parallel_lines_OMA.arguments[1]; // ProjectL
            lineBUri = ((OMS)Projl_line_B_OMA.arguments[0]).uri;


        }


        if (StageStatic.stage.factState.ContainsKey(lineAUri)
         && StageStatic.stage.factState.ContainsKey(lineBUri))

            return new ParallelLineFact(lineAUri,lineBUri, uri, StageStatic.stage.factState);

        else    //If dependent facts do not exist return null
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return "||" + _Facts[Lid1].Label + _Facts[Lid2].Label;
    }



    /// <summary>
    /// Constructs struct for right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="l1URI"><see cref="Lid1"/></param>
    /// <param name="l2URI"><see cref="Lid2"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateParallelLineDeclaration(string l1URI, string l2URI)
    {
        
        List<MMTTerm> innerArguments = new List<MMTTerm>
        {
            new OMS(l1URI),
            new OMS(l2URI)
        };

        List<MMTTerm> outerArguments = new List<MMTTerm>
        {
            new OMA(new OMS(MMTURIs.ParallelLine), innerArguments)
        };

        //OMS constructor generates full URI
        MMTTerm tp = new OMA(new OMS(MMTURIs.Ded), outerArguments);
        MMTTerm df = null;

        MMTSymbolDeclaration mmtDecl = new MMTSymbolDeclaration(this.Label, tp, df);
        AddFactResponse.sendAdd(mmtDecl, out this._URI);


        return mmtDecl;
    }


    /// \copydoc Fact.hasDependentFacts
    public override Boolean hasDependentFacts()
    {
        return true;
    }

    /// \copydoc Fact.getDependentFactIds
    public override string[] getDependentFactIds()
    {
        return new string[] { Lid1, Lid2 };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Lid1].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Lid2].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    public override int GetHashCode()
    {
        return this.Lid1.GetHashCode() ^ this.Lid2.GetHashCode() ;
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(ParallelLineFact f1, ParallelLineFact f2)
    {
        if ((f1.Lid1 == f2.Lid1 && f1.Lid2 == f2.Lid2))
            return true;

        RayFact r1f1 = (RayFact)_Facts[f1.Lid1];
        RayFact r2f1 = (RayFact)_Facts[f1.Lid2];
        RayFact r1f2 = (RayFact)_Facts[f2.Lid1];
        RayFact r2f2 = (RayFact)_Facts[f2.Lid2];

        return (r1f1.Equivalent(r1f2) && r2f1.Equivalent(r2f2)) ;
    }
}




//TODO big work in progress Circle Wrapper

/// TODO Work in Progress
/// <summary>
/// A Circle that is made out of a middle point, a plane and a radius  
/// </summary>
public class CircleFact : FactWrappedCRTP<CircleFact>
{
   
    /// <summary> defining the middle point of the circle  </summary>
    public string Pid1;
    /// <summary>  defining the base point of the circle plane </summary>
    public string Pid2;
    /// <summary>  radius of the circle </summary>
    public float radius;
    /// <summary> normal vector of the plane </summary>
    public Vector3 normal;
   
    /// <summary> \copydoc Fact.Fact </summary>
    public CircleFact() : base()
    {
        this.normal = Vector3.zero;
        this.Pid1 = null;
        this.Pid2 = null;
        this.radius = 0;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public CircleFact(CircleFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Pid1], old_to_new[fact.Pid2], fact.radius, fact.normal);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="pid2">sets <see cref="Pid2"/></param>
    /// <param name="radius">sets <see cref="radius"/></param>
    /// <param name="normal">sets <see cref="normal"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public CircleFact(string pid1, string pid2, float radius, Vector3 normal, FactOrganizer organizer) : base(organizer)
    {
        init(pid1, pid2, radius, normal);
    }

    /// <summary>
    /// Initiates <see cref="Pid1"/>, <see cref="Pid2"/>, <see cref="radius"/>,<see cref="dir1"/>,<see cref="dir2"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="pid2">sets <see cref="Pid2"/></param>
    /// <param name="radius">sets <see cref="radius"/></param>
    /// <param name="normal">sets <see cref="normal"/></param>
    private void init(string pid1, string pid2, float radius, Vector3 normal)
    {
        this.Pid1 = pid1;
        this.Pid2 = pid2;

        PointFact pf1 = _Facts[pid1] as PointFact;
        PointFact pf2 = _Facts[pid2] as PointFact;


        this.radius = radius;
        this.normal = normal;

        MMTDeclaration mmtDecl;
        string p1URI = pf1.Id;
        string p2URI = pf2.Id;

        mmtDecl = generateCircleFactDeclaration(p1URI, p2URI, radius, normal);

        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="Pid1">sets <see cref="Pid1"/></param>
    /// <param name="Pid2">sets <see cref="Pid2"/></param>
    /// <param name="radius">sets <see cref="radius"/></param>
    /// <param name="normal">sets <see cref="normal"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public CircleFact(string Pid1, string Pid2, float radius, Vector3 normal, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Pid1 = Pid1;
        this.Pid2 = Pid2;

        this.radius = radius;
        this.normal = normal;
 
        this._URI = backendURI;
        _ = this.Label;
    }

    /// <summary>
    /// parses the Circlefact response of the MMT-Server
    /// </summary>
    /// \copydoc Fact.parseFact(Scroll.ScrollFact) 
    public new static CircleFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;

        OMA df = (OMA)((Scroll.ScrollSymbolFact)fact).df;

        if (df == null)
            return null;

        Scroll.ScrollSymbolFact casted_fact = (Scroll.ScrollSymbolFact)fact;


        OMA planeOMA = (OMA) ((OMA)casted_fact.df).arguments[0];
        OMA pointAOMA = (OMA) planeOMA.arguments[0];

        OMA n = (OMA) planeOMA.arguments[1];
        Vector3 normal = new Vector3(((OMF)n.arguments[0]).f, ((OMF)n.arguments[1]).f, ((OMF)n.arguments[2]).f);


        // get the mid point uri
        string parse_id_M = ParsingDictionary.MMTermToString(((OMA)casted_fact.df).arguments[1]);
        string M_uri = ParsingDictionary.parseTermsToId[parse_id_M];

        string parse_id_A = ParsingDictionary.MMTermToString(planeOMA.arguments[0]);
        string A_uri = ParsingDictionary.parseTermsToId[parse_id_A];

        // get the radius
        float radius = ((OMF)((OMA)casted_fact.df).arguments[2]).f;


        if (StageStatic.stage.factState.ContainsKey(M_uri)
         && StageStatic.stage.factState.ContainsKey(A_uri))
              return new CircleFact(M_uri,A_uri,radius,normal, uri, StageStatic.stage.factState);

        else    //If dependent facts do not exist return null
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {

        return  "○"+_Facts[Pid1].Label;
    }



    /// <summary>
    /// Constructs struct for right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="p1URI"> <see cref="Pid1"/></param>
    /// <param name="p2URI"> <see cref="Pid2"/></param>
    /// <param name="radius"> <see cref="radius"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateCircleFactDeclaration( string p1URI, string p2URI, float radius, Vector3 normal)
    {
        PointFact p1 = _Facts[p1URI] as PointFact;
        PointFact p2 = _Facts[p2URI] as PointFact;

        List<MMTTerm> normalArgs = new List<MMTTerm>
        {
            new OMF(normal.x),
            new OMF(normal.y),
            new OMF(normal.z)
        };
        OMA NormalVector = new OMA(new OMS(MMTURIs.Tuple), normalArgs);



        List<MMTTerm> planeArgs = new List<MMTTerm>
        {
            new OMS(p2URI),
            NormalVector //n
        };

        OMA CirclePlane = new OMA(new OMS(MMTURIs.pointNormalPlane), planeArgs);
        OMS middlePoint = new OMS(p1URI);
        OMF Radius = new OMF(radius);

        List<MMTTerm> outerArguments = new List<MMTTerm>
        {
           CirclePlane,
           middlePoint,
           Radius
        };

        //OMS constructor generates full URI
        // Do i need this here? doubt 
        MMTTerm tp = new OMS(MMTURIs.CircleType3d);
        MMTTerm df = new OMA(new OMS(MMTURIs.MkCircle3d), outerArguments);

        MMTSymbolDeclaration mmtDecl = new MMTSymbolDeclaration(this.Label, tp, df);
        AddFactResponse.sendAdd(mmtDecl, out this._URI);


        return mmtDecl;
    }


    /// \copydoc Fact.hasDependentFacts
    public override Boolean hasDependentFacts()
    {
        return true;
    }

    public Vector3 getNormal() 
    {
        return normal;
    }


    /// \copydoc Fact.getDependentFactIds
    public override string[] getDependentFactIds()
    {
        return new string[] { Pid1,Pid2 };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid1].Label;
 
        // obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Lid2].Label;

        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    public override int GetHashCode()
    {
        return  this.Pid1.GetHashCode() ^ this.Pid2.GetHashCode() ;
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(CircleFact f1, CircleFact f2)
    {
        if ( f1.Pid1 == f2.Pid1 && f1.normal == f2.normal && f1.radius == f2.radius)
            return true;

        PointFact p1f1 = (PointFact)_Facts[f1.Pid1];
        PointFact p1f2 = (PointFact)_Facts[f2.Pid1];

        return (p1f1.Equivalent(p1f2) && f1.normal == f2.normal && f1.radius == f2.radius);
    }
}


/// <summary>
/// A <see cref="PointFact"/> on a <see cref="CircleFact"/>
/// </summary>
public class OnCircleFact : FactWrappedCRTP<OnCircleFact>
{
    /// <summary> the point on the circle  </summary>
    public string Pid;
    /// <summary> the circle, which the point is on  </summary>
    public string Cid;

    /// <summary> \copydoc Fact.Fact </summary>
    public OnCircleFact() : base()
    {
        this.Pid = null;
        this.Cid = null;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public OnCircleFact(OnCircleFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Pid], old_to_new[fact.Cid]);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="pid">sets <see cref="Pid"/></param>
    /// <param name="cid">sets <see cref="Cid"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public OnCircleFact(string pid, string cid, FactOrganizer organizer) : base(organizer)
    {
        init(pid, cid);
    }

    /// <summary>
    /// Initiates <see cref="Pid"/>, <see cref="Rid"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="pid">sets <see cref="Pid"/></param>
    /// <param name="cid">sets <see cref="Cid"/></param>
    private void init(string pid, string cid)
    {
        this.Pid = pid;
        this.Cid = cid;

        PointFact pf = _Facts[pid] as PointFact;
        CircleFact cf = _Facts[cid] as CircleFact;
        string pURI = pf.Id;
        string cURI = cf.Id;

        List<MMTTerm> innerArguments = new List<MMTTerm>
        {
            new OMS(cURI),
            new OMS(pURI)
        };

        List<MMTTerm> outerArguments = new List<MMTTerm>
        {
            new OMA(new OMS(MMTURIs.OnCircle), innerArguments)
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
    /// <param name="cid">sets <see cref="Cid"/></param>
    /// <param name="uri">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public OnCircleFact(string pid, string cid, string uri, FactOrganizer organizer) : base(organizer)
    {
        this.Pid = pid;
        this.Cid = cid;
        this._URI = uri;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static OnCircleFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;

        OMA tp = (OMA)((Scroll.ScrollSymbolFact)fact).tp;
        if (tp == null)
            return null;

        string circleUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0]).uri;
        string pointUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).uri;
       
        if (StageStatic.stage.factState.ContainsKey(pointUri)
         && StageStatic.stage.factState.ContainsKey(circleUri))
            return new OnCircleFact(pointUri, circleUri, uri, StageStatic.stage.factState);

        //If dependent facts do not exist return null
        else
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return _Facts[Pid].Label + "∈" + _Facts[Cid].Label;
    }

    /// \copydoc Fact.hasDependentFacts
    public override Boolean hasDependentFacts()
    {
        return true;
    }

    /// \copydoc Fact.getDependentFactIds
    public override string[] getDependentFactIds()
    {
        return new string[] { Pid, Cid };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid].Label + "∈" + _Facts[this.Cid].Label;
        obj.GetComponent<FactWrapper>().fact = this;

        return obj;
    }

    /// \copydoc Fact.GetHashCode
    public override int GetHashCode()
    {
        return this.Pid.GetHashCode() ^ this.Cid.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(OnCircleFact c1, OnCircleFact c2)
    {
        if (c1.Pid == c2.Pid && c1.Cid == c2.Pid)
            return true;

        PointFact pc1 = (PointFact)_Facts[c1.Pid];
        CircleFact cc1 = (CircleFact)_Facts[c1.Cid];

        PointFact pc2 = (PointFact)_Facts[c2.Pid];
        CircleFact cc2 = (CircleFact)_Facts[c2.Cid];

        return pc1.Equivalent(pc2) && cc1.Equivalent(cc2);
    }
}



/// <summary>
/// Angle comprised of three <see cref="PointFact">PointFacts</see> [A,B,C]
/// </summary>
public class AngleCircleLineFact : FactWrappedCRTP<AngleCircleLineFact>
{
    /// @{ <summary>
    /// One <see cref="Fact.Id">Id</see> of three <see cref="PointFact">PointFacts</see> defining Angle [<see cref="Pid1"/>, <see cref="Pid2"/>, <see cref="Pid3"/>].
    /// </summary>
    public string Cid1, Rid2;
    /// @}
    float angle;

    /// <summary> <see langword="true"/>, if AngleFact is approximately 90° or 270°</summary>


    /// <summary> \copydoc Fact.Fact </summary>
    public AngleCircleLineFact() : base()
    {
        this.Cid1 = null;
        this.Rid2 = null;
        this.angle = 0.0f;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public AngleCircleLineFact(AngleCircleLineFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Cid1], old_to_new[fact.Rid2], fact.angle);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="pid2">sets <see cref="Pid2"/></param>
    /// <param name="pid3">sets <see cref="Pid3"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public AngleCircleLineFact(string cid1, string rid2, float angle, FactOrganizer organizer) : base(organizer)
    {
        init(cid1, rid2, angle);
    }

    /// <summary>
    /// Initiates <see cref="Pid1"/>, <see cref="Pid2"/>, <see cref="Pid3"/>, <see cref="is_right_angle"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="pid2">sets <see cref="Pid2"/></param>
    /// <param name="pid3">sets <see cref="Pid3"/></param>
    private void init(string cid1, string rid2, float angle)
    {
        this.Cid1 = cid1;
        this.Rid2 = rid2;
        this.angle = angle;

        CircleFact cf1 = _Facts[cid1] as CircleFact;
        RayFact rf2 = _Facts[rid2] as RayFact;
        MMTDeclaration mmtDecl;
        string c1URI = cf1.Id;
        string r2URI = rf2.Id;
       
        if (Mathf.Abs(Mathf.Abs(angle) - 90.0f) < 0.1) {
            // also generate an orthogonal circle line fact
            OrthogonalCircleLineFact orthofact = new OrthogonalCircleLineFact(cid1, rid2, StageStatic.stage.factState);
            bool exists;
            StageStatic.stage.factState.Add(orthofact, out exists, true );
        }

        mmtDecl = generateMMTDeclaration(angle, c1URI, r2URI);

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
    public AngleCircleLineFact(string Cid1, string Rid2, float angle, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Cid1 = Cid1;
        this.Rid2 = Rid2;
        this.angle = angle;

        if (Mathf.Abs(Mathf.Abs(angle) - 90.0f) < 0.1)
        {
            // also generate an orthogonal circle line fact
            OrthogonalCircleLineFact orthofact = new OrthogonalCircleLineFact(Cid1, Rid2, StageStatic.stage.factState);
            bool exists;
            StageStatic.stage.factState.Add(orthofact, out exists, true);
        }

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static AngleCircleLineFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        string
            CircleUri,
            RayUri;

        OMA df = (OMA)((Scroll.ScrollValueFact)fact).lhs;

        if (df == null)
            return null;

        // init it with 0 degrees, so we don't accidentally generate orthogonalfacts 
        // and the parsing works correctly if smb ever adds a scroll for this
        float angle = 0.0f;

        if((((Scroll.ScrollValueFact)fact).value)!=null)
            angle = ((OMF)(((Scroll.ScrollValueFact)fact).value)).f;


        CircleUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;
        RayUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[1]).uri;

        if (StageStatic.stage.factState.ContainsKey(CircleUri)
         && StageStatic.stage.factState.ContainsKey(RayUri))
        {
            return new AngleCircleLineFact(CircleUri, RayUri, angle, uri, StageStatic.stage.factState);
        }
        else
        {
            //If dependent facts do not exist return null
            return null;
        }
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return  "∠" + _Facts[Cid1].Label + _Facts[Rid2].Label;
    }

    /// <summary>
    /// Constructs struct for not-right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="val">Angle != 90f, _not checked_</param>
    /// <param name="p1URI"><see cref="Pid1"/></param>
    /// <param name="p2URI"><see cref="Pid2"/></param>
    /// <param name="p3URI"><see cref="Pid3"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateMMTDeclaration(float val, string c1URI, string r2URI)
    {
        MMTTerm lhs =
            new OMA(
                new OMS(MMTURIs.AnglePlaneLine),
                new List<MMTTerm> {
                    new OMS(c1URI),
                    new OMS(r2URI),
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
        return new string[] { Cid1, Rid2 };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Cid1].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Rid2].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    public override int GetHashCode()
    {
        return this.Cid1.GetHashCode() ^ this.Rid2.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(AngleCircleLineFact f1, AngleCircleLineFact f2)
    {
        if (f1.Cid1 == f2.Cid1 && f1.Rid2 == f2.Rid2 )
            return true;

        CircleFact c1f1 = (CircleFact)_Facts[f1.Cid1];
        RayFact r2f1 = (RayFact)_Facts[f1.Rid2];

        CircleFact c1f2 = (CircleFact)_Facts[f2.Cid1];
        RayFact r2f2 = (RayFact)_Facts[f2.Rid2];

        return (c1f1.Equivalent(c1f2) && r2f1.Equivalent(r2f2));
    }
}


/// <summary>
/// A RadiusFact that corresponds to a <see cref="CircleFact">PointFacts</see> and has a float value (the actual radius).
/// </summary>
public class RadiusFact : FactWrappedCRTP<RadiusFact>
{
    ///  <summary> The circle corresponding to the radius </summary>
    public string Cid1;
    ///  <summary> The radius as a float </summary>
    public float rad;

    /// <summary> \copydoc Fact.Fact </summary>
    public RadiusFact() : base()
    {
        this.Cid1 = null;
        this.rad = 0.0f;

    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public RadiusFact(RadiusFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Cid1]);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public RadiusFact(string cid1, FactOrganizer organizer) : base(organizer)
    {
        init(cid1);
    }

    /// <summary>
    /// Initiates <see cref="Cid1"/> and <see cref="rad"/>
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    private void init(string cid1)
    {
        this.Cid1 = cid1;

        CircleFact cf1 = _Facts[cid1] as CircleFact;
        this.rad = cf1.radius;


        MMTDeclaration mmtDecl;
        string c1URI = cf1.Id;


        mmtDecl = generateMMTDeclaration(c1URI,this.rad);

        AddFactResponse.sendAdd(mmtDecl,  out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public RadiusFact(string Cid1, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Cid1 = Cid1;

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static RadiusFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        string CircleUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;

        if (StageStatic.stage.factState.ContainsKey(CircleUri))

            return new RadiusFact(CircleUri, uri, StageStatic.stage.factState);

        else    //If dependent facts do not exist return null
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return "r "+ _Facts[Cid1].Label;
    }

    /// <summary>
    /// Constructs struct for not-right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="rad"> see <see cref="rad"/></param>
    /// <param name="c1URI"> see <see cref="Cid1"/></param>

    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateMMTDeclaration( string c1URI, float rad)
    {
        MMTTerm lhs =
            new OMA(
                new OMS(MMTURIs.RadiusCircleMetric),
                new List<MMTTerm> {
                    new OMS(c1URI),
                }
            );

        MMTTerm valueTp = new OMS(MMTURIs.RealLit);
        MMTTerm value = new OMF(rad);

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
        return new string[] { Cid1};
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "r: "+_Facts[this.Cid1].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    public override int GetHashCode()
    {
        return this.Cid1.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(RadiusFact f1, RadiusFact f2)
    {
        if (f1.Cid1 == f2.Cid1)
            return true;

        CircleFact c1f1 = (CircleFact)_Facts[f1.Cid1];

        CircleFact c1f2 = (CircleFact)_Facts[f2.Cid1];
        // if they correspond to the same circle, then automatically the radius has to be the same

        return (c1f1.Equivalent(c1f2));
    }
}


/// <summary>
/// Area of a <see cref="CircleFact">CircleFact</see> 
/// </summary>
public class AreaCircleFact : FactWrappedCRTP<AreaCircleFact>
{
   /// <summary> the circle <see cref="CircleFact">CircleFact</see>  </summary>
    public string Cid1;
    /// <summary> the area which is contained by the circle </summary>
    public float A;



    /// <summary> \copydoc Fact.Fact </summary>
    public AreaCircleFact() : base()
    {
        this.Cid1 = null;
        this.A = 0.0f;

    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public AreaCircleFact(AreaCircleFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Cid1]);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public AreaCircleFact(string cid1, FactOrganizer organizer) : base(organizer)
    {
        init(cid1);
    }

    /// <summary>
    /// Initiates <see cref="Cid1"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>

    private void init(string cid1)
    {
        this.Cid1 = cid1;

        CircleFact cf1 = _Facts[cid1] as CircleFact;
        this.A = cf1.radius * cf1.radius * ( (float) Math.PI );


        MMTDeclaration mmtDecl;
        string c1URI = cf1.Id;


        mmtDecl = generateMMTDeclaration(c1URI, this.A);

        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="Cid1">sets <see cref="Cid1"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public AreaCircleFact(string Cid1, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Cid1 = Cid1;

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static AreaCircleFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;
        string CircleUri = ((OMS)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).uri;

        if (StageStatic.stage.factState.ContainsKey(CircleUri))
            return new AreaCircleFact(CircleUri, uri, StageStatic.stage.factState);
        else    //If dependent facts do not exist return null
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return "A(" + _Facts[Cid1].Label+")";
    }


    /// <summary>
    /// Constructs a response, that is sent to the MMT-Server
    /// </summary>
    /// <param name="area"> area of the circle </param>
    /// <param name="c1URI">  <see cref="Cid1"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateMMTDeclaration(string c1URI, float area)
    {
        MMTTerm lhs =
            new OMA(
                new OMS(MMTURIs.AreaCircle),
                new List<MMTTerm> {
                    new OMS(c1URI),
                }
            );

        MMTTerm valueTp = new OMS(MMTURIs.RealLit);
        MMTTerm value = new OMF(area);

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
        return new string[] { Cid1 };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Cid1].Label;
        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    /// is this a problem?
    public override int GetHashCode()
    {
        return this.Cid1.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(AreaCircleFact f1, AreaCircleFact f2)
    {
        if (f1.Cid1 == f2.Cid1)
            return true;
        CircleFact c1f1 = (CircleFact)_Facts[f1.Cid1];
        CircleFact c1f2 = (CircleFact)_Facts[f2.Cid1];

        return (c1f1.Equivalent(c1f2) && f1.A == f2.A);
    }
}


/// <summary>
/// The volume of a cone A  defined by a base area  <see cref="CircleFact">CircleFact</see>, an apex <see cref="PointFact">PointFact</see> and the volume as float
/// </summary>
public class ConeVolumeFact : FactWrappedCRTP<ConeVolumeFact>
{
    ///  <summary> a <see cref="CircleFact">CircleFact</see> describing the base area </summary>
    public string Cid1;
    ///  <summary> a <see cref="PointFact">PointFact</see> describing the apex point  </summary>
    public string Pid1;
    ///  <summary> the volume of the cone as a float </summary>
    public float vol;

    /// <summary> \copydoc Fact.Fact </summary>
    public ConeVolumeFact() : base()
    {
        this.Cid1 = null;
        this.Pid1 = null;
        this.vol = 0.0f;

    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public ConeVolumeFact(ConeVolumeFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Cid1], old_to_new[fact.Pid1], fact.vol);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public ConeVolumeFact(string cid1,string pid1, float vol, FactOrganizer organizer) : base(organizer)
    {
        init(cid1,pid1,vol);
    }

    /// <summary>
    /// Initiates <see cref="Pid1"/>, <see cref="Pid2"/>, <see cref="Pid3"/>, <see cref="is_right_angle"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    private void init(string cid1,string pid1, float vol)
    {
        this.Cid1 = cid1;
        this.Pid1 = pid1;

        CircleFact cf1 = _Facts[cid1] as CircleFact;
        PointFact pf1 = _Facts[pid1] as PointFact;
        this.vol = vol;


        MMTDeclaration mmtDecl;
        string c1URI = cf1.Id;
        string p1URI = pf1.Id;


        mmtDecl = generateMMTDeclaration(c1URI, p1URI, vol);

        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public ConeVolumeFact(string Cid1,string Pid1, float volume,  string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Cid1 = Cid1;
        this.Pid1 = Pid1;
        this.vol = volume;

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static ConeVolumeFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;

        if (((Scroll.ScrollValueFact)fact).lhs == null)
            return null;

        string CircleUri = ((OMS)((OMA)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).arguments[0]).uri;
        string PointUri = ((OMS)((OMA)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).arguments[1]).uri;
        float volume = 0.0f;
        if( (((Scroll.ScrollValueFact)fact).value) !=null   )
            volume =  ((OMF) ((Scroll.ScrollValueFact)fact).value).f ;

        if (StageStatic.stage.factState.ContainsKey(CircleUri)&& StageStatic.stage.factState.ContainsKey(PointUri))

            return new ConeVolumeFact(CircleUri,PointUri,volume, uri, StageStatic.stage.factState);

        else    //If dependent facts do not exist return null
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return "V(" + _Facts[Cid1].Label +"," + _Facts[Pid1].Label+")";
    }

    /// <summary>
    /// Constructs struct for not-right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="c1URI"> Uri for <see cref="Cid1"/></param>
    /// <param name="p1URI"> Uri for <see cref="Pid1"/></param>
    /// <param name="val"> <see cref="vol"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateMMTDeclaration(string c1URI, string p1URI, float val)
    {
        MMTTerm lhs =
            new OMA(
                new OMS(MMTURIs.VolumeCone),

                new List<MMTTerm> {
                    new OMA(new OMS(MMTURIs.ConeOfCircleApex),
                        new List<MMTTerm> {
                            new OMS(c1URI),
                            new OMS(p1URI),
                         }
                    ),
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
        return new string[] { Cid1, Pid1 };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text =  _Facts[this.Cid1].Label+ _Facts[this.Pid1].Label;
        obj.GetComponent<FactWrapper>().fact = this;

        return obj;
    }

    /// \copydoc Fact.GetHashCode
    /// uhhh is this a problem?
    public override int GetHashCode()
    {
        return this.Cid1.GetHashCode()^this.Pid1.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(ConeVolumeFact f1, ConeVolumeFact f2)
    {
        if (f1.Cid1 == f2.Cid1 && f1.Pid1 == f2.Pid1)
            return true;

        CircleFact c1f1 = (CircleFact)_Facts[f1.Cid1];
        CircleFact c1f2 = (CircleFact)_Facts[f2.Cid1];

        PointFact p1f1 = (PointFact)_Facts[f1.Pid1];
        PointFact p1f2 = (PointFact)_Facts[f2.Pid1];

        return (c1f1.Equivalent(c1f2)&& p1f1.Equivalent(p1f2) && f1.vol== f2.vol );

    }
}


/// <summary>
/// The fact that the plane of a <see cref="CircleFact">CircleFact</see> and the line <see cref="RayFact>RayFact</see> are orthogonal
/// </summary>
public class OrthogonalCircleLineFact : FactWrappedCRTP<OrthogonalCircleLineFact>
{
    ///  <summary> a <see cref="CircleFact">CircleFact</see> describing the base area </summary>
    public string Cid1;
    ///  <summary> a <see cref="RayFact">Rayfact</see> describing the line </summary>
    public string Lid1;
  

    /// <summary> \copydoc Fact.Fact </summary>
    public OrthogonalCircleLineFact() : base()
    {
        this.Cid1 = null;
        this.Lid1 = null;

    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public OrthogonalCircleLineFact(OrthogonalCircleLineFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Cid1], old_to_new[fact.Lid1]);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public OrthogonalCircleLineFact(string cid1, string lid1, FactOrganizer organizer) : base(organizer)
    {
        init(cid1, lid1);
    }

    /// <summary>
    /// Initiates <see cref="Pid1"/>, <see cref="Pid2"/>, <see cref="Pid3"/>, <see cref="is_right_angle"/>, <see cref="Fact._URI"/> and creates MMT %Fact Server-Side
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    private void init(string cid1, string lid1)
    {
        this.Cid1 = cid1;
        this.Lid1 = lid1;

        CircleFact cf1 = _Facts[cid1] as CircleFact;
        RayFact lf1 = _Facts[lid1] as RayFact;


        MMTDeclaration mmtDecl;
        string c1URI = cf1.Id;
        string l1URI = lf1.Id;


        mmtDecl = generateMMTDeclaration(c1URI, l1URI);

        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public OrthogonalCircleLineFact(string Cid1, string Lid1,  string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Cid1 = Cid1;
        this.Lid1 = Lid1;

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static OrthogonalCircleLineFact parseFact(Scroll.ScrollFact fact)
    {
        OMA tp = (OMA)((Scroll.ScrollSymbolFact)fact).tp;
        if (tp == null)
            return null;

        string uri = fact.@ref.uri;

        string CircleUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0]).uri;
        string LineUri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).uri;

        if (StageStatic.stage.factState.ContainsKey(CircleUri)
         && StageStatic.stage.factState.ContainsKey(LineUri))

            return new OrthogonalCircleLineFact(CircleUri, LineUri, uri, StageStatic.stage.factState);

        else    //If dependent facts do not exist return null
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return   _Facts[Cid1].Label + "⊥" + _Facts[Lid1].Label;
    }

    /// <summary>
    /// Constructs struct for not-right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="c1URI"> Uri for <see cref="Cid1"/></param>
    /// <param name="l1URI"> Uri for <see cref="Lid1"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateMMTDeclaration(string c1URI, string l1URI)
    {
        List<MMTTerm> innerArguments = new List<MMTTerm>
        {
            new OMS(c1URI),
            new OMS(l1URI)
        };

        List<MMTTerm> outerArguments = new List<MMTTerm>
        {
            new OMA(new OMS(MMTURIs.OrthoCircleLine), innerArguments)
        };
        MMTTerm tp = new OMA(new OMS(MMTURIs.Ded), outerArguments);
        MMTTerm df = null;

        return new MMTSymbolDeclaration(this.Label, tp, df);
    }

    /// \copydoc Fact.hasDependentFacts
    public override Boolean hasDependentFacts()
    {
        return true;
    }

    /// \copydoc Fact.getDependentFactIds
    public override string[] getDependentFactIds()
    {
        return new string[] { Cid1, Lid1 };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Cid1].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Lid1].Label;

        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    /// uhhh is this a problem?
    public override int GetHashCode()
    {
        return this.Cid1.GetHashCode() ^ this.Lid1.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(OrthogonalCircleLineFact f1, OrthogonalCircleLineFact f2)
    {
        if (f1.Cid1 == f2.Cid1 && f1.Lid1 == f2.Lid1)
            return true;

        CircleFact c1f1 = (CircleFact)_Facts[f1.Cid1];
        CircleFact c1f2 = (CircleFact)_Facts[f2.Cid1];

        RayFact l1f1 = (RayFact)_Facts[f1.Lid1];
        RayFact l1f2 = (RayFact)_Facts[f2.Lid1];

        return (c1f1.Equivalent(c1f2) && l1f1.Equivalent(l1f2));

    }
}

/// <summary>
/// The volume of a cone A  defined by a base area  <see cref="CircleFact">CircleFact</see>, a top area <see cref="CircleFact">CircleFact</see> and the volume as float
/// </summary>
public class TruncatedConeVolumeFact : FactWrappedCRTP<TruncatedConeVolumeFact>
{
    ///  <summary> a <see cref="CircleFact">CircleFact</see> describing the base area </summary>
    public string Cid1;
    ///  <summary> a <see cref="PointFact">PointFact</see> describing the apex point  </summary>
    public string Cid2;
    ///  <summary> the volume of the cone as a float </summary>
    public float vol;
    ///  <summary> OMA proof that the two circles are parallel  </summary>
    public OMA proof;

    /// <summary> \copydoc Fact.Fact </summary>
    public TruncatedConeVolumeFact() : base()
    {
        this.Cid1 = null;
        this.Cid2 = null;
        this.vol = 0.0f;
        this.proof = null;

    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public TruncatedConeVolumeFact(TruncatedConeVolumeFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Cid1], old_to_new[fact.Cid2], fact.vol, fact.proof);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="cid2">sets <see cref="Cid2"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    /// <param name="proof">sets <see cref="proof"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public TruncatedConeVolumeFact(string cid1, string cid2, float vol, OMA proof, FactOrganizer organizer) : base(organizer)
    {
        init(cid1, cid2, vol, proof);
    }

    /// <summary>
    /// sets variables and generates MMT Declaration
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="cid2">sets <see cref="Cid2"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    /// <param name="proof">sets <see cref="proof"/></param>
    private void init(string cid1, string cid2, float vol, OMA proof)
    {
        this.Cid1 = cid1;
        this.Cid2 = cid2;
        this.proof = proof;

        CircleFact cf1 = _Facts[cid1] as CircleFact;
        CircleFact cf2 = _Facts[cid2] as CircleFact;
        this.vol = vol;


        MMTDeclaration mmtDecl;
        string c1URI = cf1.Id;
        string c2URI = cf2.Id;


        mmtDecl = generateMMTDeclaration(c1URI, c2URI, vol, proof);

        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="Cid1">sets <see cref="Cid1"/></param>
    /// <param name="Cid2">sets <see cref="Cid2"/></param>
    /// <param name="volume">sets <see cref="vol"/></param>
    /// <param name="proof">sets <see cref="proof"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public TruncatedConeVolumeFact(string Cid1, string Cid2, float volume, OMA proof, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Cid1 = Cid1;
        this.Cid2 = Cid2;
        this.vol = volume;
        this.proof = proof;

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static TruncatedConeVolumeFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;

        string Circle1Uri = ((OMS)((OMA)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).arguments[0]).uri;
        string Circle2Uri = ((OMS)((OMA)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).arguments[1]).uri;
        float volume = ((OMF)((Scroll.ScrollValueFact)fact).value).f;

        OMA proof = (OMA)(((OMA)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).arguments[2]);

        if (StageStatic.stage.factState.ContainsKey(Circle1Uri) && StageStatic.stage.factState.ContainsKey(Circle2Uri))

            return new TruncatedConeVolumeFact(Circle1Uri, Circle2Uri, volume, proof, uri,  StageStatic.stage.factState);

        else    //If dependent facts do not exist return null
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return "V(" + _Facts[Cid1].Label +"," + _Facts[Cid2].Label+")";
    }



    /// <summary>
    /// Constructs struct for not-right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="c1URI"> Uri for <see cref="Cid1"/></param>
    /// <param name="c2URI"> Uri for <see cref="Cid2"/></param>
    /// <param name="val"> <see cref="vol"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateMMTDeclaration(string c1URI, string c2URI, float val, OMA proof)
    {
        MMTTerm lhs =
            new OMA(
                new OMS(MMTURIs.TruncatedVolumeCone),

                new List<MMTTerm> {
                    new OMS(c1URI),
                    new OMS(c2URI),
                    proof,
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
        return new string[] { Cid1, Cid2 };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Cid1].Label + _Facts[this.Cid2].Label;
        obj.GetComponent<FactWrapper>().fact = this;

        return obj;
    }

    /// \copydoc Fact.GetHashCode
    /// uhhh is this a problem?
    public override int GetHashCode()
    {
        return this.Cid1.GetHashCode() ^ this.Cid2.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(TruncatedConeVolumeFact f1, TruncatedConeVolumeFact f2)
    {
        if (f1.Cid1 == f2.Cid1 && f1.Cid2 == f2.Cid2)
            return true;

        CircleFact c1f1 = (CircleFact)_Facts[f1.Cid1];
        CircleFact c1f2 = (CircleFact)_Facts[f2.Cid1];

        CircleFact c2f1 = (CircleFact)_Facts[f1.Cid2];
        CircleFact c2f2 = (CircleFact)_Facts[f2.Cid2];

        return (c1f1.Equivalent(c1f2) && c2f1.Equivalent(c2f2) && f1.vol == f2.vol);

    }
}


/// <summary>
/// A RightAngleFact defined by 3  <see cref="PointFact">Pointfact</see> 
/// </summary>
public class RightAngleFact : FactWrappedCRTP<RightAngleFact>
{
    ///  <summary> three <see cref="PointFact">Pointfacts</see> defining the right angle </summary>
    public string Pid1, Pid2, Pid3;



    /// <summary> \copydoc Fact.Fact </summary>
    public RightAngleFact() : base()
    {
        this.Pid1 = null;
        this.Pid2 = null;
        this.Pid3 = null;
    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public RightAngleFact(RightAngleFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Pid1], old_to_new[fact.Pid2],old_to_new[fact.Pid3]);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="pid1">sets <see cref="Pid1"/></param>
    /// <param name="pid2">sets <see cref="Pid2"/></param>
    /// <param name="pid3">sets <see cref="Pid3"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public RightAngleFact(string pid1, string pid2, string pid3, FactOrganizer organizer) : base(organizer)
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


        MMTDeclaration mmtDecl;
        string p1URI = pf1.Id;
        string p2URI = pf2.Id;
        string p3URI = pf3.Id;


        mmtDecl = generateMMTDeclaration(p1URI, p2URI, p3URI);

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
    public RightAngleFact(string Pid1, string Pid2, string Pid3, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Pid1 = Pid1;
        this.Pid2 = Pid2;
        this.Pid3 = Pid3;

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static RightAngleFact parseFact(Scroll.ScrollFact fact)
    {
        OMA tp = (OMA)((Scroll.ScrollSymbolFact)fact).tp;
        if (tp == null)
            return null;

        string Point1Uri = "";
        string Point2Uri = "";
        string Point3Uri = "";

        string uri = fact.@ref.uri;
        OMA proof_OMA = (OMA)((Scroll.ScrollSymbolFact)fact).tp; // proof DED
        OMA rightAngleOMA = (OMA)proof_OMA.arguments[0]; // rightAngle OMA

        if (rightAngleOMA.arguments[0] is OMS)
        {
            Point1Uri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[0]).uri;
            Point2Uri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[1]).uri;
            Point3Uri = ((OMS)((OMA)((OMA)((Scroll.ScrollSymbolFact)fact).tp).arguments[0]).arguments[2]).uri;
        }

        if (StageStatic.stage.factState.ContainsKey(Point1Uri)
         && StageStatic.stage.factState.ContainsKey(Point2Uri)
         && StageStatic.stage.factState.ContainsKey(Point3Uri))

            return new RightAngleFact(Point1Uri, Point2Uri, Point3Uri, uri, StageStatic.stage.factState);

        else    //If dependent facts do not exist return null
            return null;
    }


    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return _Facts[Pid1].Label + _Facts[Pid2].Label + _Facts[Pid3].Label + "⊥";
    }


    /// <summary>
    /// Constructs struct for not-right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="p1URI"> Uri for <see cref="Pid1"/></param>
    /// <param name="p2URI"> Uri for <see cref="Pid2"/></param>
    /// <param name="p3URI"> Uri for <see cref="Pid3"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateMMTDeclaration(string p1URI, string p2URI, string p3URI)
    {
        List<MMTTerm> innerArguments = new List<MMTTerm>
        {
            new OMS(p1URI),
            new OMS(p2URI),
            new OMS(p3URI)
        };

        List<MMTTerm> outerArguments = new List<MMTTerm>
        {
            new OMA(new OMS(MMTURIs.RightAngle), innerArguments)
        };
        MMTTerm tp = new OMA(new OMS(MMTURIs.Ded), outerArguments);
        MMTTerm df = null;

        return new MMTSymbolDeclaration(this.Label, tp, df);
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
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid1].Label;
        obj.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid2].Label;
        obj.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Pid3].Label;


        obj.GetComponent<FactWrapper>().fact = this;
        return obj;
    }

    /// \copydoc Fact.GetHashCode
    /// uhhh is this a problem?
    public override int GetHashCode()
    {
        return this.Pid1.GetHashCode() ^ this.Pid2.GetHashCode() ^ this.Pid3.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(RightAngleFact f1, RightAngleFact f2)
    {
        if (f1.Pid1 == f2.Pid1 && f1.Pid2 == f2.Pid2 && f1.Pid3 == f2.Pid3)
            return true;

        PointFact p1f1 = (PointFact)_Facts[f1.Pid1];
        PointFact p2f1 = (PointFact)_Facts[f1.Pid2];
        PointFact p3f1 = (PointFact)_Facts[f1.Pid3];

        PointFact p1f2 = (PointFact)_Facts[f2.Pid1];
        PointFact p2f2 = (PointFact)_Facts[f2.Pid2];
        PointFact p3f2 = (PointFact)_Facts[f2.Pid3];

      


        return (p1f1.Equivalent(p1f2) && p2f1.Equivalent(p2f2) && p3f1.Equivalent(p3f2) );

    }
}




/// <summary>
/// The volume of a cylinder defined by a base area  <see cref="CircleFact">CircleFact</see>, a top area <see cref="CircleFact">CircleFact</see> and the volume as float
/// </summary>
public class CylinderVolumeFact : FactWrappedCRTP<CylinderVolumeFact>
{
    ///  <summary> a <see cref="CircleFact">CircleFact</see> describing the base area </summary>
    public string Cid1;
    ///  <summary> a <see cref="CircleFact">CircleFact</see> describing the top area  </summary>
    public string Cid2;
    ///  <summary> the volume of the cylinder as a float </summary>
    public float vol;
    ///  <summary> OMA proof that the two circles are parallel  </summary>
    public OMA proof;

    /// <summary> \copydoc Fact.Fact </summary>
    public CylinderVolumeFact() : base()
    {
        this.Cid1 = null;
        this.Cid2 = null;
        this.vol = 0.0f;
        this.proof = null;

    }

    /// <summary>
    /// Copies <paramref name="fact"/> by initiating new MMT %Fact.
    /// </summary>
    /// <param name="fact">Fact to be copied</param>
    /// <param name="old_to_new"><c>Dictionary</c> mapping <paramref name="fact"/>.<see cref="getDependentFactIds"/> in <paramref name="fact"/>.<see cref="Fact._Facts"/> to corresponding <see cref="Fact.Id"/> in <paramref name="organizer"/> </param>
    /// <param name="organizer">sets <see cref="_Facts"/></param>
    public CylinderVolumeFact(CylinderVolumeFact fact, Dictionary<string, string> old_to_new, FactOrganizer organizer) : base(fact, organizer)
    {
        init(old_to_new[fact.Cid1], old_to_new[fact.Cid2], fact.vol, fact.proof);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="cid2">sets <see cref="Cid2"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    /// <param name="proof">sets <see cref="proof"/></param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public CylinderVolumeFact(string cid1, string cid2, float vol, OMA proof, FactOrganizer organizer) : base(organizer)
    {
        init(cid1, cid2, vol, proof);
    }

    /// <summary>
    /// sets variables and generates MMT Declaration
    /// </summary>
    /// <param name="cid1">sets <see cref="Cid1"/></param>
    /// <param name="cid2">sets <see cref="Cid2"/></param>
    /// <param name="vol">sets <see cref="vol"/></param>
    /// <param name="proof">sets <see cref="proof"/></param>
    private void init(string cid1, string cid2, float vol, OMA proof)
    {
        this.Cid1 = cid1;
        this.Cid2 = cid2;
        this.proof = proof;

        CircleFact cf1 = _Facts[cid1] as CircleFact;
        CircleFact cf2 = _Facts[cid2] as CircleFact;
        this.vol = vol;


        MMTDeclaration mmtDecl;
        string c1URI = cf1.Id;
        string c2URI = cf2.Id;


        mmtDecl = generateMMTDeclaration(c1URI, c2URI, vol, proof);

        AddFactResponse.sendAdd(mmtDecl, out this._URI);
    }

    /// <summary>
    /// Bypasses initialization of new MMT %Fact by using existend URI, _which is not checked for existence_.
    /// </summary>
    /// <param name="Cid1">sets <see cref="Cid1"/></param>
    /// <param name="Cid2">sets <see cref="Cid2"/></param>
    /// <param name="volume">sets <see cref="vol"/></param>
    /// <param name="proof">sets <see cref="proof"/></param>
    /// <param name="backendURI">MMT URI</param>
    /// <param name="organizer">sets <see cref="Fact._Facts"/></param>
    public CylinderVolumeFact(string Cid1, string Cid2, float volume, OMA proof, string backendURI, FactOrganizer organizer) : base(organizer)
    {
        this.Cid1 = Cid1;
        this.Cid2 = Cid2;
        this.vol = volume;
        this.proof = proof;

        this._URI = backendURI;
        _ = this.Label;
    }

    /// \copydoc Fact.parseFact(Scroll.ScrollFact)
    public new static CylinderVolumeFact parseFact(Scroll.ScrollFact fact)
    {
        string uri = fact.@ref.uri;

        string Circle1Uri = ((OMS)((OMA)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).arguments[0]).uri;
        string Circle2Uri = ((OMS)((OMA)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).arguments[1]).uri;
        float volume = ((OMF)((Scroll.ScrollValueFact)fact).value).f;

        OMA proof = (OMA)(((OMA)((OMA)((Scroll.ScrollValueFact)fact).lhs).arguments[0]).arguments[2]);

        if (StageStatic.stage.factState.ContainsKey(Circle1Uri) && StageStatic.stage.factState.ContainsKey(Circle2Uri))

            return new CylinderVolumeFact(Circle1Uri, Circle2Uri, volume, proof, uri, StageStatic.stage.factState);

        else    //If dependent facts do not exist return null
            return null;
    }

    /// \copydoc Fact.generateLabel
    protected override string generateLabel()
    {
        return "V(" + _Facts[Cid1].Label + "," + _Facts[Cid2].Label + ")";
    }



    /// <summary>
    /// Constructs struct for not-right-angled MMT %Fact <see cref="AddFactResponse"/>
    /// </summary>
    /// <param name="c1URI"> Uri for <see cref="Cid1"/></param>
    /// <param name="c2URI"> Uri for <see cref="Cid2"/></param>
    /// <param name="val"> <see cref="vol"/></param>
    /// <returns>struct for <see cref="AddFactResponse"/></returns>
    private MMTDeclaration generateMMTDeclaration(string c1URI, string c2URI, float val, OMA proof)
    {
        MMTTerm lhs =
            new OMA(
                new OMS(MMTURIs.CylinderVolume),

                new List<MMTTerm> {
                    new OMS(c1URI),
                    new OMS(c2URI),
                    proof,
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
        return new string[] { Cid1, Cid2 };
    }

    /// \copydoc Fact.instantiateDisplay(GameObject, Transform)
    public override GameObject instantiateDisplay(GameObject prefab, Transform transform)
    {
        var obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _Facts[this.Cid1].Label + _Facts[this.Cid2].Label;
        obj.GetComponent<FactWrapper>().fact = this;

        return obj;
    }

    /// \copydoc Fact.GetHashCode
    /// uhhh is this a problem?
    public override int GetHashCode()
    {
        return this.Cid1.GetHashCode() ^ this.Cid2.GetHashCode();
    }

    /// \copydoc Fact.Equivalent(Fact, Fact)
    protected override bool EquivalentWrapped(CylinderVolumeFact f1, CylinderVolumeFact f2)
    {
        if (f1.Cid1 == f2.Cid1 && f1.Cid2 == f2.Cid2)
            return true;

        CircleFact c1f1 = (CircleFact)_Facts[f1.Cid1];
        CircleFact c1f2 = (CircleFact)_Facts[f2.Cid1];

        CircleFact c2f1 = (CircleFact)_Facts[f1.Cid2];
        CircleFact c2f2 = (CircleFact)_Facts[f2.Cid2];

        return (c1f1.Equivalent(c1f2) && c2f1.Equivalent(c2f2) && f1.vol == f2.vol);

    }
}



