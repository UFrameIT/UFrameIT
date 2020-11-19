using TMPro;
using UnityEngine;
using System.Collections.Generic;
using static CommunicationEvents;

public class RenderedScrollFact : MonoBehaviour
{

    public int ID;
    public TextMeshProUGUI LabelMesh;
    private string _label;
    public string factUri;

    private Color defaultColor = new Color(255,255,255,135);
    private Color highlightColor = new Color(255,0,0,135);

    public GameObject ScrollParameterObject;

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

    void Start()
    {
        HintAvailableEvent.AddListener(OnHintAvailable);
    }

    public void OnClickHintButton() {
        CompletionsHintEvent.Invoke(this.ScrollParameterObject, factUri);
    }

    public void OnHintAvailable(List<string> uris) {
        UnityEngine.UI.Button button = ScrollParameterObject.GetComponentInChildren<UnityEngine.UI.Button>();
        UnityEngine.UI.ColorBlock buttonColor = button.colors;

        if (uris.Contains(factUri))
        {
            buttonColor.normalColor = highlightColor;
            button.colors = buttonColor;
        }
        else {
            buttonColor.normalColor = defaultColor;
            button.colors = buttonColor;
        }
    }
}
