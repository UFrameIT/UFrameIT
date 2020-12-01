using UnityEngine;

public class MeshRendererHintAnimation : MonoBehaviour
{
    public MeshRenderer meshRendererToChange;
    private Color meshRendererToChangeDefaultColor;
    //Make sure when using RGBA-Colors, the A-value of animationStartColor 
    //and animationEndColor is the same OR try with value = 255
    public Color animationStartColor;
    public Color animationEndColor;

    public float animateDuration;
    private bool animating = false;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (meshRendererToChange != null)
            meshRendererToChangeDefaultColor = meshRendererToChange.material.color;
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
            animating = true;
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
}
