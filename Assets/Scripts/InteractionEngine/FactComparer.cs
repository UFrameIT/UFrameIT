using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactComparer : EqualityComparer<Fact>
{
    protected bool search_righthanded;

    protected abstract bool Compare (Fact solution, Fact fact);

    public FactComparer SetSearchRight()
    {
        search_righthanded = true;
        return this;
    }
    public FactComparer SetSearchLeft()
    {
        search_righthanded = false;
        return this;
    }

    public override bool Equals(Fact left, Fact right)
    {
        return search_righthanded ? Compare(left, right) : Compare(right, left);
    }

    public override int GetHashCode(Fact obj)
    {
        return obj.GetType().GetHashCode();
    }
}

public class FactEquivalentsComparer : FactComparer
{
    protected override bool Compare (Fact solution, Fact fact)
    {
        return solution.Equivalent(fact);
    }
}

class LineFactHightDirectionComparer : FactComparer
{
    protected override bool Compare (Fact solution, Fact fact)
    {
        return fact is LineFact && solution is LineFact
            && Math3d.IsApproximatelyParallel(((LineFact) fact).Dir, ((LineFact) solution).Dir)
            && ((LineFact) fact).Distance + Math3d.vectorPrecission >= ((LineFact) solution).Distance;
        // && Mathf.Approximately(((LineFact) x).Distance, ((LineFact) y).Distance);
    }
}
