using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    PlayerInputActions _input;

    [SerializeField] GameObject _titleScreen;
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _settingsMenu;
    [SerializeField] Button _firstMenuButton;
    [SerializeField] Button _settingsMenuButton;
    
    

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        _input = new PlayerInputActions();
        _input.UI.Enable();
        _input.UI.Submit.started += OpenMenu;
        _input.UI.Click.started += OpenMenu;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenMenu(InputAction.CallbackContext context)
    {
        //Open the main menu from the title screen
        //Check if it is open. if not, open it, else ignore it
        if (!_mainMenu.activeInHierarchy)
        {
            Debug.Log("MAIN MENU OPENING!!");
            _titleScreen.SetActive(false);
            _mainMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            StartCoroutine(SetButtonSelectCo(_firstMenuButton));
            _input.UI.Submit.started -= OpenMenu;
            _input.UI.Click.started -= OpenMenu;
        }
    }

    public IEnumerator SetButtonSelectCo(Button _buttonToSelect)
    {
        yield return new WaitForSeconds(.2f);
        EventSystem.current.SetSelectedGameObject(_buttonToSelect.gameObject);
    }

    public void OpenSettingsMenu()
    {
        _settingsMenu.SetActive(true);
        StartCoroutine(SetButtonSelectCo(_settingsMenuButton));
    }

    public void CloseSettingsMenu()
    {
        _settingsMenu.SetActive(false);
        StartCoroutine(SetButtonSelectCo(_firstMenuButton));
    }

    public void StartGame()
    {
        //Start the game (Load the first level)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
