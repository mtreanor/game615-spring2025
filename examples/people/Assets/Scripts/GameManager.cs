using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    // Create a list
    public List<UnitScript> units = new List<UnitScript>();

    public Camera cam;

    public UnitScript selectedUnit = null;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    void OnEnable()
    {
        if (GameManager.instance != null)
        {
            Destroy(this);
        }
        else
        {
            GameManager.instance = this;
        }

        // Subscribe to the action that will come from the DialogueManager
        DialogueManager.DialogueAction += DisplayDialogue;
    }

    void OnDisable() 
    {
        DialogueManager.DialogueAction -= DisplayDialogue;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // This if checks that the left mouse button was pressed, and the second part
        // of this checks to see that the the mouse isn't over a UI element. The purpose
        // for this was that the lines below were making it so the click on buttons
        // never worked (because they turned off the UI element before the click was
        // recognized by the button).
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            // This 'ScreenPointToRay' function converts a screen position to a world (3d) ray
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            // See the below 'out' in front of the parameter hit. This is a way that Unity/C#
            // allows the Physics.Raycast function to 'stuff' a bunch of useful information
            // inside of the 'hit' variable.
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity) )
            {
                // If we get in here, it means that the raycast collided with something.
                // 'hit' now has this useful info:
                //      - collider (because we get access to the gameObject through that
                //      - point (the position the raycast hit
                //      - the "layer" that the gameObject we hit is on
                //
                // Check to see that thing we hit was a unit
                if (hit.collider.CompareTag("unit"))
                {

                    SelectUnit(hit.collider.gameObject.GetComponent<UnitScript>());

                }
                else
                {
                    // If we get in here, it means we clicked on something other than
                    // a unit. If that thing is on the layer "Ground", tell the selected unit
                    // to move there.
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ground"))
                    {
                        selectedUnit?.GoToPoint(hit.point);
                    }
                }
            }
            else
            {
                // If the raycast didn't collide with anything, deselect the unit.
                if (selectedUnit != null)
                {
                    selectedUnit.bodyRenderer.material.color = selectedUnit.unselectedColor;
                }
                selectedUnit = null;
            }
        }
    }

    public void DisplayDialogue(string speaker, string dialogue) {
        nameText.text = speaker;
        dialogueText.text = dialogue;
        dialoguePanel.SetActive(true);
    }

    public void SelectUnit(UnitScript unit)
    {
        if (selectedUnit != null)
        {
            // Deselect the previously selected unit.
            selectedUnit.bodyRenderer.material.color = selectedUnit.unselectedColor;
        }

        // Set the currently selected unit to be the unit passed in to this function (that's the whole point!).
        selectedUnit = unit;
        selectedUnit.bodyRenderer.material.color = selectedUnit.selectedColor;
    }

}
