using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using static CommunicationEvents;

public class DisplayFacts : MonoBehaviour
{
    public Dictionary<Type, GameObject> prefabDictionary;

    public Dictionary<string, GameObject> displayedFacts = new Dictionary<string, GameObject>();

    public Transform factscreenContent;

    [Header("FactPrefabs")]
    public GameObject prefab_Point;
    public GameObject prefab_Distance;
    public GameObject prefab_Angle;
    public GameObject prefab_Default;
    public GameObject prefab_OnLine;
    public GameObject prefab_Line;
    public GameObject prefab_ParallelLineFact;
    public GameObject prefab_RectangleFact;
    public GameObject prefab_RadiusFact;
    public GameObject prefab_AreaCircle;
    public GameObject prefab_ConeVolume;
    public GameObject prefab_OrthogonalCircleLine;
    public GameObject prefab_TruncatedConeVolume;
    public GameObject prefab_RightAngle;
    public GameObject prefab_CylinderVolume;



    public GameObject prefab_CircleFact;
    public GameObject prefab_OnCircleFact;
    public GameObject prefab_AngleCircleLineFact;

    [Header("Visualisation")]
    public int x_Start;
    public int y_Start;
    public int X_Pacece_Between_Items;
    public int y_Pacece_Between_Items;
    public int number_of_Column;

    //Start is called before the first frame update
    void Start()
    {
        prefabDictionary = new Dictionary<Type, GameObject>() {
            {typeof(PointFact), prefab_Point},
            {typeof(LineFact), prefab_Distance},
            {typeof(RayFact), prefab_Line},
            {typeof(AngleFact), prefab_Angle},
            {typeof(OnLineFact), prefab_OnLine},
            {typeof(ParallelLineFact), prefab_ParallelLineFact},

            {typeof(CircleFact), prefab_CircleFact},
            {typeof(OnCircleFact), prefab_OnCircleFact},
            {typeof(AngleCircleLineFact), prefab_AngleCircleLineFact},
            {typeof(RadiusFact), prefab_RadiusFact},
            {typeof(AreaCircleFact), prefab_AreaCircle},
            {typeof(ConeVolumeFact), prefab_ConeVolume},
            {typeof(OrthogonalCircleLineFact), prefab_OrthogonalCircleLine },
            {typeof(TruncatedConeVolumeFact), prefab_TruncatedConeVolume },
            {typeof(RightAngleFact), prefab_RightAngle },
            {typeof(CylinderVolumeFact), prefab_CylinderVolume},
            



        };

        var rect = GetComponent<RectTransform>();
        x_Start = (int)(rect.rect.x + X_Pacece_Between_Items * .5f);
        y_Start = (int)(-y_Pacece_Between_Items * .5f);
        number_of_Column = Mathf.Max(1, (int)(rect.rect.width / prefab_Point.GetComponent<RectTransform>().rect.width) - 1);

        AddFactEvent.AddListener(AddFact);
        RemoveFactEvent.AddListener(RemoveFact);
        AnimateExistingFactEvent.AddListener(AnimateFact);
    }

    public void AddFact(Fact fact) {
        var fid = fact.Id;
        var obj = CreateDisplay(transform, fact);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.localPosition = GetPosition(displayedFacts.Count);
        displayedFacts.Add(fact.Id, obj);
        AdjustFactscreenContentHeight();
    }

    public void RemoveFact(Fact fact)
    {
        GameObject.Destroy(displayedFacts[fact.Id]);
        displayedFacts.Remove(fact.Id);
        UpdatePositions();
        AdjustFactscreenContentHeight();
    }

    public void UpdatePositions()
    {
        int i = 0;
        foreach (var element in displayedFacts)
            element.Value.GetComponent<RectTransform>().localPosition = GetPosition(i++);
    }

    public void AnimateFact(Fact fact) {
        var factIcon = displayedFacts[fact.Id];
        factIcon.GetComponentInChildren<ImageHintAnimation>().AnimationTrigger();
    }

    private GameObject CreateDisplay(Transform transform, Fact fact)
    {
        return fact.instantiateDisplay(prefabDictionary[fact.GetType()], factscreenContent);
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(x_Start + (X_Pacece_Between_Items * (i % number_of_Column)), y_Start + (-y_Pacece_Between_Items * (i / number_of_Column)), 0f);
    }

    private void AdjustFactscreenContentHeight()
    {
        var rect = factscreenContent.GetComponent<RectTransform>();
        var height = (float)(y_Pacece_Between_Items * Math.Ceiling(((float)displayedFacts.Count / number_of_Column)));
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
    }
}
