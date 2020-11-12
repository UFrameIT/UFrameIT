using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RenderedScrollFact : MonoBehaviour
{

    public int ID;
    public TextMeshProUGUI LabelMesh;
    private string _label;
    public string factUri;

    public string Label
    {
        get { return _label; }
        set
        {
            if (_label == value) return;
            _label = value;
            LabelMesh.text = value;
        }
    }

}
