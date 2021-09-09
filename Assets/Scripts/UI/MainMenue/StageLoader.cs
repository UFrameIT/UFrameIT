using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: SE: Split for Stage/Local
public class StageLoader : MenueLoader
{
    public GameObject StageList;
    public GameObject Entry;

    protected bool local = false;

    private string NoOfficial = "No Entry found, please check directory!";
    private string NoLocal = "No local Stage\nClick [+]";

    private void OnEnable()
    {
        Init(mode == 1);
    }

    private void Start()
    {
        scroll.verticalNormalizedPosition = 1f;
    }

    private void OnDisable()
    {
        Clear();
    }

    public void Init(bool local)
    {
        Clear();

        this.local = local;

        StageStatic.SetStage("", local);
        StageStatic.ShallowLoadStages();

        Dictionary<string, Stage> dict = local ? StageStatic.StageLocal : StageStatic.StageOfficial;
        ListStageButtons(dict);

        scroll.verticalScrollbar.numberOfSteps = dict.Count;
    }

    private void Clear()
    {
        for (int i = 0; i < StageList.transform.childCount; i++)
            Destroy(StageList.transform.GetChild(i).gameObject);
    }

    private void Default()
    {
        var def = Instantiate(Entry);
        def.transform.SetParent(StageList.transform, false);

        string message = local ? NoLocal : NoOfficial;
        WriteInChildText(def.transform.GetChild(2).gameObject, message);
    }

    private void ListStageButtons(Dictionary<string, Stage> dict)
    {
        if (dict.Count == 0)
        {
            Default();
            return;
        }

        var list = local ?
            dict.Values.OrderBy((v) => v.number) :
            dict.Values.OrderByDescending((v) => v.number);

        foreach (var stage in list)
        {
            GameObject prefab = Instantiate(Entry);

            prefab.transform.SetParent(StageList.transform, false);
            prefab.transform.SetAsFirstSibling();

            WriteInChildText(prefab.transform.GetChild(0).gameObject, stage.number.ToString());
            WriteInChildText(prefab.transform.GetChild(1).gameObject, stage.name);
            WriteInChildText(prefab.transform.GetChild(2).gameObject, stage.description);

            // TODO: handle unable to load
            prefab.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { Loader.LoadStage(stage.name, mode == 1, true); });
        }
    }

    private void WriteInChildText(GameObject entry, string content)
    {
        if (content == null)
            return;

        entry.transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = content;
    }

}
