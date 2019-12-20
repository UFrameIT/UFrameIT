using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolModeText : MonoBehaviour
{
    private bool timerActive { get; set; }
    private float timer { get; set; }
    private float timerDuration = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Show the Text that the MarkPointMode is active on startup
        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().CrossFadeAlpha(0.0f, 0.0f, false);
        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().CrossFadeAlpha(1.0f, 0.9f, false);
        this.timerActive = true;
        this.timer = 0;
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
    }

    // Update is called once per frame
    void Update()
    {
        //If the Timer is Active, check if timerDuration is reached and set the Text inactive
        if (this.timerActive)
        {
            this.timer += Time.deltaTime;
            if (this.timer >= this.timerDuration)
            {
                //gameObject.SetActive(false);
                gameObject.GetComponentInChildren<UnityEngine.UI.Text>().CrossFadeAlpha(0.0f, 0.3f, false);
                this.timerActive = false;
                this.timer = 0;
            }
        }
    }

    void OnToolModeChanged(ToolMode ActiveToolMode) {

        //When ToolMode changes: Start a new Timer for showing up the Text for it
        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "ToolMode = " + ActiveToolMode;
        //gameObject.SetActive(true);
        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().CrossFadeAlpha(1.0f, 0.3f, false);
        this.timerActive = true;
        this.timer = 0;
    }
}
