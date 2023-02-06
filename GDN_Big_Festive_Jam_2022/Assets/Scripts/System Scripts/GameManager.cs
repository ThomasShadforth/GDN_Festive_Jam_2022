using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    

    public TextMeshProUGUI countText;
    public TextMeshProUGUI countDownText;
    public TextMeshProUGUI timerText;


    public int presentCount { get; private set; }
    public int goalPresents { get; private set; }
    public bool isCountingDown;
    int maxPresents;
    public float defaultCountdown;
    public float defaultTimer;
    float _timer;
    float _countdown;

    private void Awake()
    {
        
    }

    

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetMaxPresents();
        //AudioManager.instance.Play(SceneManager.GetActiveScene().name);
    }

    public void SetMaxPresents()
    {
        //Sets default values (max presents, the countdown, timer for the game, etc.)
        maxPresents = 0;
        PresentObject[] presentsFound = FindObjectsOfType<PresentObject>();
        maxPresents = presentsFound.Length;
        _countdown = defaultCountdown;
        _timer = defaultTimer;
        StagePitPosition.lastCheckPointPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        string tempCountdown = string.Format("{0:00}", _countdown);
        string timerDefault = string.Format("{0:0}:{1:00}", Mathf.Floor(_timer/60), _timer % 60);
        countDownText.text = tempCountdown;
        timerText.text = timerDefault;
        presentCount = 0;
        StartCoroutine(CountDownCo());
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        

        countText.text = "Present Count: " + presentCount;
    }

    // Update is called once per frame
    void Update()
    {
        //Do not execute while the game is starting
        if (isCountingDown)
        {
            return;
        }

        //count down the game's timer while it is greater than 0
        if (_timer > 0)
        {
            _timer -= GamePause.deltaTime;
            string timerDefault = string.Format("{0:0}:{1:00}", Mathf.Floor(_timer / 60), _timer % 60);
            timerText.text = timerDefault;

            if(_timer < 0)
            {
                //Trigger game over after the timer is set to 0
                _timer = 0;
                timerDefault = string.Format("{0:0}:{1:00}", Mathf.Floor(_timer / 60), _timer % 60);
                timerText.text = timerDefault;

                //Trigger a game over
                GameOver();
            }
        }
        

    }

    //Called when picking up a present, throwing a present, dunking a present, or when a present is stolen from the player by the elves
    public void ChangePresentCount(int presentChange)
    {
        presentCount += presentChange;

        countText.text = "Present Count: " + presentCount;

        FindObjectOfType<PlayerController>().CheckPresentCount(presentCount);
    }

    //Changes the number of presents currently stored in the goal (Used to check if the level has been completed)
    public void ChangeGoalPresents(int presentChange)
    {
        goalPresents += presentChange;

        //Debug.Log(goalPresents);

        //Set the text for the goal

        if(goalPresents == maxPresents)
        {
            //win the level, move to next one
            //Debug.Log("LEVEL COMPLETE!");
            StartCoroutine(LoadGameWinCo());
        }

    }

    //Loads the victory scene
    IEnumerator LoadGameWinCo()
    {
        UIFade.instance.FadeToBlack();

        yield return new WaitForSeconds(1f);

        //dispose of player input

        FindObjectOfType<PlayerController>()._input.Dispose();
        SceneManager.LoadScene("GameWin");
        DestroySelf();
    }

    //Fades to black, calls the game over coroutine
    void GameOver()
    {
        if(UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }

        StartCoroutine(LoadGameOverCo());
    }

    //Coroutine used to countdown at the start of the game
    IEnumerator CountDownCo()
    {
        isCountingDown = true;

        yield return new WaitForSeconds(.5f);

        //After a brief pause after loading in, start the countdown
        countDownText.gameObject.SetActive(true);

        while(_countdown > 0)
        {
            _countdown -= GamePause.deltaTime;

            string tempCountdown = string.Format("{0:00}", _countdown);
            countDownText.text = tempCountdown;

            yield return null;
        }

        //Allows the player to move after the countdown ends
        isCountingDown = false;
        countDownText.gameObject.SetActive(false);
    }

    //loads the game over scene
    IEnumerator LoadGameOverCo()
    {
        yield return new WaitForSeconds(1f);
        FindObjectOfType<PlayerController>()._input.Dispose();
        SceneIndexer.lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("GameOver");
        DestroySelf();
    }

    //destroys itself after the game ends (quit to menu, win, game over, etc.)
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
