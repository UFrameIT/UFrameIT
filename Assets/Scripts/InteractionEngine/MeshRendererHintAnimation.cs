using UnityEngine;
using static GlobalSettings;

public class MeshRendererHintAnimation : MonoBehaviour
{
    public MeshRenderer meshRendererToChange;
    private Color meshRendererToChangeDefaultColor;
    
    private Color animationStartColor;
    private Color animationEndColor;

    private float animateDuration;
    private bool animating = false;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (meshRendererToChange != null)
            meshRendererToChangeDefaultColor = meshRendererToChange.material.color;

        updateAnimationParameters();
    }

    // Update is called once per frame
    void Update()
    {
        if (animating) {
            this.timer += Time.deltaTime;
            Animate();
        }
    }

    public void AnimationTrigger() {
        if (meshRendererToChange != null)
        {
            updateAnimationParameters();
            animating = true;
        }
    }

    private void Animate() {
        if (timer >= animateDuration)
        {
            animating = false;
            timer = 0;
            meshRendererToChange.material.color = meshRendererToChangeDefaultColor;
        }
        else {
            meshRendererToChange.material.color = Color.Lerp(animationStartColor, animationEndColor, Mathf.PingPong(Time.time, 1));
        }
    }

    private void updateAnimationParameters()
    {
        animationStartColor = globalSettings.hintAnimationStartColor;
        animationEndColor = globalSettings.hintAnimationEndColor;
        animateDuration = globalSettings.hintAnimationDuration;
    }
}
