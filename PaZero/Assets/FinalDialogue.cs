using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class FinalDialogue : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject _visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    [SerializeField] TextMeshProUGUI[] choicesText;
#pragma warning restore 0649

    public bool _isInRange = false;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    public bool dialogueIsPlaying { get; private set; }
    public bool choosing = false;
    public PlayerManager player;
    private Story currentStory;



    private void Awake()
    {
        if (_visualCue != null)
        {
            _visualCue.SetActive(false);
        }
    }

    void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (Input.GetKeyDown(interactKey))
            {
                EnterDialogueMode(inkJSON);
            }
        }
        else
        {
            if (_visualCue != null)
            {
                _visualCue.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isInRange = true;
            if(_visualCue != null)
            {
                _visualCue.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isInRange = false;
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
        
    }

    private IEnumerator ExitDialogueMode()
    {
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        yield return new WaitForSeconds(0.12f);
        dialogueIsPlaying = false;
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
        }
        else if (!choosing)
        {
            //empty inkJSON file
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        // check if supported
        if(currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);

        }

        int index = 0;
        // enable and init choices to the amount of choices for this line of dialogue
        foreach(Choice choice in currentChoices)
        {
            choosing = true;
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for(int i = index; i <choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());

    }

    public void MakeChoice(int choiceIndex)
    {
        if(choiceIndex == 0) 
        {
            if(player._radioProgress < 25)
            {
                SceneManager.LoadScene("A-ENDING");
            }
            else 
            {
                SceneManager.LoadScene("B-ENDING");
            }
            
        }
        currentStory.ChooseChoiceIndex(choiceIndex);
        choosing = false;
        ContinueStory();
    }

    private IEnumerator SelectFirstChoice()
    {
        // clear first
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        // set selected on a different frame
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }
}
