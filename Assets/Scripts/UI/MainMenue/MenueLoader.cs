using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenueLoader : MonoBehaviour
{
    public UnityEngine.UI.ScrollRect scroll;
    public GameObject Pages;

    protected static int mode = 0;
    private int mode_last = 0;

    public void SetMode(int select)
    {
        if (!gameObject.activeSelf)
            return;

        switch (select)
        {
            case 0:
            case 1:
                break;

            case 2:
                if (mode == select) {
                    SetMode(mode_last);
                    return;
                }
                break;
        }

        Clear();

        mode_last = mode;
        mode = select;

        Pages.transform.GetChild(select).gameObject.SetActive(true);
    }

    private void Clear()
    {
        for (int i = 0; i < Pages.transform.childCount; i++)
            Pages.transform.GetChild(i).gameObject.SetActive(false);
    }




}
