using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIFade : MonoBehaviour
{
    bool _shouldFadeFromBlack;
    bool _shouldFadeToBlack;

    public bool fading;

    public Image fadeImage;

    public static UIFade instance;

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
        if(fadeImage.color.a == 1f)
        {
            FadeFromBlack();
        }
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
    }

    // Update is called once per frame
    void Update()
    {
        if (_shouldFadeToBlack)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.MoveTowards(fadeImage.color.a, 1, 2 * GamePause.deltaTime));
        }

        if (_shouldFadeFromBlack)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.MoveTowards(fadeImage.color.a, 0f, 2 * GamePause.deltaTime));

            if (fadeImage.color.a == 0f)
            {
                fadeImage.gameObject.SetActive(false);
                _shouldFadeFromBlack = false;
                
            }
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void FadeToBlack()
    {
        fading = true;
        fadeImage.gameObject.SetActive(true);
        _shouldFadeToBlack = true;
        _shouldFadeFromBlack = false;
    }

    public void FadeFromBlack()
    {
        fading = false;
        _shouldFadeFromBlack = true;
        _shouldFadeToBlack = false;
    }
}
