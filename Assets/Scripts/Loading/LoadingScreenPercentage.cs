using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached in the Loading-Screen to <see cref="TMPro"/> object for progress display.
/// Loads set <see cref="Loader.nextscene"/> and updates progress in <see cref="TMPro.TextMeshProUGUI"/>.
/// </summary>
public class LoadingScreenPercentage : MonoBehaviour
{
    public float maxFrameIncrease = 100f / 100;
    private float currentValue;

    void Start()
    {
        currentValue = 0f;
        Loader.LoaderCallback();
    }

    void Update()
    {
        UpdateText();
    }

    private void OnDestroy()
    {
        Loader.PostLoad();
    }

    private void UpdateText()
    {
        currentValue += maxFrameIncrease;
        currentValue = currentValue > Loader.progress ? Loader.progress : currentValue;

        gameObject.GetComponent<TMPro.TextMeshProUGUI>().text =
            string.Format("{0,2:D}%", (int)(currentValue * 100));
    }
}
