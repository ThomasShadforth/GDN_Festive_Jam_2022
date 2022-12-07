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
    }

    public void SetMaxPresents()
    {
        maxPresents = 0;
        PresentObject[] presentsFound = FindObjectsOfType<PresentObject>();
        maxPresents = presentsFound.Length;
        _countdown = defaultCountdown;
        _timer = defaultTimer;
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
        if (isCountingDown)
        {
            return;
        }

        if (_timer > 0)
        {
            _timer -= GamePause.deltaTime;
            string timerDefault = string.Format("{0:0}:{1:00}", Mathf.Floor(_timer / 60), _timer % 60);
            timerText.text = timerDefault;

            if(_timer < 0)
            {
                _timer = 0;
                timerDefault = string.Format("{0:0}:{1:00}", Mathf.Floor(_timer / 60), _timer % 60);
                timerText.text = timerDefault;

                //Trigger a game over
                GameOver();
            }
        }
        

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

        //Debug.Log(goalPresents);

        //Set the text for the goal

        if(goalPresents == maxPresents)
        {
            //win the level, move to next one
            //Debug.Log("LEVEL COMPLETE!");
        }

    }

    void GameOver()
    {
        if(UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }

        StartCoroutine(LoadGameOverCo());
    }

    IEnumerator CountDownCo()
    {
        isCountingDown = true;

        yield return new WaitForSeconds(.5f);

        countDownText.gameObject.SetActive(true);

        while(_countdown > 0)
        {
            _countdown -= GamePause.deltaTime;

            string tempCountdown = string.Format("{0:00}", _countdown);
            countDownText.text = tempCountdown;

            yield return null;
        }

        isCountingDown = false;
        countDownText.gameObject.SetActive(false);
    }

    IEnumerator LoadGameOverCo()
    {
        yield return new WaitForSeconds(1f);
        SceneIndexer.lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("GameOver");
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
