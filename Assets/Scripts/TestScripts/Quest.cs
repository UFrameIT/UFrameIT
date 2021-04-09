using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using static Scroll;

public class Quest : MonoBehaviour
{
    public GameObject questobject;
    public Scroll appliedScroll;
    //public List<Fact> requiredFacts;

    // Start is called before the first frame update
    void Start()
    {
        //GetHeight(questobject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    float GetHeight(GameObject questobject)
    {
        float height = questobject.GetComponent<Collider>().bounds.size.y;
        //float scale = questobject.transform.localScale.y;
        Debug.Log("Height is "+height);
        return height;
    }

    void CheckRequiredFacts()
    {
       // List<ScrollFact> requiredFacts = appliedScroll.requiredFacts;
       foreach (Scroll.ScrollFact fact in appliedScroll.requiredFacts)
       {
            CheckSingleFact(fact);
       }
       

    }

    void CheckSingleFact(Scroll.ScrollFact fact)
    {

    }

    // 1 if gameobject = tree_01 -> return 1 usw; 

    string CheckQuestObject()
    { 

        string meshname = questobject.GetComponent<MeshFilter>().mesh.name;
       
        if (meshname.Contains("Tree_01"))
        {
            return "Tree_01";
        } else if(meshname.Contains("Tree_02"))
        {
            return "Tree_02";
        }else 
            return "no Tree";


    }



    
}
