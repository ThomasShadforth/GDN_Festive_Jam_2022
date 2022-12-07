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

        _input.Player.DropPresent.started += DepositIntoGoal;
        _input.Player.DropPresent.performed += DepositIntoGoal;
        //_input.Player.DropPresent.canceled += InteractWithGoal;
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

    void DepositIntoGoal(InputAction.CallbackContext context)
    {
        //Debug.Log("DEPOSITING");

        if (GamePause.gamePaused || GameManager.instance.isCountingDown)
        {
            return;
        }

        if (_playerInRange)
        {
            if (GameManager.instance.presentCount > 0)
            {
                if (context.started)
                {
                    GameManager.instance.ChangeGoalPresents(1);
                    GameManager.instance.ChangePresentCount(-1);
                    GameObject presentToDeposit = PresentObjectPool.instance.DepositPresent();
                    _depositedPresents.Enqueue(presentToDeposit);
                    presentsRemaining--;
                    Debug.Log(_depositedPresents.Count);
                }
            }
            else
            {
                if(presentsRemaining > 0)
                {
                    if (context.performed)
                    {
                        
                        GameManager.instance.ChangeGoalPresents(-_depositedPresents.Count);
                        GameManager.instance.ChangePresentCount(_depositedPresents.Count);
                        presentsRemaining += _depositedPresents.Count;

                        Queue<GameObject> tempPresentsQueue = new Queue<GameObject>();
                        tempPresentsQueue = _depositedPresents;

                        PresentObjectPool.instance.SetPoolQueue(tempPresentsQueue);

                        //Debug.Log(PresentObjectPool.instance.GetPoolCount());
                    }
                }
            }
        }
    }

    void InteractWithGoal(InputAction.CallbackContext context)
    {
        
        bool held = false;

        if (context.performed)
        {
            held = true;
            Debug.Log("TAKING ALL OF THE THINGS");
        }

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
                    if (context.canceled && !held)
                    {
                        GameManager.instance.ChangeGoalPresents(-1);
                        GameManager.instance.ChangePresentCount(1);
                        PresentObjectPool.instance.RetrievePresent(_depositedPresents.Dequeue());
                        presentsRemaining++;
                    } else if (context.performed)
                    {
                        GameManager.instance.ChangeGoalPresents(-_depositedPresents.Count);
                        GameManager.instance.ChangePresentCount(_depositedPresents.Count);
                        presentsRemaining += _depositedPresents.Count;

                        for(int i = 0; i < _depositedPresents.Count; i++)
                        {
                            PresentObjectPool.instance.RetrievePresent(_depositedPresents.Dequeue());

                        }
                    }
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
        
    }
}
