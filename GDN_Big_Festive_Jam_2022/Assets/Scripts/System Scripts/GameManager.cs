using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    

    public TextMeshProUGUI countText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;

    public float maxLevelTimer;

    public int presentCount { get; private set; }
    public int goalPresents { get; private set; }
    int maxPresents;

    [SerializeField] float _defaultCountdown;
    [SerializeField] float _countdown;

    public bool isCountingDown;
    bool countdownStarted;

    //public 

    private void Awake()
    {
        
    }

    

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetMaxPresents();
    }

    public void SetMaxPresents()
    {
        PresentObject[] presentsFound = FindObjectsOfType<PresentObject>();
        maxPresents = presentsFound.Length;
        _countdown = _defaultCountdown;
        isCountingDown = true;
        countdownStarted = false;
        presentCount = 0;
        string tempTimer = string.Format("{0:0}:{1:00}", Mathf.Floor(maxLevelTimer/60), maxLevelTimer % 60);
        timerText.text = tempTimer;

        StartCoroutine(EnableCountdownCo());

    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        countText.text = "Present Count: " + presentCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCountingDown)
        {
            if (countdownStarted)
            {
                _countdown -= Time.deltaTime;
                string tempText = string.Format("{0:00}", _countdown);
                countdownText.text = tempText;

                if(_countdown < 0)
                {
                    isCountingDown = false;
                    countdownText.gameObject.SetActive(false);
                }
            }

            return;
        }

        maxLevelTimer -= GamePause.deltaTime;
        string tempTimer = string.Format("{0:0}:{1:00}", Mathf.Floor(maxLevelTimer / 60), maxLevelTimer % 60);
        timerText.text = tempTimer;
    }

    public void ChangePresentCount(int presentChange)
    {
        presentCount += presentChange;

        countText.text = "Present Count: " + presentCount;

        FindObjectOfType<PlayerController>().CheckPresentCount(presentCount);
    }

    public void ChangeGoalPresents(int presentChange)
    {
        goalPresents += presentChange;

        //Set the text for the goal

        if(goalPresents == maxPresents)
        {
            //win the level, move to next one
            Debug.Log("LEVEL COMPLETE!");
        }

    }

    IEnumerator EnableCountdownCo()
    {
        yield return new WaitForSeconds(.5f);
        countdownText.gameObject.SetActive(true);
        countdownStarted = true;

    }
    
    public bool GetCountdown()
    {
        //Debug.Log(isCountingDown);

        return countdownText.gameObject.activeInHierarchy;
    }
}
