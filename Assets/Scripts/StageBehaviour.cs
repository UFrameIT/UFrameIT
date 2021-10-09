using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StageStatic;

public class StageBehaviour : MonoBehaviour
{
    void Start()
    {
        StageStatic.LoadInitStage(true /*StageStatic.stage.player_record.solved*/, gameObject);
    }

    private void OnDestroy()
    {
        StageStatic.SetMode(Mode.Play); // no Mode.Create
        StageStatic.stage.solution.hardreset();
        StageStatic.stage.factState.hardreset();
    }

    // needed as endpoint for unity buttons
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
