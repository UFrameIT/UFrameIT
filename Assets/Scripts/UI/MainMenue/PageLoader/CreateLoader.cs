using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLoader : MenueLoader
{
    public TMPro.TMP_InputField Category;
    public TMPro.TMP_InputField Id;
    public TMPro.TMP_InputField Name;
    public TMPro.TMP_InputField Description;
    public TMPro.TMP_Dropdown WorldDropdown;

    public GameObject Messenger;

    protected string category
    {
        get { return Category.text.Trim(); }
        set { Category.text = value; }
    }
    protected int id { 
        get { return Id.text.Length == 0 ? StageStatic.NextNumber(true) : int.Parse(Id.text); }
        set { Id.text = value.ToString(); }
    }
    protected new string name { 
        get { return Name.text.Trim(); }
        set { Name.text = value; }
    }
    protected string description { 
        get { return Description.text; }
        set { Description.text = value; }
    }
    protected string scene { 
        get { 
            return WorldDropdown.value < StageStatic.Worlds.Count ?
                StageStatic.Worlds[WorldDropdown.value] : invalid_world;
        }
        set {
            if (!StageStatic.Worlds.Contains(value)) {
                //WorldDropdown.AddOptions(new List<string> { invalid_world });
                WorldDropdown.value = StageStatic.Worlds.Count;
            } else
                WorldDropdown.value = StageStatic.Worlds.IndexOf(value);
        }
    }


    protected string invalid_world = "Invalid";


    protected void OnEnable()
    {
        Init();
    }

    protected new void Start()
    {
        scroll.verticalScrollbar.numberOfSteps = 0;
        scroll.verticalNormalizedPosition = 1f;
    }

    public void Init()
    {
        WorldDropdown.ClearOptions();
        WorldDropdown.AddOptions(StageStatic.Worlds);
    }

    public void Create()
    { 
        var error = StageStatic.LoadNewStage(category, id, name, description, scene);
        if (!error.pass) {
            Error(error);
            return;
        }
    }

    protected void Error(StageStatic.StageErrorStruct error)
    {
        //TODO: inform failure & why?
        if (error.category)
        {
            throw new System.NotImplementedException("Invalid Category"); // unused
        } 
        if (error.id)
        {
            throw new System.NotImplementedException("Id must be unique within a category"); // technichal a lie
        }
        if (error.name) // for savegame identification (could be chained with local/category/id)
        {
            throw new System.NotImplementedException("Name must be unique");
        }
        if (error.description)
        {
            throw new System.NotImplementedException("Invalid Description"); // unused
        }
        if (error.scene)
        {
            throw new System.NotImplementedException("Please select a World");
        }
        if (error.local) // keep custom levels custom
        {
            throw new System.NotImplementedException("Not allowed to create or change non-local Stages"); 
        }
        if (error.load)
        {
            throw new System.NotImplementedException("Unknown error: Stage could not be loaded"); // needs work
        }
    }
}
