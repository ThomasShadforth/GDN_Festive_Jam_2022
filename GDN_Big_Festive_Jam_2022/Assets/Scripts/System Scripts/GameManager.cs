using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI countText;

    public int presentCount { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
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
}
