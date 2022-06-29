using UnityEngine;
using static UIconfig;

public class LoadingCircle : MonoBehaviour
{
    private RectTransform rectComponent;
    public float rotateSpeed = -200f;
    private float width=100;
    private float height=100;
    public float scale;
    

    private void Start()
    {
        rectComponent = GetComponent<RectTransform>();
        
    }

    private void Update()
    {
        rectComponent.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        if (scale != 0)
        {
            width = screWidth / scale;
            height = screHeight / scale;
            if (width < height)
            {
                width = height;
            }
            else
            {
                height = width;
            }
            rectComponent.sizeDelta = new Vector2(width, height);
        }

        
    }
}