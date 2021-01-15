using UnityEngine;
using UnityEngine.UI;
using static GlobalSettings;

public class ImageHintAnimation : MonoBehaviour
{
    public Image imageToChange;
    public Color imageToChangeDefaultColor { get; set; }

    private Color animationStartColor;
    private Color animationEndColor;
    
    private float animateDuration;
    private bool animating = false;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (imageToChange != null)
            imageToChangeDefaultColor = imageToChange.color;

        updateAnimationParameters();
    }

    // Update is called once per frame
    void Update()
    {
        if (animating)
        {
            this.timer += Time.deltaTime;
            Animate();
        }
    }

    public void AnimationTrigger()
    {
        if (imageToChange != null)
        {
            updateAnimationParameters();
            animating = true;
        }
    }

    public void ResetAnimation()
    {
        if (imageToChange != null)
        {
            Reset();
        }
    }

    private void Animate()
    {
        if (timer >= animateDuration)
        {
            Reset();
        }
        else
        {
            imageToChange.color = Color.Lerp(animationStartColor, animationEndColor, Mathf.PingPong(Time.time, 1));
        }

    }

    private void Reset()
    {
        animating = false;
        timer = 0;
        imageToChange.color = imageToChangeDefaultColor;
    }

    private void updateAnimationParameters() {
        animationStartColor = globalSettings.hintAnimationStartColor;
        animationEndColor = globalSettings.hintAnimationEndColor;
        animateDuration = globalSettings.hintAnimationDuration;
    }
}
