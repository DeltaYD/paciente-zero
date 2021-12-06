using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
#pragma warning disable 0649
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    [SerializeField] TextMeshProUGUI[] choicesText;
#pragma warning restore 0649

    public PlayerManager _pm;
    public MainMenu _levelChanger;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }
    public bool choosing = false;
    public bool triggerCutscene = false;
    public bool cutsceneMode = false;

    private static DialogueManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene.");
        }
        instance = this;
    }

    // Start is called before the first frame update
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
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ContinueStory();
        }
    }

    public static DialogueManager GetInstance()
    {
        return instance;
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

        // hardcode rip this is ugly asf
        if (_pm._hasEndingN && cutsceneMode)
        {
            _levelChanger.sceneToMoveOn = 5;
            cutsceneMode = false;
            _levelChanger.StartGame();
        }
        else if (triggerCutscene)
        {
            if (_pm._radioProgress < 30)
            {
                // ENDING A: PARASITE WINS
                _levelChanger.sceneToMoveOn = 4;
            }
            else
            {
                // ENDING N: PLAYER MUST END HIMSELF SOMEWHERE ELSE
                _levelChanger.sceneToMoveOn = 6;
            }
            cutsceneMode = false;
            _levelChanger.StartGame();
        }
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
        Debug.Log(choiceIndex);
        currentStory.ChooseChoiceIndex(choiceIndex);

        if (choiceIndex == 0 && cutsceneMode)
        {
            //start a cutscene at the end of the script
            triggerCutscene = true;
        }
        else
        {
            cutsceneMode = false;
        }

        choosing = false;
        //ContinueStory();
    }

    private IEnumerator SelectFirstChoice()
    {
        // clear first
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        // set selected on a different frame
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void CutsceneModeEnable()
    {
        cutsceneMode = true;
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

}
