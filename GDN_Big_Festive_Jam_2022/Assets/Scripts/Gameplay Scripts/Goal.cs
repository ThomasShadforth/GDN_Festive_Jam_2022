using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Goal : MonoBehaviour
{
    PlayerInputActions _input;

    bool _playerInRange;

    int presentsRemaining;

    public TextMeshProUGUI presentsRemainingText;

    Queue<GameObject> _depositedPresents = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();

        _input.Player.DropPresent.performed += InteractWithGoal;
        presentsRemaining = FindObjectsOfType<PresentObject>().Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInRange)
        {
            presentsRemainingText.gameObject.SetActive(true);

            if (presentsRemaining > 0)
            {
                presentsRemainingText.text = presentsRemaining + " presents left!";
            }
            else
            {
                presentsRemainingText.text = "All presents found!";
            }
        }
        else
        {
            presentsRemainingText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
        }
    }

    void InteractWithGoal(InputAction.CallbackContext context)
    {
        if(GamePause.gamePaused || GameManager.instance.isCountingDown)
        {
            return;
        }

        if (_playerInRange)
        {
            if (GameManager.instance.presentCount == 0)
            {
                if(presentsRemaining > 0)
                {
                    GameManager.instance.ChangeGoalPresents(-1);
                    GameManager.instance.ChangePresentCount(1);
                    PresentObjectPool.instance.RetrievePresent(_depositedPresents.Dequeue());
                    presentsRemaining++;
                }
                
            }
            else
            {
                GameManager.instance.ChangeGoalPresents(1);
                GameManager.instance.ChangePresentCount(-1);
                _depositedPresents.Enqueue(PresentObjectPool.instance.DepositPresent());
                presentsRemaining--;
            }
        }
        else
        {
            Debug.Log("NOT NEAR GOAL");
        }
    }
}
