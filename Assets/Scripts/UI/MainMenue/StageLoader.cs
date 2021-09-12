using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StageLoader : ListLoader<Stage>
{
    public GameObject EntryBody;
    public GameObject EntryTime;

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

        Dictionary<string, Stage> dict = local ? 
            StageStatic.StageLocal : StageStatic.StageOfficial;

        var ord = local ? 
            dict.Values.OrderBy(s => s.category) :
            dict.Values.OrderBy(s => StageStatic.Category.ContainsKey(s.category) ? StageStatic.Category[s.category] : -1);

        ord = ord.ThenBy(s => s.number);

        ListButtons(ord.ToList());

        //scroll.verticalScrollbar.numberOfSteps = dict.Count;
    }

    protected override void Default()
    {
        var def = Instantiate(EntryHeader);
        CreateEntry(def, List, NoId, NoStage, local ? NoLocal:NoDescr, new Color(0,0,0,0));
    }

    protected override void ListButtonsWrapped(List<Stage> list)
    {
        string current_category = "";
        GameObject category_entry = null;
        float
            cat_sum = 0,
            cat_suc = 0;

        foreach (var stage in list)
        {
            if(current_category != stage.category || category_entry == null)
            {
                PopulateCategory();

                current_category = stage.category;
                category_entry = Instantiate(EntryHeader);
                cat_sum = 0;
                cat_suc = 0;
            }

            cat_sum++;
            cat_suc += System.Convert.ToInt32(stage.completed_once);

            GameObject stage_entry = Instantiate(EntryBody);
            CreateEntry(stage_entry, category_entry.GetNthChild(new List<int> { 1 }),
                stage.number.ToString(), stage.name, stage.description,
                stage.completed_once ? Accomplished : NotYetAccomplished);

            stage_entry.GetNthChild(new List<int> { 0, 0 }).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                StageStatic.devel = false;
                // TODO: handle unable to load
                Loader.LoadStage(stage.name, local, false);
            });

            stage_entry.GetNthChild(new List<int> { 0, 0, 3, 0 }).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate
            {
                pageMenue.SetMode(3);
                editLoader.SetStage(stage.name, !stage.use_install_folder);
            });

            for(int i = 0; i < stage.player_record_list.Count; i++)
            {
                GameObject time_entry = Instantiate(EntryTime);
                time_entry.transform.SetParent(stage_entry.GetNthChild(new List<int> { 1 }).transform, false);

                PopulateLocalEntryList(time_entry, new List<string> { 
                    (i + 1).ToString(),
                    stage.player_record_list[i].name_nr.ToString(), // hidden
                    System.TimeSpan.FromSeconds(stage.player_record_list[i].time).ToString("hh':'mm':'ss") });

                var record = stage.player_record_list[i];
                time_entry.GetNthChild(new List<int> { 3, 0 }).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                    stage.deletet_record(record);
                });
                time_entry.GetNthChild(new List<int> { 4, 0 }).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                    stage.set_record(record);
                    StageStatic.devel = false;
                    // TODO: handle unable to load
                    Loader.LoadStage(stage.name, local, true);
                });
            }
        }

        PopulateCategory();

        return;

        // === local methods ===

        void PopulateCategory()
        {
            if (category_entry != null)
            {
                var p = cat_suc / cat_sum;
                CreateEntry(category_entry, List,
                    ((int) (p*100)).ToString() + "%",
                    current_category, "",
                    p * Accomplished + (1 - p) * NotYetAccomplished);
            }
        }
    }

    private void CreateEntry(GameObject entry, GameObject parent, string number, string name, string description, Color color_status)
    {
        entry.transform.SetParent(parent.transform, false);

        entry = entry.GetNthChild(new List<int> { 0, 0 });

        PopulateLocalEntryList(entry, new List<string> { number, name, description });

        entry.GetNthChild(new List<int> { 0, 0 }).GetComponent<UnityEngine.UI.Image>().color = color_status;
    }

    private void PopulateLocalEntryList(GameObject entry, List<string> input)
    {
        for (int i = 0; i < input.Count; i++)
            entry.GetNthChild(new List<int> { i, 0, 0 }).GetComponent<TMPro.TextMeshProUGUI>().text = input[i];
    }
}
