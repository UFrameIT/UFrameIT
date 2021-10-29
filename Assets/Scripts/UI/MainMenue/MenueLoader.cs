using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Allows multiple Pages (direct children of this) to switch/ scroll through.
/// </summary>
public class MenueLoader : MonoBehaviour
{
    public MenueLoader pageMenue;
    public UnityEngine.UI.ScrollRect scroll = null;
    public GameObject Pages;

    private int mode = 0;
    private int mode_last = 0;

    protected void Start()
    {
        if(scroll != null)
            scroll.verticalNormalizedPosition = 1f;
    }

    /// <summary>
    /// Reverts to last opend Page.
    /// </summary>
    public void RevertMode()
    {
        SetMode(mode_last);
    }

    /// <summary>
    /// Deactivates all Pages, then activates Page <paramref name="select"/>.
    /// </summary>
    /// <param name="select">Page to switch to</param>
    public void SetMode(int select)
    {
        Clear();

        mode_last = mode;
        mode = select;

        Pages.transform.GetChild(select).gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates all Pages.
    /// </summary>
    private void Clear()
    {
        for (int i = 0; i < Pages.transform.childCount; i++)
            Pages.transform.GetChild(i).gameObject.SetActive(false);
    }

}
