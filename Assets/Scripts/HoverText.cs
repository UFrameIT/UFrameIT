using UnityEngine;
using UnityEngine.EventSystems;

public class HoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Transform TextTransform;
    // Start is called before the first frame update
    void Start()
    {
        TextTransform = transform.GetChild(0);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Debug.Log("hover");
        TextTransform.gameObject.SetActive(true);
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        TextTransform.gameObject.SetActive(false);
    }


}
