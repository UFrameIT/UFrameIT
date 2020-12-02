using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings globalSettings;

    //Make sure when using RGBA-Colors, the A-value of animationStartColor 
    //and animationEndColor is the same OR try with value = 255
    public Color hintAnimationStartColor;
    public Color hintAnimationEndColor;
    public float hintAnimationDuration;

    void Awake()
    {
        if (globalSettings != null)
            GameObject.Destroy(globalSettings);
        else
            globalSettings = this;

        DontDestroyOnLoad(this);
    }
}
