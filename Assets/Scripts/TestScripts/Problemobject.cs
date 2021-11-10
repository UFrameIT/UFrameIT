using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problemobject : MonoBehaviour
{ 
    public Fact problemFact; // which kind of fact should be calculated
    public Fact goAroundPoint; // Pointfact the NPC should spawn and go around
    public List<Scroll> neededScrolls; //  scrolls needed for solving the problem
    public string[] neededGadgets; // gadgets needed for solving the problem
    public string npcProblemText; // text shown while talking to npc
    public string npcSuccessText; // text shown when successfully solving the problem
    public string npcFailureText; // text shown at failure
    // public GameObject scrollobject;
    public GameObject taskCharakter;
    public TaskCharakterAnimation taskcharakter; // npc
    public GameObject successReaction; // gamereaction at success (e.g. fireworks)
    public GameObject failureReaction; // gamereaction at failure (e.g. rain)
    public string npcReaction; // npc reaction at success (e.g. circlerun)
    public RuntimeAnimatorController npcController; //controller which specifies the npc succes reaction
    // public GameObject myPrefab;


    //public Problemobject()
    //{

    //}

    // Start is called before the first frame update
    void Start()
    {
        //successReaction = (GameObject)Resources.Load("Prefabs/Fireworks_Animation", typeof(GameObject));


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Creating snap zones for the input tree
    /*void CreateSnapZones(GameObject tree)
    {
        //float treeHeight = tree.GetComponent<Collider>().bounds.size.y;
        //Idee: maintree prefab erstellen mit maßen des eingegebenen trees -> alten tree löschen (Destroy(Gameobject)) -> prefab umbenennen in ursprünglichen tree
        // 1. merke namen von scrollobject,
        // 2. merke position und maße von scrollobject
        // 3. lösche scrollobject
        // 4. erstelle prefab an der stelle vom scrollobject und mit den maßen und nenne es so wie das alte scrollobject
        string name = scrollobject.name;

        float xWert = scrollobject.transform.position.x;
        float yWert = scrollobject.transform.position.y;
        float zWert = scrollobject.transform.position.z;

        float xScale = scrollobject.transform.localScale.x;
        float yScale = scrollobject.transform.localScale.y;
        float zScale = scrollobject.transform.localScale.z;

        Destroy(scrollobject);


        GameObject problemtree = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        problemtree.name = name;
        problemtree.transform.position = new Vector3(xWert, yWert, zWert);
        problemtree.transform.localScale = new Vector3(xScale, yScale, zScale);
    }*/

    void CreateProblem()
    {

    }

    
}
