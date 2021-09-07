using UnityEngine;
using static GlobalStatic;

public class GlobalBehaviour : MonoBehaviour
{
    public static GlobalBehaviour globalSettings;

    //Make sure when using RGBA-Colors, the A-value of animationStartColor 
    //and animationEndColor is the same OR try with value = 255
    public static Color hintAnimationStartColor;
    public static Color hintAnimationEndColor;
    public static float hintAnimationDuration;

    void Awake()
    {
        GlobalStatic.ShallowLoadStages();
    }

    public static void SetMode(bool create)
    {
        SetMode(create ? Mode.Create : Mode.Play);
    }

    public static void SetMode(Mode mode)
    {
        GlobalStatic.SetMode(mode);
    }
}
