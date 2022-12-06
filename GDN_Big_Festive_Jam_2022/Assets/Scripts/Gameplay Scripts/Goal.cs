using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Goal : MonoBehaviour
{
    PlayerInputActions _input;

    bool _playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        _input = new PlayerInputActions();
        _input.Player.Enable();

        _input.Player.DropPresent.performed += InteractWithGoal;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if (_playerInRange)
        {
            if (GameManager.instance.presentCount == 0)
            {
                GameManager.instance.ChangeGoalPresents(-1);
                GameManager.instance.ChangePresentCount(1);
            }
            else
            {
                GameManager.instance.ChangeGoalPresents(1);
                GameManager.instance.ChangePresentCount(-1);
            }
        }
        else
        {
            Debug.Log("NOT NEAR GOAL");
        }
    }
}
