using UnityEngine;
using static CommunicationEvents;

//TODO: think about having one Level class and then different subclasses like TreeLevel, Tangenslevel?
public class Level : MonoBehaviour
{

    //Solving game parameters
    public GameObject snapZoneTop;
    public GameObject snapZoneBottom;
    public static Vector3 solutionVector;
    public static bool solved = false;
    // Start is called before the first frame update
    void Start()
    {
        solutionVector = snapZoneTop.transform.position - snapZoneBottom.transform.position;
    }


    public static bool gameSolved()
    {

        Vector3 tempDir1 = new Vector3(0, 0, 0);
        Vector3 tempDir2 = new Vector3(0, 0, 0);

        if (solved == true)
            return true;
        else
        {
            //Look for solutionFact in global factList
            foreach (var entry in Facts)
            {
                Fact fact = entry.Value;
                if (typeof(LineFact).IsInstanceOfType(fact))
                {
                    tempDir1 = ((PointFact) Facts[((LineFact) fact).Pid1]).Point
                             - ((PointFact) Facts[((LineFact) fact).Pid2]).Point;
                    tempDir2 = ((PointFact) Facts[((LineFact) fact).Pid2]).Point
                             - ((PointFact) Facts[((LineFact) fact).Pid1]).Point;

                    if (solutionVector == tempDir1 || solutionVector == tempDir2)
                    {
                        solved = true;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
