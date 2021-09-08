using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StageStatic;

public class StageBehaviour : MonoBehaviour
{
    void Awake()
    {
        StageStatic.LoadInitStage(true, gameObject);
    }

    private void OnDestroy()
    {
        StageStatic.devel = false;
    }

    public void SetDevel(bool devel)
    {
        StageStatic.devel = devel;
        gameObject.UpdateTagActive("DevelopingMode", devel);
    }

    public void SetMode(bool create)
    {
        SetMode(create ? Mode.Create : Mode.Play);
    }

    public void SetMode(Mode mode, GameObject obj = null)
    {
        obj ??= gameObject;
        StageStatic.SetMode(mode, obj);
    }
}
