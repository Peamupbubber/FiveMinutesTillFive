using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    private const int AGGRESSION_MULT = -100;
    private const int SELF_COMPLETED_TASK_MULT = 0;
    private const int COWORKER_COMPLETED_TASK_MULT = 100;
    private const int INCOMPLETE_TASK_MULT = 200;

    [SerializeField] private TextMeshProUGUI finalScoreText;

    private void Start()
    {
        GameManager.gameManager.endMenu = gameObject.GetComponent<Canvas>();
        CalculateFinalScore();
    }
    public void CalculateFinalScore() {
        float finalScore = AGGRESSION_MULT * GameManager.gameManager.managerAggression +
                           SELF_COMPLETED_TASK_MULT * GameManager.gameManager.selfCompleteTasks +
                           COWORKER_COMPLETED_TASK_MULT * GameManager.gameManager.coworkerCompleteTasks +
                           INCOMPLETE_TASK_MULT * GameManager.gameManager.incompleteTasks;

        finalScoreText.text += finalScore;
    }

    public void RestartGame()
    {
        GameManager.gameManager.RestartGame();
    }
    public void InfoScreen()
    {
        GameManager.gameManager.OpenInfoScreen();
    }

    public void QuitGame()
    {
        GameManager.gameManager.QuitGame();
    }
}
