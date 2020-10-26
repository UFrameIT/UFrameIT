using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using static JSONManager;
using JsonSubTypes;
using Newtonsoft.Json;


/*
[Serializable]
public class Declaration
{
    public string name;
    public string isProof;
    public string value;
    public string identifier;
    public string description;
}


[Serializable]
public class Scroll
{
    public string problemTheory;
    public string solutionTheory;
    public string label;
    public string description;
    public Declaration[] declarations;
    //public string output;

    public static Scroll generateFromJson(string json) {
       return JsonUtility.FromJson<Scroll>(json);
    }

    public string toJson() {
        return JsonUtility.ToJson(this);
    } 
}
*/
public class Scroll : LightScroll
{
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


    //also collects occurences to be changed later
    public static string ParseString(int stringId, string raw, List<ScrollFact> scrollFacts)
    {
       
        int i = 0;
        while(true)
        {
            char[] rawChars = raw.ToCharArray();
            i = Array.FindIndex(rawChars,i, c => c == '$');
            if (i == -1) break;
           // Debug.Log(i);

            //id of Fact, specified after the $
            int toChange = (int) Char.GetNumericValue(raw[i + 1]);

            //label of this Fact
            string label = scrollFacts[toChange].label;

            raw = raw.Remove(i,2);
            raw = raw.Insert(i,label);

            if(FactOccurences[toChange] == null)
            {
                FactOccurences[toChange] = new List<KeyValuePair<int, int>>();
            }
            //if (toChange >= 0) Debug.Log(stringId);
            FactOccurences[toChange].Add(new KeyValuePair<int, int>(stringId,i));

        
        }

        return raw;
    }

  



/*
    public class ScrollList
    {
        public List<Scroll> scrolls;
       
    }*/

    public class ScrollAssignment
    {
       public KeyValuePair<string, JSONManager.MMTTerm> assignment;
    }

    public class FilledScroll
    {
        LightScroll scroll;
        List<ScrollAssignment> assignments;

        public FilledScroll(LightScroll scroll, List<ScrollAssignment> assignments)
        {
            this.scroll = scroll;
            this.assignments = assignments;
        }
    }


    [JsonConverter(typeof(JsonSubtypes), "kind")]
    [JsonSubtypes.KnownSubType(typeof(ScrollSymbolFact), "general")]
    [JsonSubtypes.KnownSubType(typeof(ScrollValueFact), "veq")]
    public class ScrollFact
    {
        public string kind;
        public UriReference @ref;
        public string label;
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
    }

    /**
    * Class used for deserializing incoming value-declarations from mmt
    */
    public class ScrollValueFact : ScrollFact
    {
        MMTTerm lhs;
        MMTTerm valueTp;
        MMTTerm value;
        MMTTerm proof;
    }

}

public class LightScroll
{
    public ScrollTheoryReference @ref;
}

public class ScrollTheoryReference
{
    public string problemTheory;
    public string solutionTheory;
}





