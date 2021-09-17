using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIToolBox
{
    public static void PopulateLocalEntryList(GameObject entry, List<string> input)
    {
        for (int i = 0; i < input.Count; i++)
            entry.GetNthChild(new List<int> { i, 0, 0 }).GetComponent<TMPro.TextMeshProUGUI>().text = input[i];
    }
}
