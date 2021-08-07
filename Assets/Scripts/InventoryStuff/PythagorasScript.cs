using System.Collections.Generic;
using UnityEngine;

public class PythagorasScript : MonoBehaviour
{
    private Dictionary<string, Fact> items = new Dictionary<string, Fact>();

    public void putFact(string name, Fact obj)
    {
        if (this.items.ContainsKey(name))
        {
            this.items.Remove(name);
        }
        this.items.Add(name, obj);
    }

    public void doMagic()
    {
        Dictionary<string, Fact>.Enumerator enumerator = this.items.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Debug.Log(enumerator.Current.Key + " is mapped to " + enumerator.Current.Value);
        }
    }
}
