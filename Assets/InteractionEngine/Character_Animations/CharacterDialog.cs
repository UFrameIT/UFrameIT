using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static TaskCharakterAnimation;

public class CharacterDialog : MonoBehaviour
{
    public TextMeshPro textDisplay;
    public TextMeshPro textHint;
    public string[] sentences;
    private int index;
    //speed for typing the Text
    public float typingSpeed;

    //Only reset once after Player is out of range of the TaskCharacter
    private bool textReseted = true;
    //If pushoutSucceeded -> Disable Talking with TaskCharacter
    private bool pushoutSucceeded = false;

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.PushoutFactEvent.AddListener(PushoutSucceededSentence);
        //Type first sentence
        StartCoroutine(Type());

    }

    private void Update()
    {
        if(!pushoutSucceeded && Input.GetKeyDown(KeyCode.Return) && TaskCharakterAnimation.getPlayerInTalkingZone())
        {
            //Type Next sentence if player is in the talkinZone around the TaskCharacter AND the player typed the return-Key
            NextSentence();
        }
        else if (!pushoutSucceeded && !TaskCharakterAnimation.getPlayerInTalkingZone() && !textReseted)
        {
            //Reset Sentence if Player is out of range of the TaskCharacter and it's not already reseted
            ResetSentence();
        }
    }

    //Type a sentence slowly
    IEnumerator Type() {
        foreach (char letter in sentences[index].ToCharArray()) {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }


    public void NextSentence() {
        //-2 because the last sentence is only for SucceededPushout-Purposes
        if (index < sentences.Length - 2)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
            textReseted = false;
        }
        else {
            textDisplay.text = "";
        }
    }

    public void ResetSentence() {
        index = 0;
        textDisplay.text = "";
        //Type first sentence again
        StartCoroutine(Type());
        textReseted = true;
    }

    public void PushoutSucceededSentence(Fact startFact) {
        textDisplay.text = "";
        //Last Sentence is the Pushout-Sentence
        index = sentences.Length - 1;
        pushoutSucceeded = true;
        //Disable Hint With Enter-Key for Talking
        textHint.GetComponent<MeshRenderer>().enabled = false;
        //Type final message
        StartCoroutine(Type());
    }
}
