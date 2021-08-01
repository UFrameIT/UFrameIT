using UnityEngine;

public class ScrollClickedScript : MonoBehaviour
{
    public Scroll scroll;
    public GameObject DetailScreen;

    public void onClick()
    {
        this.DetailScreen.GetComponent<ScrollDetails>().setScroll(this.scroll);
    }
}
