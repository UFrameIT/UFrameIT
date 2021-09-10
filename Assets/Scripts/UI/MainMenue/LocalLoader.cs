using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: SE: Split for Stage/Local
public class LocalLoader : ListLoader<Stage>
{
    public EditLoader editLoader;

    protected bool local = true;
    private string NoLocal = "No local Stage\nClick [+]";

    public override void Init()
    {
        StageStatic.SetStage("", local);
        StageStatic.ShallowLoadStages();

        Dictionary<string, Stage> dict = local ? StageStatic.StageLocal : StageStatic.StageOfficial;
        ListButtons(dict.Values.OrderBy((v) => v.number).ToList());

        scroll.verticalScrollbar.numberOfSteps = dict.Count;
    }

    protected override void Default()
    {
        var def = Instantiate(Entry);
        def.transform.SetParent(List.transform, false);

        WriteInChildText(def.transform.GetChild(2).gameObject, NoLocal);
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

            WriteInChildText(prefab.transform.GetChild(0).gameObject, stage.number.ToString());
            WriteInChildText(prefab.transform.GetChild(1).gameObject, stage.name);
            WriteInChildText(prefab.transform.GetChild(2).gameObject, stage.description);

            prefab.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate
            {
                pageMenue.SetMode(3);
                editLoader.SetStage(stage.name, !stage.use_install_folder);
            });
        }
    }
}
