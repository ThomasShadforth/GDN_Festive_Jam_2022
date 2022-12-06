using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    

    public TextMeshProUGUI countText;

    public int presentCount { get; private set; }
    public int goalPresents { get; private set; }
    int maxPresents;


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
        PresentObject[] presentsFound = FindObjectsOfType<PresentObject>();
        maxPresents = presentsFound.Length;
        
        presentCount = 0;
        
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
}
