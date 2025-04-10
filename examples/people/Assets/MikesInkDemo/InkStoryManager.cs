using UnityEngine;
using System.Collections;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;

public class InkStoryManager : MonoBehaviour
{
    [SerializeField]
	private TextAsset inkJSONAsset = null;

    [SerializeField]
    private GameObject choicePrefab;

    [SerializeField]
    private GameObject choicesUI;

    [SerializeField]
    private GameObject textBoxUI;

    [SerializeField]
    private TMP_Text textBox;

    // We are going to use this to make the coroutine wait until a choice button 
    // is clicked.
    private bool choiceMade = false;

    // We can use this to know whether a knot is currently running. I use it to
    // control whether I can launch TalkToCharacter or not.
    public bool knotActive = false;

    Story inkStory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inkStory = new Story(inkJSONAsset.text);

        StartCoroutine(LaunchKnot("IntroductoryScene"));
    }

    IEnumerator LaunchKnot(string knotName) {
        knotActive = true;
        // Set ink to use the knotName that was provided
        inkStory.ChoosePathString(knotName);

        textBoxUI.SetActive(true);
        
        while (inkStory.canContinue) {
            
            // As long as there are no choices, keep displaying lines in
            // the text box, and waiting for the player to press space.
            while (inkStory.canContinue) {
                
                string line = inkStory.Continue().Trim();
                Debug.Log(line);
                // Display the line in the text box
                textBox.text = line;

                // Wait for input
                while (!Input.GetKeyDown(KeyCode.Space))
                {
                    yield return null; // Wait for next frame
                }
                yield return null; // This is necessary because the loop continued when space was pressed and registered it as being pressed again.
            }

            // If there are any choices, wait for the choice to be made. We make this
            // a while because there may be two sets of choice in a row.
            while (inkStory.currentChoices.Count > 0) {

                choicesUI.SetActive(true);

                // Display all the choices, if there are any!
                for (int i = 0; i < inkStory.currentChoices.Count; i++) {
                    Choice choice = inkStory.currentChoices [i];
                    GameObject buttonObj = Instantiate(choicePrefab, choicesUI.transform);
                    Button button = buttonObj.GetComponent<Button>();
                    TMP_Text choiceText = buttonObj.GetComponentInChildren<TMP_Text>();
		            choiceText.text = choice.text;

                    // Tell the button what to do when we press it
                    button.onClick.AddListener(() => {
                        inkStory.ChooseChoiceIndex(choice.index);
                        inkStory.Continue();
                        choiceMade = true;
                        RemoveChoiceButtons();
                    });
                }
                // Wait for the button function above to be called, which sets
                // choiceMade to true, so we will move on in the coroutine
                while (!choiceMade)
                {
                    yield return null;
                }
                choiceMade = false; // reset this
                choicesUI.SetActive(false);
            }
        }   

        // Turn off the UI now that the knot is over
        textBoxUI.SetActive(false);
        knotActive = false;
        Debug.Log("KNOT COMPLETE!");
    }

    void RemoveChoiceButtons() {
		int childCount = choicesUI.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			Destroy (choicesUI.transform.GetChild (i).gameObject);
		}
    }

    public void TalkToCharacter() {
        // You need to reset the story if you want it to happen over and over
        // In this way, it might make sense to have a different ink file for 
        // each interaction.
        inkStory = new Story(inkJSONAsset.text);
        inkStory.variablesState["player"] = "Mike";
        inkStory.variablesState["responder"] = "Buddy the cat";
        inkStory.variablesState["responder_affinityTowardPlayer"] = 8;
        StartCoroutine(LaunchKnot("TalkToCharacter"));
    }

    // Update is called once per frame
    void Update()
    {
        if (!knotActive && Input.GetKeyDown(KeyCode.Space)) {
            TalkToCharacter();
        }
    }
}
