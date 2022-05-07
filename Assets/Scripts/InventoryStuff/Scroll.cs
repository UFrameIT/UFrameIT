using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static JSONManager;

public class Scroll
{
    public string @ref;
    public string label;
    public string description;
    public List<ScrollFact> requiredFacts;
    public List<ScrollFact> acquiredFacts;

    public static List<Scroll> FromJSON(string json)
    {
        List<Scroll> scrolls = JsonConvert.DeserializeObject<List<Scroll>>(json);
        return scrolls;
    }
    public static string ToJSON(FilledScroll scroll)
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(scroll);
        return json;
    }

    public class FilledScroll
    {
        public string scroll;
        public List<ScrollAssignment> assignments;

        public FilledScroll(string scroll, List<ScrollAssignment> assignments)
        {
            this.scroll = scroll;
            this.assignments = assignments;
        }
    }

    public class ScrollTheoryReference
    {
        public string problemTheory;
        public string solutionTheory;
    }

    [JsonConverter(typeof(JsonSubtypes), "kind")]
    [JsonSubtypes.KnownSubType(typeof(ScrollSymbolFact), "general")]
    [JsonSubtypes.KnownSubType(typeof(ScrollValueFact), "veq")]
    public abstract class ScrollFact
    {
        public string kind;
        public UriReference @ref;
        public string label;

        public abstract String getType();

        public abstract String getApplicant();
    }

    public class UriReference
    {
        public string uri;

        public UriReference(string uri)
        {
            this.uri = uri;
        }
    }

    /**
    * Class used for deserializing incoming symbol-declarations from mmt
    */
    public class ScrollSymbolFact : ScrollFact
    {
        public MMTTerm tp;
        public MMTTerm df;

        public override String getType()
        {
            if (this.tp is OMS)
                return ((OMS)this.tp).uri;
            else if (this.tp is OMA)
                return ((OMS)((OMA)((OMA)this.tp).arguments[0]).applicant).uri;
            else
                return null;
        }

        public override String getApplicant()
        {
            //Debug.Log(" Check " + this.tp is OMS + " and " + this.tp is OMA + " and " + this.tp is OMSTR + " or " + this.tp is OMF);
            // return ((OMS)((OMA)((OMA)this.tp).arguments[0]).arguments[0]).uri;
            if (this.df is OMA && ((OMA)this.df).applicant is OMS)
                return ((OMS)((OMA)this.df).applicant).uri;
            

            return null ;// ((OMS)((OMA)((OMA)this.df).arguments[0]).applicant).uri;

        }
    }

    /**
    * Class used for deserializing incoming value-declarations from mmt
    */
    public class ScrollValueFact : ScrollFact
    {
        public MMTTerm lhs;
        public MMTTerm valueTp;
        public MMTTerm value;
        public MMTTerm proof;

        public override String getType()
        {
            if (this.lhs is OMA & (((OMA)this.lhs).applicant is OMS))
                return ((OMS)((OMA)this.lhs).applicant).uri;
            else
                return null;
        }
        public override String getApplicant()
        {
            // TODO Test this 
            if (this.lhs is OMA & (((OMA)this.lhs).applicant is OMS))
                return ((OMS)((OMA)this.lhs).applicant).uri;
            return null; 
        }

    }

    public class ScrollAssignment
    {
        public UriReference fact;
        public OMS assignment;
    }

    public class ScrollApplicationInfo
    {
        public Boolean valid;
        public ScrollApplicationCheckingError[] errors;
        public List<Scroll.ScrollFact> acquiredFacts;
    }

    public class ScrollDynamicInfo
    {
        public Scroll original;
        public Scroll rendered;
        public List<List<Scroll.ScrollAssignment>> completions;
        public Boolean valid;
        public ScrollApplicationCheckingError[] errors;
    }

    public class ScrollApplicationCheckingError
    {
        public String kind;
        public string msg;
        public MMTTerm fact;
    }

}





