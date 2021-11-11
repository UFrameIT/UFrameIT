using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Allowes for Pages of <see cref="MenueLoader"/> to contain Lists.
/// </summary>
/// <typeparam name="T">Type to list</typeparam>
public abstract class ListLoader<T> : MenueLoader
{
    /// <summary> <see cref="GameObject"/> to which new <see cref="EntryHeader"/> entries are being added. </summary>
    public GameObject List;
    /// <summary> Template for new entries to be added to <see cref="List"/> </summary>
    public GameObject EntryHeader;

    protected void OnEnable()
    {
        Clear();
        Init();
    }

    protected void OnDisable()
    {
        Clear();
    }

    /// <summary> Creates listing </summary>
    public abstract void Init();

    /// <summary> Removes all list-entries</summary>
    protected virtual void Clear()
    {
        List.DestroyAllChildren();
    }

    /// <summary> Creates a Default listing.</summary>
    protected abstract void Default();

    public virtual void ListButtons(List<T> list)
    {
        if (list.Count == 0)
            Default();
        else
            ListButtonsWrapped(list);
    }

    protected abstract void ListButtonsWrapped(List<T> list);

}