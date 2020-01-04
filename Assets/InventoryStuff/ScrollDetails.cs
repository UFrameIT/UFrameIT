using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollDetails : MonoBehaviour
{

    public Scroll scroll;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setScroll(Scroll s) {
        this.scroll = s;
        Debug.Log(s);
    }
}
