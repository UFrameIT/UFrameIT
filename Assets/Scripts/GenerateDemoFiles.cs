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
        demo.use_install_folder = true;
        demo.hierarchie = new List<Directories> { /*Directories.Stages*/ };

        demo.factState = new FactOrganizer(true);
        demo.solution = new SolutionOrganizer(false);

        StageStatic.StageOfficial = new Dictionary<string, Stage>
        {
            { demo.name, demo },
        };
        StageStatic.SetStage(demo.name, false);

        // Populate Solution
        PointFact
            buttom = new PointFact(Vector3.zero, Vector3.up, StageStatic.stage.solution),
            top = new PointFact(Vector3.zero + Vector3.up * minimalSolutionHight, Vector3.up, StageStatic.stage.solution);

        StageStatic.stage.solution.Add(buttom, out _);
        StageStatic.stage.solution.Add(top, out _, true);

        LineFact target = new LineFact(buttom.Id, top.Id, StageStatic.stage.solution);
        var target_Id = StageStatic.stage.solution.Add(target, out _, true);

        // Set Solution
        StageStatic.stage.solution.ValidationSet =
            new List<(HashSet<string>, FactComparer)>
            { (new HashSet<string> { target_Id }, new LineFactHightDirectionComparer()) };

        // Save
        StageStatic.stage.store();
    }
}
