using UnityEngine;

public class ScrollUIManager : MonoBehaviour
{
    public bool initialShow;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (initialShow)
            Show();
        else
            Hide();
    }

    public void Show(float delay = 0f)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide(float delay = 0f)
    {
        canvasGroup.alpha = 0f; //this makes everything transparent
        canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
    }
}
