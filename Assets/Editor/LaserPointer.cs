using System.Collections;
using UnityEngine;
using UnityEditor;

public class LaserPointer : Editor 
{
    static public RaycastHit laserHit;
    static public bool laserBool;

    [ExecuteInEditMode]
    static void Init()
    {
        laserBool = false;
    }

    void OnSceneGUI()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = Camera.current.ScreenPointToRay(mousePosition);
        RaycastHit tempHit;

        if (!(laserBool = Physics.Raycast(ray, out tempHit, Mathf.Infinity, int.MaxValue)))
            return ;

        laserHit = tempHit;
    }

}
