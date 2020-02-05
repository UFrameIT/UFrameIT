using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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

