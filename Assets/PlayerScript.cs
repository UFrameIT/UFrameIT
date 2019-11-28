using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Inventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddLenghtFact()
    {
        LengthObject fact =  ScriptableObject.CreateInstance<LengthObject>();
        fact.PointA = "A";
        fact.PointB = "B";
        fact.Lenght = 1.0;
        fact.Description = "LengthFact";
        inventory.AddFact(fact);
    }

    public void AddDemoScroll(){
        DefaultScroll scroll =  ScriptableObject.CreateInstance<DefaultScroll>();
        scroll.Description = "Dis is Scroll";
        inventory.AddScroll(scroll);
    }

    

    private void OnApplicationQuit()
    {
        inventory.Facts.Clear();
    }
}
