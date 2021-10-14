using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all FactComparer to be used in conjucntion with <see cref="Stage.solution"/>
/// <seealso cref="SolutionOrganizer"/>
/// <seealso cref="SolutionOrganizer.ValidationSet"/>
/// <seealso cref="FactOrganizer.DynamiclySolved(SolutionOrganizer, out System.Collections.Generic.List<System.Collections.Generic.List<string>>, out System.Collections.Generic.List<System.Collections.Generic.List<string>>)"/>
/// </summary>
public abstract class FactComparer : EqualityComparer<Fact>
{
    /// <summary>defines parameter order for <see cref="Equals(Fact, Fact)"/></summary>
    protected bool search_righthanded;

    /// <summary>
    /// Implements criteria, by which <paramref name="fact"/> is beeing compared to <paramref name="solution"/>
    /// </summary>
    /// <param name="solution">to be fulfilled</param>
    /// <param name="fact">to be fulfilling</param>
    /// <returns><c>true</c> iff fact is described by solution</returns>
    protected abstract bool Compare (Fact solution, Fact fact);

    /// <summary>
    /// Sets <see cref="search_righthanded"/>, so that parameter order of <see cref="Equals(Fact, Fact)"/> is effectively:
    /// (<see cref="Compare(Fact, Fact).solution"/>, <see cref="Compare(Fact, Fact).fact"/>), 
    /// when a <see cref="ICollection<Fact>"/> is on the right hand side of an <see cref="System.Linq"/> operation
    /// </summary>
    /// <returns><c>this</c> object to be used</returns>
    public FactComparer SetSearchRight()
    {
        search_righthanded = true;
        return this;
    }
    /// <summary>
    /// Sets <see cref="search_righthanded"/>, so that parameter order of <see cref="Equals(Fact, Fact)"/> is effectively:
    /// (<see cref="Compare(Fact, Fact).solution"/>, <see cref="Compare(Fact, Fact).fact"/>), 
    /// when a <see cref="ICollection<Fact>"/> is on the left hand side of an <see cref="System.Linq"/> operation
    /// </summary>
    /// <returns><c>this</c> object to be used</returns>
    public FactComparer SetSearchLeft()
    {
        search_righthanded = false;
        return this;
    }

    /// <summary>
    /// Called by <see cref="System.Linq"/> iff <see cref="GetHashCode(Fact)"/> returns same result for both parameters <paramref name="left"/> and <paramref name="right"/>.
    /// <remarks>Always set correct search order *manually* beforehand, so that <see cref="Compare(Fact, Fact)"/> can return correct results.</remarks>
    /// <seealso cref="SetSearchRight"/>
    /// <seealso cref="SetSearchLeft"/>
    /// </summary>
    /// <param name="left">lefthand parameter</param>
    /// <param name="right">righthand parameter</param>
    /// <returns><c>search_righthanded ? Compare(left, right) : Compare(right, left);</c></returns>
    public override bool Equals(Fact left, Fact right)
    {
        return search_righthanded ? Compare(left, right) : Compare(right, left);
    }

    /// <summary>
    /// Called by <see cref="System.Linq"/> to check for possible fulfillment of each parameter before calling <see cref="Equals(Fact, Fact)"/> iff both results are the same.
    /// <remarks>Default implementation checks for sameness of runtime type, but may vary for subclasses.</remarks>
    /// </summary>
    /// <param name="obj">parameter of possible subsequent call of <see cref="Equals(Fact, Fact)"/></param>
    /// <returns></returns>
    public override int GetHashCode(Fact obj)
    {
        return obj.GetType().GetHashCode();
    }
}

/// <summary>
/// Checks if both <see cref="Fact">Facts</see> are Equivalent, while accounting for Unity and floating point precission
/// </summary>
public class FactEquivalentsComparer : FactComparer
{
    /// \copydoc FactEquivalentsComparer
    /// \copydoc FactComparer.Compare
    protected override bool Compare (Fact solution, Fact fact)
    {
        return solution.Equivalent(fact);
    }
}

/// <summary>
/// Checks <see cref="LineFact">LineFacts</see> if <see cref="Compare(Fact, Fact).fact"/> is of same direction and at least of same length as <see cref="Compare(Fact, Fact).solution"/>, while accounting for Unity and floating point precission
/// <seealso cref="Math3d.vectorPrecission"/>
/// </summary>
class LineFactHightDirectionComparer : FactComparer
{
    /// \copydoc LineFactHightDirectionComparer
    /// \copydoc FactComparer.Compare
    protected override bool Compare (Fact solution, Fact fact)
    {
        return fact is LineFact factLine && solution is LineFact solutionLine
            && Math3d.IsApproximatelyParallel(factLine.Dir, solutionLine.Dir)
            && factLine.Distance + Math3d.vectorPrecission >= solutionLine.Distance;
        // && Mathf.Approximately(((LineFact) x).Distance, ((LineFact) y).Distance);
    }
}
