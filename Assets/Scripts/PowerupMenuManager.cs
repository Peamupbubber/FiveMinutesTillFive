using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerupMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI confirmButtontext;

    [SerializeField] private GameObject trashCanButton;
    [SerializeField] private GameObject throwingButton;
    [SerializeField] private GameObject stickyHandButton;

    private string[] powerupNames = { "A", "B", "C" };
    private int choice;

    //Some field that will update the powerup in gameManager

    private void Start()
    {
        GameManager.gameManager.powerupMenu = GetComponent<Canvas>();
        if (GameManager.gameManager.trashCanPowerEnabled)
            trashCanButton.SetActive(false);
        if (GameManager.gameManager.throwingPowerEnabled)
            throwingButton.SetActive(false);
        if (GameManager.gameManager.stickyHandPowerEnabled)
            stickyHandButton.SetActive(false);
    }

    public void UpdateConfirmText(int i) {
        confirmButtontext.gameObject.transform.parent.gameObject.SetActive(true);
        confirmButtontext.text = "Confirm powerup " + powerupNames[i] + "?";
        choice = i;
    }

    public void StartLevel() {
        if (choice == 0)
            GameManager.gameManager.TrashCanPowerup();
        else if (choice == 1)
            GameManager.gameManager.ThrowingPowerup();
        else if (choice == 2)
            GameManager.gameManager.StickyHandPowerup();
        else
            Debug.Log("This should never happen");

        GameManager.gameManager.StartLevel();
    }
}
