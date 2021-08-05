using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

//TODO: think about having one Level class and then different subclasses like TreeLevel, Tangenslevel?
public class Level : MonoBehaviour
{

    //Solving game parameters
    public int minimalSolutionHight;

    public static bool solved = false;


    void Start()
    // Start is called before the first frame update
    {
        // TODO: do not generate! -> load from somewhere
        PointFact
            buttom = new PointFact(Vector3.zero, Vector3.up, SolutionManager),
            top = new PointFact(Vector3.zero + Vector3.up * minimalSolutionHight, Vector3.up, SolutionManager);

        SolutionManager.Add(buttom, out _);
        SolutionManager.Add(top, out _, true);

        LineFact target = new LineFact(buttom.URI, top.URI, SolutionManager);
        Solution.Add(SolutionManager[SolutionManager.Add(target, out _, true)]);
    }

    public static bool gameSolved()
    {
        solved =
            LevelFacts.DynamiclySolved(Solution, out _, out List<Fact> hits, new LineFactHightDirectionComparer());

        if (solved)
            foreach (var hit in hits)
                AnimateExistingFactEvent.Invoke(hit);

        return solved;
    }

}
