using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using static JSONManager;
using JsonSubTypes;
using Newtonsoft.Json;

public class Scroll
{
    public ScrollTheoryReference @ref;
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

    // id of fact, positions in Description
    public static List<KeyValuePair<int,int>>[] FactOccurences;

    public static void InitDynamicScroll(int n)
    {
        FactOccurences = new List<KeyValuePair<int, int>>[n];
    }

    public class FilledScroll
    {
        public ScrollTheoryReference scroll;
        //public List<List<KeyValuePair<JSONManager.URI, JSONManager.MMTTerm>>> assignments;
        public List<List<System.Object>> assignments;

        public FilledScroll(ScrollTheoryReference scroll, List<List<System.Object>> assignments)
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
    }

    public class UriReference
    {
        public string uri;
    }

    /**
    * Class used for deserializing incoming symbol-declarations from mmt
    */
    public class ScrollSymbolFact : ScrollFact
    {
        public MMTTerm tp;
        public MMTTerm df;
        
        public override String getType() {
            if (this.tp is OMS)
                return ((OMS)this.tp).uri;
            else
                return null;
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
    }

    public class ScrollDynamicInfo
    {
        public Scroll original;
        public Scroll rendered;
        //public List<List<ScrollAssignmentsToBeImplemented>> completions
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





