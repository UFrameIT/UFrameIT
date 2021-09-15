using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditLoader : CreateLoader
{
    private Stage original_stage;

    public new void Init()
    {
        base.Init();
    }

    public void SetStage(string name, bool local)
    {
        StageStatic.SetStage(name, local);
        original_stage = StageStatic.stage;

        if (!original_stage.DeepLoad())
            Error(StageStatic.StageErrorStruct.NotLoadable);

        category = original_stage.category;
        id = original_stage.number;
        this.name = original_stage.name;
        description= original_stage.description;
        scene = original_stage.scene;
    }

    public void Reset()
    {
        SetStage(original_stage.name, !original_stage.use_install_folder);
    }

    private void _Delete()
    {
        StageStatic.Delete(original_stage);
    }

    public void Delete()
    {
        if (original_stage.use_install_folder)
        {
            Error(StageStatic.StageErrorStruct.InvalidFolder);
            return;
        }
        //Reset();
        //TODO: ask user
        _Delete();
        pageMenue.RevertMode();
    }

    private bool _Clone(bool overwrite)
    {
        var error = StageStatic.Validate(category, id, name, description, scene);
        if (overwrite) {
            if(name == original_stage.name)
                error.name = false;
            if(id == original_stage.number)
                error.id = false;
        }
        if (!error.pass) {
            Error(error);
            return false;
        }

        Stage new_stage = new Stage(original_stage, category, id, name, description, scene, true);
        if (!overwrite)
            new_stage.ResetSaves();

        StageStatic.stage = new_stage;
        return true;
    }

    public void Clone()
    {
        if (!_Clone(false))
            return;

        StageStatic.SetStage(name, true);
        StageStatic.LoadCreate();
    }

    private bool _Save()
    {
        if (!_Clone(true))
            return false;

        if (name != original_stage.name)
        // has not been overridden
            _Delete();

        return true;
    }

    public void Save()
    {
        if (original_stage.use_install_folder)
        {
            Error(StageStatic.StageErrorStruct.InvalidFolder);
            return;
        }

        if (!_Save())
            return;

        pageMenue.RevertMode();
    }

    public void Edit()
    {
        if (original_stage.use_install_folder)
        {
            Error(StageStatic.StageErrorStruct.InvalidFolder);
            return;
        }

        _Save();

        StageStatic.SetStage(name, true);
        StageStatic.LoadCreate();
    }
}
