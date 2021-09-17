using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: SE: Split for Stage/Local
public abstract class ListLoader<T> : MenueLoader
{
    public GameObject List;
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

    public abstract void Init();

    protected virtual void Clear()
    {
        List.DestroyAllChildren();
    }

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