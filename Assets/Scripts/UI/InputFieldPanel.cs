using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldPanel : MonoBehaviour
{
    private TMPro.TextMeshProUGUI output;
    private TMPro.TMP_InputField input;
    private string newline;

    void Start()
    {
        output = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        input = gameObject.GetComponentInChildren<TMPro.TMP_InputField>();
    }

    private void Update()
    {
        Fix();
    }

    public void Fix()
    {
        if (input == null || output == null)
            Start();

        output.text = input.text + " ";
    }
}
