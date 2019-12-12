using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PythagorasScript : MonoBehaviour
{
    private Dictionary<string, ItemObject> items = new Dictionary<string, ItemObject>();

    public void putItem(string name, ItemObject obj) {
        if (this.items.ContainsKey(name)) {
            this.items.Remove(name);
        }
        this.items.Add(name, obj);
    }

    public void doMagic() {
        Dictionary<string, ItemObject>.Enumerator enumerator = this.items.GetEnumerator();
        while (enumerator.MoveNext()){
            Debug.Log(enumerator.Current.Key + " is mapped to " + enumerator.Current.Value);
        }
    }
}
