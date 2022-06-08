using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;
using JsonSubTypes;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class MMTURICollection
{
    public string Point = "http://mathhub.info/MitM/core/geometry?3DGeometry?point";
    public string Tuple = "http://gl.mathhub.info/MMT/LFX/Sigma?Symbols?Tuple";
    public string LineType = "http://mathhub.info/MitM/core/geometry?Geometry/Common?line_type";
    public string LineOf = "http://mathhub.info/MitM/core/geometry?Geometry/Common?lineOf";

    public string OnLine = "http://mathhub.info/MitM/core/geometry?Geometry/Common?onLine";
    public string Ded = "http://mathhub.info/MitM/Foundation?Logic?ded";
    public string Eq = "http://mathhub.info/MitM/Foundation?Logic?eq";
    public string Metric = "http://mathhub.info/MitM/core/geometry?Geometry/Common?metric";
    public string Angle = "http://mathhub.info/MitM/core/geometry?Geometry/Common?angle_between";
    public string Sketch = "http://mathhub.info/MitM/Foundation?InformalProofs?proofsketch";
    public string RealLit = "http://mathhub.info/MitM/Foundation?RealLiterals?real_lit";
    
    public string ParallelLine = "http://mathhub.info/MitM/core/geometry?Geometry/Common?parallelLine";
    // public string RectangleFact = "http://mathhub.info/FrameIT/frameworld?FrameITRectangles?rectangleType";
    //  public string RectangleFactmk = "http://mathhub.info/FrameIT/frameworld?FrameITRectangles?mkRectangle";

    public string CircleType3d = "http://mathhub.info/FrameIT/frameworld?FrameITCircle?circleType3D";
    public string MkCircle3d = "http://mathhub.info/FrameIT/frameworld?FrameITCircle?circle3D";
    public string TriangleMiddlePoint = "http://mathhub.info/FrameIT/frameworld?FrameITTriangles?triangleMidPointWrapper";
    public string RadiusCircleMetric = "http://mathhub.info/FrameIT/frameworld?FrameITCircle?circleRadius";

    public string AreaCircle = "http://mathhub.info/FrameIT/frameworld?FrameITCircle?areaCircle";
    public string VolumeCone = "http://mathhub.info/FrameIT/frameworld?FrameITCone?volumeCone";
    public string ConeOfCircleApex = "http://mathhub.info/FrameIT/frameworld?FrameITCone?circleConeOf";


    public string ParametrizedPlane = "http://mathhub.info/MitM/core/geometry?Geometry/Planes?ParametrizedPlane";
    public string pointNormalPlane = "http://mathhub.info/MitM/core/geometry?Geometry/Planes?pointNormalPlane";
    public string OnCircle =       "http://mathhub.info/FrameIT/frameworld?FrameITCircle?pointOnCircle";
    public string AnglePlaneLine = "http://mathhub.info/FrameIT/frameworld?FrameITCircle?angleCircleLine";
    public string OrthoCircleLine ="http://mathhub.info/FrameIT/frameworld?FrameITCircle?orthogonalCircleLine";


    public string TruncatedVolumeCone = "http://mathhub.info/FrameIT/frameworld?FrameITCone?truncatedConeVolume";
    public string CylinderVolume = "http://mathhub.info/FrameIT/frameworld?FrameITCylinder?cylinderVolume";

    public string ParallelCircles = "http://mathhub.info/FrameIT/frameworld?FrameITCone?parallelCircles";
    public string RightAngle = "http://mathhub.info/FrameIT/frameworld?FrameITBasics?rightAngle";



}

public static class JSONManager 
{
    //could init the strings of MMTURIs with JSON or other settings file instead
    public static MMTURICollection MMTURIs = new MMTURICollection();

    public class URI
    {
        public string uri;

        public URI(string uri)
        {
            this.uri = uri;
        }
    }
    
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
        public float f;
        public string kind = "OMF";

