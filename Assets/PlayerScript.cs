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
        fact.pointA = "A";
        fact.pointB = "B";
        fact.Lenght = 1.0;
        fact.Description = "LengthFact";
        inventory.AddFact(fact);
    }


    public void AddAngleFact(){
        AngleObject fact = ScriptableObject.CreateInstance<AngleObject>();
        fact.pointA = "X";
        fact.pointB = "Y";
        fact.pointC = "Z";
        fact.angle = 90.0;
        fact.Description = "AngleFact";
        inventory.AddFact(fact);
    }

    public void AddPointFact(){
        PointObject fact = ScriptableObject.CreateInstance<PointObject>();
        fact.point = "P";
        inventory.AddFact(fact);
    }

    private void OnApplicationQuit()
    {
        inventory.Facts.Clear();
        for(int i = 0; i < inventory.Scrolls.Count; i++){
            inventory.Scrolls[i].isDisplayed = false;
        }

    }
}
