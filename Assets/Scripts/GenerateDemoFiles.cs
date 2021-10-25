using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class GenerateDemoFiles
{
    public static void GenerateAll()
    {
        GenerateDemoA();
        GenerateDemoB();
    }

    public static void GenerateDemoA()
    {
        // Params
        float minimalSolutionHight = 6;

        // Generate Stage
        // TODO? use constructor
        Stage demo = new Stage
        {
            number = 0,
            category = "Demo Category",
            name = "TechDemo A",
            scene = "RiverWorld",
            description = "Tree Stage",
            use_install_folder = true,
            hierarchie = new List<Directories> { /*Directories.Stages*/ }
        };

        // needed to generate facts
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
            new List<SolutionOrganizer.SubSolution>
            { new SolutionOrganizer.SubSolution(new HashSet<string> { target_Id }, null, null, new LineFactHightDirectionComparer()) };

        // Save
        StageStatic.stage.store();
    }

    public static void GenerateDemoB()
    {
        // Params
        float minimalSolutionHight = 6;

        // Generate Stage
        // TODO? use constructor
        Stage demo = new Stage
        {
            number = 0,
            category = "Demo Category",
            name = "TechDemo B",
            scene = "RiverWorld",
            description = "River Stage",
            use_install_folder = true,
            hierarchie = new List<Directories> { /*Directories.Stages*/ }
        };

        // needed to generate facts
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
            new List<SolutionOrganizer.SubSolution> {
                new SolutionOrganizer.SubSolution(new HashSet<string> { target_Id }, null, null, new LineFactHightDirectionComparer()),
                new SolutionOrganizer.SubSolution(new HashSet<string> { target_Id }, null, null, new LineSpanningOverRiverWorldComparer()),
                new SolutionOrganizer.SubSolution(null, new List<int> { 1 }, new List<int> { 0 }, new LineFactHightComparer()),
            };

        // Save
        StageStatic.stage.store();
    }
}
