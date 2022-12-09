using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameWin : MonoBehaviour
{
    [SerializeField] GameObject menuButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuButton);
    }

    public void ReturnToMenu()
    {
        StartCoroutine(LoadMainMenuCo());
    }

    IEnumerator LoadMainMenuCo()
    {
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1f);

        if (AudioManager.instance != null && GameManager.instance != null)
        {
            AudioManager.instance.DestroySelf();
            GameManager.instance.DestroySelf();
        }

        SceneManager.LoadScene(0);
    }
}

