using UnityEngine;
using UnityEngine.UI;
using static GlobalSettings;

public class ImageHintAnimation : MonoBehaviour
{
    public Image imageToChange;
    private Color imageToChangeDefaultColor;

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

        animationStartColor = globalSettings.hintAnimationStartColor;
        animationEndColor = globalSettings.hintAnimationEndColor;
        animateDuration = globalSettings.hintAnimationDuration;
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
            animating = true;
    }

    private void Animate()
    {
        if (timer >= animateDuration)
        {
            animating = false;
            timer = 0;
            imageToChange.color = imageToChangeDefaultColor;
        }
        else
        {
            imageToChange.color = Color.Lerp(animationStartColor, animationEndColor, Mathf.PingPong(Time.time, 1));
        }

    }
}
