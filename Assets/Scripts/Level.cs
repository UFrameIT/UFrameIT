using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

//TODO: think about having one Level class and then different subclasses like TreeLevel, Tangenslevel?
public class Level : MonoBehaviour
{

    //Solving game parameters
    public int minimalSolutionHight;


    void Start()
    // Start is called before the first frame update
    {
        // TODO: do not generate! -> load from somewhere
        PointFact
            buttom = new PointFact(Vector3.zero, Vector3.up, SolutionManager),
            top = new PointFact(Vector3.zero + Vector3.up * minimalSolutionHight, Vector3.up, SolutionManager);

        SolutionManager.Add(buttom, out _);
        SolutionManager.Add(top, out _, true);

        LineFact target = new LineFact(buttom.Id, top.Id, SolutionManager);
        Solution.Add(SolutionManager[SolutionManager.Add(target, out _, true)]);
        Fact.Clear();

        SolutionSet = (SolutionManager,
            new List<(HashSet<string>, FactComparer)> 
            { (new HashSet<string> { target.Id, buttom.Id }, new LineFactHightDirectionComparer()) });
    }

    public static bool gameSolved()
    {
        bool solved =
            LevelFacts.DynamiclySolved(Solution, out _, out List<Fact> hits, FactComparer: new LineFactHightDirectionComparer());
        bool solvedEXP =
            LevelFacts.DynamiclySolvedEXP(SolutionSet, out _, out List<List<string>> hitsEXP);


        if (solved)
            foreach (var hit in hits)
                AnimateExistingFactEvent.Invoke(hit);

        if (solvedEXP)
            foreach (var hitlist in hitsEXP)
                foreach(var hit in hitlist)
                    AnimateExistingFactEvent.Invoke(LevelFacts[hit]);

        return solved;
    }

}
