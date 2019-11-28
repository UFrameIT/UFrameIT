using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistanceDisplayScript : MonoBehaviour
{

    public LengthObject fact;
    public TextMeshProUGUI PointOne;
    public TextMeshProUGUI PointTwo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PointOne.text = fact.PointA;
        PointTwo.text = fact.PointB;
    }

    void setfact(LengthObject f){
        this.fact = f;
    }
}
