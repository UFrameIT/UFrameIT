using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIToolBox;

public class CollapsableCategory : MonoBehaviour
{
    public GameObject StageEntry;
    public GameObject StageEntryEntry;
    public MenueLoader pageMenue;
    public EditLoader editLoader;

    public List<Stage> content;

    public string category;

    public void Init()
    {
        var header = gameObject.GetNthChild(new List<int> { 0, 0 });

        // set text init
        PopulateLocalEntryList(header, new List<string> { "err", category, "" });
        // set colour err (for now)
        header.GetNthChild(new List<int> { 0, 0 }).GetComponent<UnityEngine.UI.Image>().color =
            GlobalBehaviour.StageError;

        var p = DrawChildren();

        // set text percent
        PopulateLocalEntryList(header, new List<string> { ((int)(p * 100)).ToString() + "%"});

        // set colour percent
        header.GetNthChild(new List<int> { 0, 0 }).GetComponent<UnityEngine.UI.Image>().color =
            p * GlobalBehaviour.StageAccomplished + (1 - p) * GlobalBehaviour.StageNotYetAccomplished;
    }


    public float DrawChildren()
    {
        float
            cat_sum = 0,
            cat_suc = 0;

        foreach (var stage in content)
        {
            cat_sum++;
            cat_suc += System.Convert.ToInt32(stage.completed_once);

            GameObject stage_entry = Instantiate(StageEntry);
            stage_entry.transform.SetParent(gameObject.GetNthChild(new List<int>{ 1 }).transform, false);

            CollapsableStage stage_beahviour = stage_entry.GetComponent<CollapsableStage>();
            stage_beahviour.stage_name = stage.name;
            stage_beahviour.local = !stage.use_install_folder;
            stage_beahviour.pageMenue = pageMenue;
            stage_beahviour.editLoader = editLoader;
            stage_beahviour.TimeEntry = StageEntryEntry;
            stage_beahviour.Init();
        }

        return cat_suc / cat_sum;
    }
}
