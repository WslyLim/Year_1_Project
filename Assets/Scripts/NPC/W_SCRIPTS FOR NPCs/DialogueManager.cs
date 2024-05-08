using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;

public class DialogueManager : MonoBehaviour
{

    //public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private bool dialogueStarted = false;
    private Queue<string> sentences;
    private FirstPersonController playerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<FirstPersonController>();
        sentences = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dialogueStarted) 
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        playerInput.enabled = false;
        dialogueStarted = true;
        dialogueBox.SetActive(true);
        //nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() 
    {
        Debug.Log(sentences.Count);
        if (sentences.Count == 0)
        {
            dialogueStarted= false;
            dialogueBox.SetActive(false);
            playerInput.enabled = true;
            return;
        }

        string sentence = sentences.Dequeue();

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

}
