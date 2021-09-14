using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UIToolBox;

public class CollapsableStage : MonoBehaviour
{
    public GameObject TimeEntry;
    public MenueLoader pageMenue;
    public EditLoader editLoader;
    public string stage_name;
    public bool local;
    public Stage stage { get { return StageStatic.GetStage(stage_name, local); } }

    public void Init()
    {
        var header = gameObject.GetNthChild(new List<int> { 0, 0 });

        // set text
        PopulateLocalEntryList(header, new List<string> { stage.number.ToString(), stage.name, stage.description });

        // set colour
        header.GetNthChild(new List<int> { 0, 0 }).GetComponent<UnityEngine.UI.Image>().color = 
            stage.completed_once ? GlobalBehaviour.StageAccomplished : GlobalBehaviour.StageNotYetAccomplished;

        // set implicit load button (whole header)
        header.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
            StageStatic.devel = false;
            // TODO: handle unable to load
            Loader.LoadStage(stage.name, !stage.use_install_folder, false);
        });

        // set explicit edit button
        header.GetNthChild(new List<int> { 3, 0 }).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate
        {
            pageMenue.SetMode(3);
            editLoader.SetStage(stage.name, !stage.use_install_folder);
        });

        DrawChildren();
    }

    public void DrawChildren()
    {
        var record_list = stage.player_record_list.Values.OrderBy(r => r.time).ToList();
        var body = gameObject.GetNthChild(new List<int> { 1 });
        body.DestroyAllChildren();

        for (int i = 0; i < record_list.Count; i++)
        {
            GameObject time_entry = Instantiate(TimeEntry);
            time_entry.transform.SetParent(body.transform, false);

            PopulateLocalEntryList(time_entry, new List<string> {
                    (i + 1).ToString(),
                    "", // hidden
                    System.TimeSpan.FromSeconds(record_list[i].time).ToString("hh':'mm':'ss") });

            var index = record_list[i].name;
            // set delete button
            time_entry.GetNthChild(new List<int> { 3, 0 }).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                stage.deletet_record(stage.player_record_list[index]);
                this.Init();
            });
            // set clone button
            time_entry.GetNthChild(new List<int> { 4, 0 }).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate {
                stage.set_record(stage.player_record_list[index]);
                StageStatic.devel = false;
                // TODO: handle unable to load
                Loader.LoadStage(stage.name, !stage.use_install_folder, true);
            });
        }
    }
}
