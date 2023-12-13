using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipBox;

    [HideInInspector] public Sprite playerAddedSprite;
    [SerializeField] private Sprite cupSprite;
    [SerializeField] private Sprite coinSprite;

    [HideInInspector] public bool shouldGiveATMTooltip = true;
    [HideInInspector] public bool interactedWithATM = false;
    [HideInInspector] public bool timeToCheckForEnteringRoom = false;
    [HideInInspector] public bool enteredCubicleRoom = false;

    private Coroutine tutorialRoutine;

    public void StartTutorial() {
        tutorialRoutine = StartCoroutine(TutorialRoutine());
    }
    public void StopTutorial()
    {
        if (tutorialRoutine != null)
            StopCoroutine(tutorialRoutine);
        playerAddedSprite = null;
        tooltipBox.text = "";
    }

    /* Runs through a set of objectives for the player to accomplish during the tutorial
     * Only gives an ATM tooltip if the player does not have that task as the player should be able to decide if they want to complete their task or not
     */
    private IEnumerator TutorialRoutine() {
        tooltipBox.text = "Press E to interact with an item, try grabbing a cup!";

        while (playerAddedSprite != cupSprite) yield return null; /* Wait for player to grab cup */

        if (shouldGiveATMTooltip) {         
            tooltipBox.text = "You can get a coin from the ATM by spending your money!";

            while (!interactedWithATM) yield return null; /* Wait for player to interact with the atm */

            tooltipBox.text = "Now grab the coin!";

            while (playerAddedSprite != coinSprite) yield return null; /* Wait for player to grab coin */
        }

        tooltipBox.text = "You can press control to crouch!";

        while(!Input.GetKeyDown(KeyCode.LeftControl)) yield return null; /* Wait for player to crouch */

        timeToCheckForEnteringRoom = true;
        tooltipBox.text = "Try making it to the cubicles without getting caught!";

        while(!enteredCubicleRoom) yield return null; /* Wait for player to enter a room with cubicles */

        tooltipBox.text = "In later levels, you can bribe coworkers to complete your tasks. But you won't know how much they're charging for it!";

        yield return new WaitForSeconds(10);

        tooltipBox.text  = "Now explore the office and avoid the manager until the end of the day!";

        while(SceneManager.GetActiveScene().name.Equals("Level 1")) yield return null; /* Wait for the level to end */

        tooltipBox.text = "";
    }
}
