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
    private int sentenceIndex;
    private int letterIndex = 0;
    private bool typingActive = false;
    //speed for typing the Text
    public float typingSpeed;
    private float timer = 0;

    //Only reset once after Player is out of range of the TaskCharacter
    private bool textReseted = true;
    //If pushoutSucceeded -> Disable Talking with TaskCharacter
    private bool pushoutSucceeded = false;

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.PushoutFactEvent.AddListener(PushoutSucceededSentence);
        //Type first sentence
        typingActive = true;
        TypeFkt();

    }

    private void Update()
    {
        TypeFkt();

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
        foreach (char letter in sentences[sentenceIndex].ToCharArray()) {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void TypeFkt() {
        if (typingActive)
        {
            if (this.timer >= this.typingSpeed)
            {
                if (letterIndex < sentences[sentenceIndex].Length)
                {
                    textDisplay.text += sentences[sentenceIndex].ToCharArray()[letterIndex];
                    letterIndex++;
                }
                else
                {
                    this.typingActive = false;
                    letterIndex = 0;
                }

                this.timer = 0;
            }
            else
            {
                this.timer += Time.deltaTime;
            }
        }
    }


    public void NextSentence() {
        //-2 because the last sentence is only for SucceededPushout-Purposes
        if (sentenceIndex < sentences.Length - 2)
        {
            sentenceIndex++;
            letterIndex = 0;
            typingActive = true;
            timer = 0;
            textDisplay.text = "";
            TypeFkt();
            textReseted = false;
        }
        else {
            textDisplay.text = "";
        }
    }

    public void ResetSentence() {
        sentenceIndex = 0;
        letterIndex = 0;
        typingActive = true;
        timer = 0;
        textDisplay.text = "";
        //Type first sentence again
        TypeFkt();
        textReseted = true;
    }

    public void PushoutSucceededSentence(Fact startFact) {
        textDisplay.text = "";
        //Last Sentence is the Pushout-Sentence
        sentenceIndex = sentences.Length - 1;
        letterIndex = 0;
        typingActive = true;
        timer = 0;
        pushoutSucceeded = true;
        //Disable Hint With Enter-Key for Talking
        textHint.GetComponent<MeshRenderer>().enabled = false;
        //Type final message
        TypeFkt();
    }
}