        public OMF(float f)
        {
            this.f = f;
        }
    }

    public class MMTDeclaration
    {
        public string label;
        public static MMTDeclaration FromJson(string json)
        {
            MMTDeclaration mmtDecl = JsonConvert.DeserializeObject<MMTDeclaration>(json);
            if (mmtDecl.label == null)
                mmtDecl.label = string.Empty;

            return mmtDecl;
        }
        public static string ToJson(MMTDeclaration mmtDecl)
        {
            if (mmtDecl.label == null)
                mmtDecl.label = string.Empty;

            string json = JsonConvert.SerializeObject(mmtDecl);
            return json;
        }
    }

    /**
     * MMTSymbolDeclaration: Class for facts without values, e.g. Points
     */ 
    public class MMTSymbolDeclaration : MMTDeclaration
    {
        public string kind = "general";
        public MMTTerm tp;
        public MMTTerm df;

        /**
         * Constructor used for sending new declarations to mmt
         */
        public MMTSymbolDeclaration(string label, MMTTerm tp, MMTTerm df)
        {
            this.label = label;
            this.tp = tp;
            this.df = df;
        }
    }

    /**
     * MMTValueDeclaration: Class for facts with values, e.g. Distances or Angles
     */
    public class MMTValueDeclaration : MMTDeclaration
    {
        public string kind = "veq";
        public MMTTerm lhs;
        public MMTTerm valueTp;
        public MMTTerm value;

        /**
         * Constructor used for sending new declarations to mmt
         */
        public MMTValueDeclaration(string label, MMTTerm lhs, MMTTerm valueTp, MMTTerm value)
        {
            this.label = label;
            this.lhs = lhs;
            this.valueTp = valueTp;
            this.value = value;
        }
    }


    // TODO? /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>

    /// <summary>
    /// Writes the given object instance to a Json file, recursively to set depth, including all members.
    /// <para>Object type must have a parameterless constructor.</para>
    /// <para>Only All properties and variables will be written to the file. These can be any type though, even other non-abstract classes.</para>
    /// </summary>
    /// <param name="filePath">The file path to write the object instance to.</param>
    /// <param name="objectToWrite">The object instance to write to the file.</param>
    /// <param name="max_depth">The depth recursion will occur. Default = 0.</param>
    public static void WriteToJsonFile(string filePath, object objectToWrite, int max_depth = 0)
    {
        int current_depth = 0;

        // This tells your serializer that multiple references are okay.
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        BindingFlags bindFlags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static;

        TextWriter writer = null;
        try
        {
            string payload = RecursiveStep(objectToWrite);
            writer = new StreamWriter(filePath);
            writer.Write(payload);
        }
        finally
        {
            if (writer != null)
                writer.Close();
        }


        // ======= local methods ======= 
        // TODO? more stable depths (see next todo)
        // TODO? respect IgnoreJson tags

        string RecursiveStep<S>(S objectToWrite) where S : new()
        {
            string json;

            if (current_depth >= max_depth 
             || Type.GetTypeCode(objectToWrite.GetType()) != TypeCode.Object
             || objectToWrite == null)
                json = JsonConvert.SerializeObject(objectToWrite, settings/*, new JsonInheritenceConverter<object>()*/);
            else
            {
                current_depth++;
                json = IntrusiveRecursiveJsonGenerator(objectToWrite);
                current_depth--;
            }

            return json;
        }

        string IntrusiveRecursiveJsonGenerator<S>(S objectToWrite) where S : new()
        // private convention? more like private suggestion!
        {
            bool is_enum = IsEnumerableType(objectToWrite.GetType());

            string json = is_enum ? "[" : "{";
            foreach (object field in is_enum ? (objectToWrite as IEnumerable) : objectToWrite.GetType().GetFields(bindFlags))
            {
                object not_so_private;
                if (is_enum)
                {
                    not_so_private = field;
                }
                else
                {
                    not_so_private = ((FieldInfo)field).GetValue(objectToWrite);
                    json += ((FieldInfo)field).Name + ":";
                }

                json += RecursiveStep(not_so_private);

                json += ",";
            }
            json = json.TrimEnd(',') + (is_enum ? "]" : "}");

            return json;


            // ======= local methods ======= 

            bool IsEnumerableType(Type type)
            {
                if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return true;

                foreach (Type intType in type.GetInterfaces())
                {
                    if (intType.IsGenericType
                        && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    /// <summary>
    /// Reads an object instance from an Json file.
    /// <para>Object type must have a parameterless constructor.</para>
    /// </summary>
    /// <typeparam name="T">The type of object to read from the file.</typeparam>
    /// <param name="filePath">The file path to read the object instance from.</param>
    /// <returns>Returns a new instance of the object read from the Json file.</returns>
    public static T ReadFromJsonFile<T>(string filePath) where T : new()
    {
        TextReader reader = null;
        try
        {
            reader = new StreamReader(filePath);
            var fileContents = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(fileContents/*, new JsonInheritenceConverter<object>()*/);
        }
        finally
        {
            if (reader != null)
                reader.Close();
        }
    }

    // tutorial @https://www.codeproject.com/Articles/1201466/Working-with-JSON-in-Csharp-VB#data_structure_types
    // unused
    // TODO: check for actual type in ReadJson
    // TODO: avoid self-referencing-loop-error in WriteJson
    public sealed class JsonInheritenceConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            var element = jo.Properties().First();
            return element.Value.ToObject(Type.GetType(element.Name));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {            
            if (value == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName(value.GetType().FullName);
            serializer.Serialize(writer, value);
            writer.WriteEndObject();
        }
    }
}
