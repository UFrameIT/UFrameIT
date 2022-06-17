using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollClickedScript : MonoBehaviour, IPointerDownHandler
{
    public Scroll scroll;
    public GameObject DetailScreen;

    public void OnPointerDown(PointerEventData eventData)
    {
        this.DetailScreen.GetComponent<ScrollDetails>().setScroll(this.scroll);
    }
}
