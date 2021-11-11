using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StageStatic;

/// <summary>
/// Attached to prefab Def_Stage
/// </summary>
public class StageBehaviour : MonoBehaviour
{
    /// <summary>
    /// Re-loads <see cref="StageStatic.stage"/> to  display all <see cref="Fact">Facts</see>.
    /// </summary>
    void Start()
    {
        StageStatic.LoadInitStage(true /*StageStatic.stage.player_record.solved*/, gameObject);
    }

    /// <summary>
    /// Resets changes made by <see cref="StageStatic.stage"/> and frees ressources.
    /// </summary>
    private void OnDestroy()
    {
        StageStatic.SetMode(Mode.Play); // no Mode.Create
        StageStatic.stage.solution.hardreset();
        StageStatic.stage.factState.hardreset();
    }

    /// <summary>
    /// Wrapps <see cref="SetMode(Mode, GameObject)"/>. Needed as endpoint for unity buttons.
    /// </summary>
    /// <param name="create"><c>SetMode(create ? Mode.Create : Mode.Play);</c></param>
    public void SetMode(bool create)
    {
        SetMode(create ? Mode.Create : Mode.Play);
    }

    /// <summary>
    /// Wrapps <see cref="StageStatic.SetMode(Mode, GameObject)"/>. Defaulting <paramref name="obj"/> to <see cref="this.gameObject"/>.
    /// </summary>
    /// \copydetails StageStatic.SetMode(Mode, GameObject)
    public void SetMode(Mode mode, GameObject obj = null)
    {
        obj ??= gameObject;
        StageStatic.SetMode(mode, obj);
    }
}
