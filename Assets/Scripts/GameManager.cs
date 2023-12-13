using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const int TUTORIAL_TIME = 150;
    private const int LEVEL_TIME = 300;

    private const int MAX_ITEMS = 10;

    private const int NUM_LEVELS = 3;
    private const int NUM_TASKS = 9;
    private const int BASE_MONEY = 5;

    private const int BASE_PANEL_WIDTH = 450;
    private const int BASE_PANEL_HEIGHT = 60;
    private const int PANEL_TASK_EXTEND = 40;

    [HideInInspector] public static GameManager gameManager;

    private List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    [HideInInspector] public int levelTime;
    public int time;
    private int currentDay;

    public int playerMoney;

    [SerializeField] private Canvas startMenu;
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas infoScreen;
    [SerializeField] private Canvas deathMenu;
    [SerializeField] private Canvas playerHUD;
    [HideInInspector] public Canvas powerupMenu;
    [HideInInspector] public Canvas endMenu;

    private bool gameIsPaused = false;
    private bool tasksShowing = false;

    [HideInInspector] public bool deathScreenActive = false;

    [HideInInspector] public bool trashCanPowerEnabled = false;
    [HideInInspector] public bool throwingPowerEnabled = false;
    [HideInInspector] public bool stickyHandPowerEnabled = false;

    [SerializeField] private GameObject tasks;

    [Range(0, 1)] public float managerAggression;
    [SerializeField] private Slider managerAggressionBar;
    [SerializeField] private TextMeshProUGUI taskViewButtonText;
    [SerializeField] private TextMeshProUGUI sceneTextBox;
    [SerializeField] private TextMeshProUGUI timeTextBox;

    [SerializeField] private TextMeshProUGUI moneyBox;
    [SerializeField] private Image powerup1;
    [SerializeField] private Image powerup2;

    [SerializeField] private Sprite trashCanPowerupSprite;
    [SerializeField] private Sprite throwingPowerupSprite;
    [SerializeField] private Sprite stickyHandPowerupSprite;

    [SerializeField] private Image[] hotbarImages = new Image[MAX_ITEMS];
    [SerializeField] private GameObject highlight;
    private int numItems;
    private int currentlySelectedItemIndex;

    [SerializeField] private GameObject taskPrefab;
    private List<GameObject> uiTaskObjs = new List<GameObject>();
    private int numCurrentTasks;
    [HideInInspector] public int incompleteTasks = 0;

    [HideInInspector] public int selfCompleteTasks = 0;
    [HideInInspector] public int coworkerCompleteTasks = 0;

    [SerializeField] private GenericTask[] allTasks;
    public HashSet<GenericTask> curTasks = new HashSet<GenericTask>();
    private HashSet<int> usedTasks = new HashSet<int>();
    
    [SerializeField] private Tutorial tutorialScript;
    private AudioSource currentLevelMusic;

    private Coroutine levelTimer;

    void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        gameManager = this;

        numCurrentTasks = 0;
        currentlySelectedItemIndex = 0;

        //Make sure the correct UI is enabled
        startMenu.enabled = true;
        pauseMenu.enabled = false;
        infoScreen.enabled = false;
        deathMenu.enabled = false;
        playerHUD.enabled = false;

        ResetAllTasks();
    }

    void Update()
    {
        CheckPause();
        CheckTask();
        CheckScroll();
        CheckDeath();
    }

    /* Update Check Functions */

    //Pause or Play the game if not in the start menu
    private void CheckPause() {
        if (SceneManager.GetActiveScene().name.Equals("Powerups"))
            return;
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (infoScreen.enabled)
                CloseInfoScreen();
            else if (!SceneManager.GetActiveScene().name.Equals("Start Screen") && !SceneManager.GetActiveScene().name.Equals("Death Screen") && !SceneManager.GetActiveScene().name.Equals("End Screen")) { 
                if (gameIsPaused)
                    Resume();
                else
                    Pause();
            }
        }
    }

    private void CheckTask() {
        if (Input.GetKeyDown(KeyCode.Tab))
            TaskView();
    }

    //Gets mouse scroll input and changes which item in the hotbar is currently selected
    private void CheckScroll() {
        if (Input.mouseScrollDelta.y < 0) {
            if (currentlySelectedItemIndex == MAX_ITEMS - 1)
                currentlySelectedItemIndex = 0;
            else
                currentlySelectedItemIndex++;
            highlight.transform.SetParent(hotbarImages[currentlySelectedItemIndex].gameObject.transform, false);
        }
        else if (Input.mouseScrollDelta.y > 0) {
            if (currentlySelectedItemIndex == 0)
                currentlySelectedItemIndex = MAX_ITEMS - 1;
            else
                currentlySelectedItemIndex--;
            highlight.transform.SetParent(hotbarImages[currentlySelectedItemIndex].gameObject.transform, false);
        }
        
    }

    private void CheckDeath() {
        if (!deathScreenActive && managerAggression >= 1) {
            PlayerLogic.playerObject.GetComponent<Animator>().SetBool("Dead", true);
            deathScreenActive = true;
            deathMenu.enabled = true;
            StopCoroutine(levelTimer);

            if (gameIsPaused)
            {
                pauseMenu.enabled = false;
                gameIsPaused = false;
                Time.timeScale = 1;
            }
            else {
                playerHUD.enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
            UpdateAggressionbar();
    }

    /**************************/


    /* Scene Managemnt Functions */

    public void StartLevel() {
        //For Testing
        //scenesToLoad.Add(SceneManager.LoadSceneAsync("Level 2"));
        //scenesToLoad.Add(SceneManager.LoadSceneAsync("Level 3"));

        scenesToLoad.Add(SceneManager.LoadSceneAsync(currentDay));
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        powerupMenu = null;
        playerHUD.enabled = true;
        sceneTextBox.text = "Day " + currentDay;

        if (currentDay == 1)
        {
            tutorialScript.StartTutorial();
            time = TUTORIAL_TIME;
        }
        else {
            tutorialScript.StopTutorial();
            time = LEVEL_TIME;
        }
        
        levelTime = time;
        UpdateTimeBox();
        UpdateMoney(BASE_MONEY * (currentDay - 1));
        currentDay++;

        ResetItems();
        GetNewTasks();

        levelTimer = StartCoroutine(LevelTimer());
    }

    public void RestartGame() {
        currentDay = 1;
        playerMoney = 1;
        managerAggression = 0f;
        numItems = 0;
        deathScreenActive = false;
        deathMenu.enabled = false;
        
        tutorialScript.StopTutorial();
        StopAllCoroutines();
        ResetAllTasks();
        ResetTasks();
        ResetPowerups();
        StartLevel();
    }

    private void ResetPowerups() {
        powerup1.sprite = null;
        powerup1.enabled = false;

        powerup2.sprite = null;
        powerup2.enabled = false;

        trashCanPowerEnabled = false;
        throwingPowerEnabled = false;
        stickyHandPowerEnabled = false;
    }

    //Also opens end scene if its called after the last day
    public void OpenPowerupsScene() {
        if (currentDay > NUM_LEVELS)
            scenesToLoad.Add(SceneManager.LoadSceneAsync("End Screen"));
        else
            scenesToLoad.Add(SceneManager.LoadSceneAsync("Powerups"));

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        playerHUD.enabled = false;
        sceneTextBox.text = "";
        timeTextBox.text = "";
    }

    public void SetCurrentLevelAudio(AudioSource audio) {
        currentLevelMusic = audio;
    }

    /*****************************/



    /* Menu Functions */
    private void Pause()
    {
        gameIsPaused = true;
        playerHUD.enabled = false;
        pauseMenu.enabled = true;
        currentLevelMusic.Pause();

        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    //Called by Resume button in pause menu
    public void Resume()
    {
        gameIsPaused = false;
        pauseMenu.enabled = false;
        playerHUD.enabled = true;
        currentLevelMusic.Play();

        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //Called by Info button in pause and start screen
    public void OpenInfoScreen()
    {
        if (SceneManager.GetActiveScene().name.Equals("Start Screen"))
            startMenu.enabled = false;
        else if (deathScreenActive)
            deathMenu.enabled = false;
        else if (SceneManager.GetActiveScene().name.Equals("End Screen"))
            endMenu.enabled = false;
        else
            pauseMenu.enabled = false;

        infoScreen.enabled = true;
    }

    public void CloseInfoScreen() {
        if (SceneManager.GetActiveScene().name.Equals("Start Screen"))
            startMenu.enabled = true;
        else if (deathScreenActive)
            deathMenu.enabled = true;
        else if (SceneManager.GetActiveScene().name.Equals("End Screen"))
            endMenu.enabled = true;
        else
            pauseMenu.enabled = true;

        infoScreen.enabled = false;
    }

    //Called by the Exit button in the start and pause menu
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /******************/



    /* HUD Functions */

    //Shows or hides the task view in the player HUD
    public void TaskView()
    { 
        if(tasksShowing) {
            taskViewButtonText.text = "<";
            tasks.SetActive(true);
            tasksShowing = false;
        }
        else
        {
            taskViewButtonText.text = ">";
            tasks.SetActive(false);
            tasksShowing = true;
        }
    }

    public void AddTask(GenericTask genericTask) {
        uiTaskObjs.Add(Instantiate(taskPrefab, tasks.transform));
        uiTaskObjs[numCurrentTasks].GetComponent<Task>().SetText(genericTask.TaskName);
        uiTaskObjs[numCurrentTasks].GetComponent<Task>().myTask = genericTask;
        numCurrentTasks++;

        RectTransform rt = tasks.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(BASE_PANEL_WIDTH, rt.sizeDelta.y + PANEL_TASK_EXTEND);
    }

    public void RemoveTask(GenericTask genericTask)
    {
        foreach (GameObject task in uiTaskObjs) {
            if (task.GetComponent<Task>().myTask == genericTask) {
                Destroy(task);
                uiTaskObjs.Remove(task);
                numCurrentTasks--;
                RectTransform rt = tasks.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(BASE_PANEL_WIDTH, rt.sizeDelta.y - PANEL_TASK_EXTEND);
                break;
            }
        }
    }

    private void ResetAllTasks() {
        selfCompleteTasks = 0;
        coworkerCompleteTasks = 0;
        incompleteTasks = 0;
        foreach (GenericTask task in allTasks)
        {
            task.TaskComplete = false;
            task.enabled = false;
        }
    }

    public void ResetTasks()
    {
        while (numCurrentTasks > 0) {
            Destroy(uiTaskObjs[0]);
            uiTaskObjs.RemoveAt(0);
            numCurrentTasks--;
        }
        RectTransform rt = tasks.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(BASE_PANEL_WIDTH, BASE_PANEL_HEIGHT);
        usedTasks.Clear();
    }

    IEnumerator LevelTimer()
    {
        while (time > 0)
        {
            time--;
            UpdateTimeBox();
            yield return new WaitForSeconds(1);
        }
        OpenPowerupsScene();
    }

    private void UpdateTimeBox()
    {
        int min = time / 60;
        int sec = time % 60;
        string buf = "";
        if (sec < 10)
            buf = "0";
        timeTextBox.text = min + ":" + buf + sec;
    }

    //Should always be used when changing player money
    public void UpdateMoney(int change) {
        playerMoney += change;
        moneyBox.text = "$" + playerMoney;
        if (SceneManager.GetActiveScene().name.Equals("Level 1")) {
            tutorialScript.interactedWithATM = true;
        }
    }

    public void TrashCanPowerup() {
        trashCanPowerEnabled = true;
        if (powerup1.enabled == false)
        {
            powerup1.sprite = trashCanPowerupSprite;
            powerup1.enabled = true;
        }
        else if (powerup2.enabled == false)
        {
            powerup2.sprite = trashCanPowerupSprite;
            powerup2.enabled = true;
        }
        else
            Debug.Log("This should never happen");
    }

    public void ThrowingPowerup()
    {
        throwingPowerEnabled = true;
        if (powerup1.enabled == false)
        {
            powerup1.sprite = throwingPowerupSprite;
            powerup1.enabled = true;
        }
        else if (powerup2.enabled == false)
        {
            powerup2.sprite = throwingPowerupSprite;
            powerup2.enabled = true;
        }
        else
            Debug.Log("This should never happen");
    }

    public void StickyHandPowerup()
    {
        stickyHandPowerEnabled = true;
        if (powerup1.enabled == false)
        {
            powerup1.sprite = stickyHandPowerupSprite;
            powerup1.enabled = true;
        }
        else if (powerup2.enabled == false)
        {
            powerup2.sprite = stickyHandPowerupSprite;
            powerup2.enabled = true;
        }
        else
            Debug.Log("This should never happen");
    }

    //This can be used to update the aggression bar when it is changed
    public void UpdateAggressionbar()
    {
        managerAggressionBar.value = managerAggression;
    }

    //This can be called by the player script when picking up an item, passing in the associated sprite to the function
    public void AddItem(Sprite itemImage, int index)
    {
        tutorialScript.playerAddedSprite = itemImage;
        hotbarImages[index].sprite = itemImage;
        hotbarImages[index].enabled = true;
        numItems++;
    }

    public void RemoveItem()
    {
        if (numItems <= 0)
            return;
        hotbarImages[currentlySelectedItemIndex].sprite = null;
        hotbarImages[currentlySelectedItemIndex].enabled = false;
        numItems--;
    }

    private void ResetItems() {
        for (int i = 0; i < MAX_ITEMS; i++) {
            hotbarImages[i].sprite = null;
            hotbarImages[i].enabled = false;
        }
        numItems = 0;

        highlight.transform.SetParent(hotbarImages[0].gameObject.transform, false);
        currentlySelectedItemIndex = 0;
    }

    //Finds the next avaiable index for adding an item, returns -1 if the player already has MAX_ITEMS
    public int GetNextIndex() {
        if (numItems == 0)
            return 0;
        for (int i = 0; i < MAX_ITEMS; i++) {
            if (hotbarImages[i].enabled == false)
                return i;
        }
        return -1;
    }

    public int GetSelectedIndex() {
        return currentlySelectedItemIndex;
    }

    public int GetMaxItems() {
        return MAX_ITEMS;
    }

    /*****************/

    // Placeholder for now
    private void GetNewTasks()
    {
        // If we have completed tasks, clean them out
        foreach(GenericTask task in curTasks)
        {
            if (task.TaskComplete)
            {
                RemoveTask(task);
            }
        }

        for(int i = 0; i < 3; i++)
        {
            System.Random random = new System.Random();
            int taskIndex = random.Next(0, NUM_TASKS);
            // Make sure we never repeat tasks
            while (usedTasks.Contains(taskIndex))
            {
                taskIndex = random.Next(0, NUM_TASKS);
            }
            usedTasks.Add(taskIndex);
            curTasks.Add(allTasks[taskIndex]);
            if (allTasks[taskIndex].TaskName.Equals("Participate in the Economy"))
                tutorialScript.shouldGiveATMTooltip = false;
            allTasks[taskIndex].enabled = true;
            AddTask(allTasks[taskIndex]);
            incompleteTasks++;
        }
    }
}
