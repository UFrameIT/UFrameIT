using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

//TODO: think about having one Level class and then different subclasses like TreeLevel, Tangenslevel?
public class Level : MonoBehaviour
{
    void Start()
    // Start is called before the first frame update
    {

    }

    public static bool gameSolved()
    {
        bool solved =
            StageStatic.stage.factState.DynamiclySolved(StageStatic.stage.solution.getMasterFactsByIndex(0), out _, out List<Fact> hits, FactComparer: StageStatic.stage.solution.ValidationSet[0].Comparer);
        bool solvedEXP =
            StageStatic.stage.factState.DynamiclySolvedEXP(StageStatic.stage.solution, out _, out List<List<string>> hitsEXP);


        if (solved)
            foreach (var hit in hits)
                AnimateExistingFactEvent.Invoke(hit);

        if (solvedEXP)
            foreach (var hitlist in hitsEXP)
                foreach(var hit in hitlist)
                    AnimateExistingFactEvent.Invoke(StageStatic.stage.factState[hit]);

        return solved;
    }
}
