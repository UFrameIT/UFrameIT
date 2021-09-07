using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenueLoader : MonoBehaviour
{
    public UnityEngine.UI.Slider slider;
    public UnityEngine.UI.Scrollbar scroll;
    public GameObject Entry;
    public GameObject EntryBuffer;

    private MenueBarSlider selector;
    private int mode;

    private List<string> mode_string = new List<string>
    {
        "official",
        "local",
    };

    void Start()
    {
        selector = slider.GetComponent<MenueBarSlider>();
        mode = -1;
        Clear();
    }

    void Update()
    {
        if (mode == selector.last_active_slot)
            return;

        mode = selector.last_active_slot;
        GlobalStatic.SetStage("", mode == 1);
        GlobalStatic.ShallowLoadStages();

        Dictionary<string, Stage> dict = mode == 0 ? GlobalStatic.StageOfficial : GlobalStatic.StageLocal;
        ListButtons(dict);

        scroll.numberOfSteps = dict.Count;
        scroll.value = 1f;
    }

    private void Clear()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
            Destroy(gameObject.transform.GetChild(i).gameObject);
    }

    private void Default()
    {
        Instantiate(Entry).transform.SetParent(gameObject.transform, false);
        Instantiate(EntryBuffer).transform.SetParent(gameObject.transform, false);
    }

    private void ListButtons(Dictionary<string, Stage> dict)
    {
        Clear();

        if (dict.Count == 0)
        {
            Default();
            return;
        }

        Instantiate(EntryBuffer).transform.SetParent(gameObject.transform, false);

        var list = mode == 0 ?
            dict.Values.OrderByDescending((v) => v.number) :
            dict.Values.OrderBy((v) => v.number);

        Destroy(gameObject.transform.GetChild(0));

        foreach(var stage in list)
        {
            GameObject prefab = Instantiate(Entry);

            prefab.transform.SetParent(gameObject.transform, false);
            prefab.transform.SetAsFirstSibling();

            var bar = prefab.transform.GetChild(0);
            WriteInChildText(bar.GetChild(0).gameObject, stage.number.ToString());
            WriteInChildText(bar.GetChild(1).gameObject, stage.name);
            WriteInChildText(bar.GetChild(2).gameObject, stage.description);

            // TODO: handle unable to load
            prefab.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { Loader.LoadStage(stage.name, mode == 1); });
        }

        void WriteInChildText(GameObject entry, string content)
        {
            if (content == null)
                return;

            entry.transform.GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = content;
        }
    }
}
