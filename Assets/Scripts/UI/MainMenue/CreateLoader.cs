using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLoader : MenueLoader
{
    public TMPro.TMP_InputField Id;
    public TMPro.TMP_InputField Name;
    public TMPro.TMP_InputField Description;
    public TMPro.TMP_Dropdown WorldDropdown;

    public GameObject Messenger;

    private const string ending = ".unity";

    private List<string> Scenes = new List<string>();

    private void OnEnable()
    {
        Init();
    }

    private void Start()
    {
        scroll.verticalScrollbar.numberOfSteps = 0;
        scroll.verticalNormalizedPosition = 1f;
    }

    public void Init()
    {
        //TODO? Update list at buildtime

        List<TMPro.TMP_Dropdown.OptionData> Worlds = new List<TMPro.TMP_Dropdown.OptionData>();
        Scenes.Clear();

        string world = "World";
        foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                string name = new System.IO.FileInfo(scene.path).Name;
                name = name.Substring(0, name.Length - ending.Length);

                if (0 == string.Compare(name, name.Length - world.Length, world, 0, world.Length))
                {
                    Scenes.Add(scene.path);
                    Worlds.Add(new TMPro.TMP_Dropdown.OptionData(name));
                }
            }
        }

        WorldDropdown.ClearOptions();
        WorldDropdown.AddOptions(Worlds);
    }

    public void Create()
    {
        string
            name = Name.text.Trim(),
            decr = Description.text.Trim(),
            scen = Scenes[WorldDropdown.value];

        int id = Id.text.Length == 0 ? StageStatic.NextNumber(true) : int.Parse(Id.text);

        if (StageStatic.LoadNewStage(id, name, decr, scen))
            return;

        //TODO: inform failure & why?
    }
}
