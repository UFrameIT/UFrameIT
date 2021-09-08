using UnityEngine;

public class GlobalBehaviour : MonoBehaviour
{
    //Make sure when using RGBA-Colors, the A-value of animationStartColor 
    //and animationEndColor is the same OR try with value = 255
    public static Color hintAnimationStartColor;
    public static Color hintAnimationEndColor;
    public static float hintAnimationDuration;

    void Awake()
    {
        StageStatic.ShallowLoadStages();
    }
}
