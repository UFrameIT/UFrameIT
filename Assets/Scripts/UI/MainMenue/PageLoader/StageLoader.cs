using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UIToolBox;


public class StageLoader : ListLoader<Stage>
{
    public GameObject EntryBody;
    public GameObject EntryTime;

    public EditLoader editLoader;

    public bool local;
    protected string
        NoId = "",
        NoStage = "-----",
        NoDescr = "No Entry found, please check directory!",
        NoLocal = "No local Stage\nClick [+] button above";

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
        GameObject category_entry = null;
        CollapsableCategory category_behaviour = null;

        foreach (var cat in list.Select(s => s.category).Distinct())
        {
            category_entry = Instantiate(EntryHeader);
            category_entry.transform.SetParent(List.transform, false);

            category_behaviour = category_entry.GetComponent<CollapsableCategory>();
            category_behaviour.content = list.Where(s => s.category == cat).ToList();
            category_behaviour.category = cat;
            category_behaviour.pageMenue = pageMenue;
            category_behaviour.editLoader = editLoader;
            category_behaviour.StageEntry = EntryBody;
            category_behaviour.StageEntryEntry = EntryTime;
            category_behaviour.Init();
        }
    }

    protected void CreateEntry(GameObject entry, GameObject parent, string number, string name, string description, Color color_status)
    {
        entry.transform.SetParent(parent.transform, false);

        entry = entry.GetNthChild(new List<int> { 0, 0 });

        PopulateLocalEntryList(entry, new List<string> { number, name, description });

        entry.GetNthChild(new List<int> { 0, 0 }).GetComponent<UnityEngine.UI.Image>().color = color_status;
    }


}
