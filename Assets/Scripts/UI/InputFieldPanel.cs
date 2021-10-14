using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Just adds a " " to the end of <see cref="TMPro"/> input-field-text-box. <br/>
/// This fixes some graphical glitches, occuring when typing a [newline].
/// </summary>
public class InputFieldPanel : MonoBehaviour
{
    private TMPro.TextMeshProUGUI output;
    private TMPro.TMP_InputField input;

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
