
using System.Collections;
using UnityEngine;
using UnityEditor;

/* ############################
 * ## Show Size Editor Addon ##
 * ## This script reports  ##
 * ## the size of a object  ##
 * ## in x,y and z axis'.  ##
 * ## All work by Eric5h5  ##
 * ##forum.unity3d.com/threa ##
 * ## ds/object-size.19991  ##
 * ## C# conversion and warn ##
 * ##ing fix by Nsomnia of  ##
 * ## BlenderTek.com and last##
 * ## function by justinlloyd##
 * ############################ */

class ShowSize : EditorWindow
{
    [MenuItem("Window/ShowSize")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        //Next line replaces old statment causing a warning in Unity 4.6 used to be "ShowSize sizeWindow = new ShowSize();"
        ShowSize sizeWindow = ScriptableObject.CreateInstance(typeof(ShowSize)) as ShowSize;
        sizeWindow.autoRepaintOnSceneChange = true;
        sizeWindow.Show();
    }

    void OnGUI()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject thisObject = Selection.activeObject as GameObject;
            if (thisObject == null)
            {
                return;
            }

            MeshFilter mf = thisObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
            if (mf == null)
            { return; }

            Mesh mesh = mf.sharedMesh;
            if (mesh == null)
            { return; }

            Vector3 size = mesh.bounds.size;
            Vector3 scale = thisObject.transform.localScale;

            GUILayout.Label("Size\n  X: " + size.x * scale.x + "\n  Y: " + size.y * scale.y + "\n  Z: " + size.z * scale.z + "");
        }

        if (!LaserPointer.laserBool)
            return;

        RaycastHit laserHit = LaserPointer.laserHit;
        GUILayout.Label("Laser Pointer:\n  " + laserHit.transform.position.ToString() + "\n\n");
    }

    void Update()
    {
        Repaint();
    }
}

