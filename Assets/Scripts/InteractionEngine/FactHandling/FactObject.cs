using UnityEngine;

/// <summary>
/// <see cref="Fact.Id"/>/ <c>MonoBehaviour</c> wrapper to be attached to <see cref="Fact.Representation"/>
/// </summary>
public class FactObject : MonoBehaviour
{
    /// <summary>
    /// <see cref="Fact.Id"/> to identify arbitrary <see cref="Fact"/> by its <see cref="Fact.Representation"/>
    /// </summary>
    public string URI;
}
