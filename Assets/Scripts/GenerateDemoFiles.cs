using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class GenerateDemoFiles
{
    public static void GenerateAll()
    {
        GenerateDemoA();
    }

    public static void GenerateDemoA()
    {
        // Params
        float minimalSolutionHight = 6;

        // Generate Stage
        Stage demo = new Stage();
        demo.number = 0;
        demo.name = "TechDemo A";
        demo.scene = "RiverWorld";
        demo.description = "Improvised Level\n(Loadable)";
        demo.creatorMode = true;
        demo.use_install_folder = true;
        demo.hierarchie = new List<Directories> { /*Directories.Stages*/ };

        demo.factState = new FactOrganizer(true);
        demo.solution = new SolutionOrganizer(false);

        GlobalStatic.StageOfficial = new Dictionary<string, Stage>
        {
            { demo.name, demo },
        };
        GlobalStatic.SetStage(demo.name, false);

        // Populate Solution
        PointFact
            buttom = new PointFact(Vector3.zero, Vector3.up, GlobalStatic.stage.solution),
            top = new PointFact(Vector3.zero + Vector3.up * minimalSolutionHight, Vector3.up, GlobalStatic.stage.solution);

        GlobalStatic.stage.solution.Add(buttom, out _);
        GlobalStatic.stage.solution.Add(top, out _, true);

        LineFact target = new LineFact(buttom.Id, top.Id, GlobalStatic.stage.solution);
        var target_Id = GlobalStatic.stage.solution.Add(target, out _, true);
        Fact.Clear();

        // Set Solution
        GlobalStatic.stage.solution.ValidationSet =
            new List<(HashSet<string>, FactComparer)>
            { (new HashSet<string> { target_Id }, new LineFactHightDirectionComparer()) };

        // Save
        GlobalStatic.stage.store();
    }
}
