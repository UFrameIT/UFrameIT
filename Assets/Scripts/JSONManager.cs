using System;
using JsonSubTypes;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MMTURICollection
{
    public string Point = "http://mathhub.info/MitM/core/geometry?3DGeometry?point";
    public string Tuple = "http://gl.mathhub.info/MMT/LFX/Sigma?Symbols?Tuple";
    public string Line_type = "http://mathhub.info/MitM/core/geometry?Geometry?Common?line_type";
    public string LineOf = "http://mathhub.info/MitM/core/geometry?Geometry?Common?lineOf";
    public string Ded = "http://mathhub.info/MitM/Foundation?Logic?ded";
    public string Eq = "http://mathhub.info/MitM/Foundation?Logic?eq";
    public string Metric = "http://mathhub.info/MitM/core/geometry?Geometry/Common?metric";
    public string Angle = "http://mathhub.info/MitM/core/geometry?Geometry/Common?angle_between";
    public string Sketch = "http://mathhub.info/MitM/Foundation?InformalProofs?proofsketch";
}

public static class JSONManager 
{
    //could init the strings of MMTURIs with JSON or other settings file instead
    public static MMTURICollection MMTURIs = new MMTURICollection();
    
    [JsonConverter(typeof(JsonSubtypes), "kind")]
    public class MMTTerm
    {
        string kind;
    }

    public class OMA : MMTTerm
    {
        public MMTTerm applicant;
        public List<MMTTerm> arguments;
        public string kind = "OMA";
        public OMA(MMTTerm applicant, List<MMTTerm> arguments)
        {
            this.applicant = applicant;
            this.arguments = arguments;
        }
    }

    public class OMS : MMTTerm
    {
        public string uri;
        public string kind = "OMS";

        public OMS(string uri)
        {
            this.uri = uri;
        }
    }

    public class OMSTR : MMTTerm
    {
        [JsonProperty("float")]
        public string s;
        public string kind = "OMSTR";

        public OMSTR(string s)
        {
            this.s = s;
        }
    }


    public class OMF : MMTTerm
    {
        [JsonProperty("float")]
        public decimal f;
        public string kind = "OMF";

        public OMF(float f)
        {
            this.f = Convert.ToDecimal(f);
        }
    }

    public class MMTDeclaration
    {
        public static MMTDeclaration FromJson(string json)
        {
            MMTDeclaration mmtDecl = JsonConvert.DeserializeObject<MMTDeclaration>(json);
            return mmtDecl;
        }
        public static string ToJson(MMTDeclaration mmtDecl)
        {
            string json = JsonConvert.SerializeObject(mmtDecl);
            return json;
        }
    }

    /**
     * MMTSymbolDeclaration: Class for facts without values, e.g. Points
    **/ 
    public class MMTSymbolDeclaration : MMTDeclaration
    {
        public string kind = "general";
        public string label;
        public MMTTerm tp;
        public MMTTerm df;

        public MMTSymbolDeclaration(string label, MMTTerm tp, MMTTerm df)
        {
            this.label = label;
            this.tp = tp;
            this.df = df;
        }
    }

    /**
     * MMTValueDeclaration: Class for facts with values, e.g. Distances or Angles
    **/
    public class MMTValueDeclaration : MMTDeclaration
    {
        public string kind = "veq";
        public string label;
        public MMTTerm lhs;
        public MMTTerm valueTp;
        public MMTTerm value;

        public MMTValueDeclaration(string label, MMTTerm lhs, MMTTerm valueTp, MMTTerm value)
        {
            this.label = label;
            this.lhs = lhs;
            this.valueTp = valueTp;
            this.value = value;
        }
    }

}
