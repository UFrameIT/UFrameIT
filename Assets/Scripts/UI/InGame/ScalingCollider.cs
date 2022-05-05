using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;
//using static StreamingAssetLoader;
//using static CheckServer;
using static CommunicationEvents;
using static UIconfig;
using UnityEngine.EventSystems;
using static Restart;
using static SceneSwitcher;
using System;

public class ScalingCollider: MonoBehaviour
{


    private double scale;
    public double ColliderRadius;
    public int Collidersize_ID;
    public bool use_ColliderRadius;

    public SphereCollider myCollider;





    void Start()
    {
        scalingCollider();
    }

    private void Update()
    {
        
    }


    private void scalingCollider()
    {
        switch (Opsys)
        {
            case 1:
                scale=colliderScale_all* UIconfig.colliderScale_Mobile_default; ;
                break;
            case 0:

                
                scale = colliderScale_all * UIconfig.colliderScale_PC_default;
                break;
            default:
                scale = colliderScale_all * UIconfig.colliderScale_PC_default;
                break;

            

        }

        if (use_ColliderRadius)
        {
             scale=scale * ColliderRadius;
        }
        else
        {

            if (CheckArray())
            {
                scale =  scale * colliderScale_Obj_array[Collidersize_ID];
            }

        }
        myCollider.radius = Convert.ToSingle(scale);
        print("Scale "+scale);


    }

    private bool CheckArray()
    {
        if (Collidersize_ID >= 0 && Collidersize_ID < UIconfig.colliderScale_Obj_array.Length)
        {
            return true;
        }
        return false;
    }
    //public static double colliderScale_ = 1; //Script
    //public static double colliderScale_PC_default = 1;
    //public static double colliderScale_Mobile_default = 5;
    //public static double colliderScale_Tree = 0.5;









}