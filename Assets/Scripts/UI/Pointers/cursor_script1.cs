using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cursor_script1 : MonoBehaviour
{

    //DEMO TO COPY FROM
    public Texture2D cursorArrow;
    public Texture2D cursorPickaxe;

    void Start()
    {
        // Cursor.visible = false;
        Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
    }

    void OnMouseEnter()
    {
        Cursor.SetCursor(cursorPickaxe, Vector2.zero, CursorMode.ForceSoftware);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
    }

}