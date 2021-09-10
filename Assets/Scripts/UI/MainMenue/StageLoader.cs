using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StageLoader : ListLoader<Stage>
{
    public EditLoader editLoader;
    public Color Accomplished;
    public Color NotYetAccomplished;

    public bool local;
    protected string
        NoId = "---",
        NoStage = "-----",
        NoDescr = "No Entry found, please check directory!",
        NoLocal = "No local Stage\nClick [+]";

    public override void Init()
    {
        StageStatic.SetStage("", local);
        StageStatic.ShallowLoadStages();

        Dictionary<string, Stage> dict = local ? StageStatic.StageLocal : StageStatic.StageOfficial;
        ListButtons(dict.Values.OrderByDescending((v) => v.number).ToList());

        scroll.verticalScrollbar.numberOfSteps = dict.Count;
    }

    protected override void Default()
    {
        var def = Instantiate(Entry);
        def.transform.SetParent(List.transform, false);

        def.GetNthChild(new List<int> { 0, 0, 0 }).GetComponent<TMPro.TextMeshProUGUI>().text = NoId;
        def.GetNthChild(new List<int> { 1, 0, 0 }).GetComponent<TMPro.TextMeshProUGUI>().text = NoStage;
        def.GetNthChild(new List<int> { 2, 0, 0 }).GetComponent<TMPro.TextMeshProUGUI>().text = local ? NoLocal:NoDescr;
    }

    protected override void ListButtonsWrapped(List<Stage> list)
    {
        foreach (var stage in list)
        {
            GameObject prefab = Instantiate(Entry);

            prefab.transform.SetParent(List.transform, false);
            prefab.transform.SetAsFirstSibling();

            // TODO: handle unable to load
            prefab.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                StageStatic.devel = false;
                Loader.LoadStage(stage.name, local, true);
            });

            prefab.GetNthChild(new List<int> { 0, 0, 0 }).GetComponent<TMPro.TextMeshProUGUI>().text = stage.number.ToString();
            prefab.GetNthChild(new List<int> { 1, 0, 0 }).GetComponent<TMPro.TextMeshProUGUI>().text = stage.name;
            prefab.GetNthChild(new List<int> { 2, 0, 0 }).GetComponent<TMPro.TextMeshProUGUI>().text = stage.description;

            prefab.GetNthChild(new List<int> { 3, 0 }).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate
            {
                pageMenue.SetMode(3);
                editLoader.SetStage(stage.name, !stage.use_install_folder);
            });

            prefab.GetNthChild(new List<int> { 0, 0 }).GetComponent<UnityEngine.UI.Image>().color = stage.completed_once ? Accomplished : NotYetAccomplished;
        }
    }
}
