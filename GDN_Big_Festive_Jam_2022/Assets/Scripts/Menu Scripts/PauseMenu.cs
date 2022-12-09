using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public PlayerInputActions _input;

    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _menuButtonSelected;
    [SerializeField] GameObject _settingsMenu;
    [SerializeField] GameObject _settingsButtonSelected;

    // Start is called before the first frame update
    void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();
        _input.Player.Pause.started += PauseGame;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if(UIFade.instance.fading || GameManager.instance.isCountingDown)
        {
            return;
        }

        if (context.started)
        {
            if (!_pauseMenu.activeInHierarchy)
            {
                //Pause the game
                GamePause.gamePaused = true;
                _input.Player.Disable();
                _input.UI.Enable();
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(_menuButtonSelected);
            }
            

            _pauseMenu.SetActive(!_pauseMenu.activeInHierarchy);
        }
    }

    public void UnpauseGame()
    {
        GamePause.gamePaused = false;
        _input.Player.Enable();
        _input.UI.Disable();
        _pauseMenu.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        _settingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_settingsButtonSelected);
        _settingsMenu.GetComponent<SettingsMenu>().SetAudioSliders();
    }

    public void CloseSettingsMenu()
    {
        _settingsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_menuButtonSelected);
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartLevelCo());
    }

    IEnumerator RestartLevelCo()
    {
        GamePause.gamePaused = false;
        if (UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }
        yield return new WaitForSeconds(1f);
        GameManager.instance.DestroySelf();
        FindObjectOfType<PlayerController>()._input.Dispose();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        GamePause.gamePaused = false;

        if(UIFade.instance != null)
        {
            UIFade.instance.FadeToBlack();
        }
        
        StartCoroutine(QuitToMenuCo());
        //Return to the main menu via scenemanager
    }

    IEnumerator QuitToMenuCo()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.DestroySelf();
        AudioManager.instance.DestroySelf();
        
        FindObjectOfType<PlayerController>()._input.Dispose();
        SceneManager.LoadScene(0);
        //UIFade.instance.DestroySelf();
    }
}
