using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOver : MonoBehaviour
{
    PlayerInputActions _input;

    [SerializeField] GameObject _firstGameOverButton;

    // Start is called before the first frame update
    void Start()
    {
        if(AudioManager.instance != null)
        {
            AudioManager.instance.Play("GameOver");
        }

        _input = new PlayerInputActions();
        _input.UI.Enable();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_firstGameOverButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RetryLevel()
    {
        if(UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }

        StartCoroutine(RetryLevelCo());
    }

    public void QuitToMenu()
    {
        if(UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }

        StartCoroutine(QuitToMenuCo());
    }

    IEnumerator QuitToMenuCo()
    {
        yield return new WaitForSeconds(1f);
        if (AudioManager.instance != null && GameManager.instance != null)
        {
            AudioManager.instance.DestroySelf();
            GameManager.instance.DestroySelf();
        }
        //UIFade.instance.DestroySelf();

        SceneManager.LoadScene(0);
    }

    IEnumerator RetryLevelCo()
    {
        yield return new WaitForSeconds(1f);

        if(SceneIndexer.lastSceneIndex == 0)
        {
            SceneManager.LoadScene(1);
            StopCoroutine(RetryLevelCo());
        }

        if(AudioManager.instance != null && GameManager.instance != null)
        {
            //AudioManager.instance.DestroySelf();
            GameManager.instance.DestroySelf();
        }

        SceneManager.LoadScene(SceneIndexer.lastSceneIndex);
    }
}
