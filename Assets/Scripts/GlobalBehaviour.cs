using UnityEngine;

public class GlobalBehaviour : MonoBehaviour
{
    //Make sure when using RGBA-Colors, the A-value of animationStartColor 
    //and animationEndColor is the same OR try with value = 255
    public static Color hintAnimationStartColor;
    public static Color hintAnimationEndColor;
    public static float hintAnimationDuration;

    public static Color StageAccomplished;
    public static Color StageNotYetAccomplished;
    public static Color StageError;

    [SerializeField]
    private Color _hintAnimationStartColor;
    [SerializeField]
    private Color _hintAnimationEndColor;
    [SerializeField]
    private float _hintAnimationDuration;

    [SerializeField]
    private Color _StageAccomplished;
    [SerializeField]
    private Color _StageNotYetAccomplished;
    [SerializeField]
    private Color _StageError;

    void Awake()
    {
        //GenerateDemoFiles.GenerateAll();

        hintAnimationStartColor = _hintAnimationStartColor;
        hintAnimationEndColor = _hintAnimationEndColor;
        hintAnimationDuration = _hintAnimationDuration;

        StageAccomplished = _StageAccomplished;
        StageNotYetAccomplished = _StageNotYetAccomplished;
        StageError = _StageError;


        StageStatic.ShallowLoadStages();
        //DontDestroyOnLoad(gameObject);
    }
}
