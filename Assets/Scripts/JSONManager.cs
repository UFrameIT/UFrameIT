using JsonSubTypes;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JSONManager 
{

    public static string URIPrefix = "http://mathhub.info/MitM/core/geometry?3DGeometry?";

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
            this.uri = URIPrefix + uri;
        }
    }


    public class OMF : MMTTerm
    {
        [JsonProperty("float")]
        public float f;
        public string kind = "OMF";

        public OMF(float f)
        {
            this.f = f;
        }
    }


    class DeclarationBody : MMTTerm
    {
        MMTTerm original;
        MMTTerm simplified;
        string kind = "O/S";
    }


    public class MMTDeclaration
    {
        public string label;
        public MMTTerm tp;
        public MMTTerm df;
    }

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
