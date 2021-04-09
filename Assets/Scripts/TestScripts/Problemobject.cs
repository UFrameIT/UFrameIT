using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problemobject : MonoBehaviour
{ 
    public GameObject scrollobject;
    public TaskCharakterAnimation taskcharakter;
    public GameObject successReaction;
    public GameObject failureReaction;
    public GameObject myPrefab;


    // Start is called before the first frame update
    void Start()
    {
        //successReaction = (GameObject)Resources.Load("Prefabs/Fireworks_Animation", typeof(GameObject));

        CreateSnapZones(scrollobject);
        Debug.Log("snapzones are created");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Creating snap zones for the input tree
    void CreateSnapZones(GameObject tree)
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

        



    }

    
}
