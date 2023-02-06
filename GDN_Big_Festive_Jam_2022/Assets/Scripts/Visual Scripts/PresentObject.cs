using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentObject : MonoBehaviour
{

    public Transform handToAppearIn;

    public PresentStates currentState = PresentStates.pickup;

    Rigidbody2D _rb2d;

    [HideInInspector]
    public Vector2 originalPosition;

    bool _canPickup;
    [HideInInspector]
    public bool _droppedOrThrown;

    [SerializeField] Collider2D _trigger;
    public Collider2D collider;

    private void OnEnable()
    {
        //Apply the velocity depending on the direction faced (DO NOT APPLY ON START)
        //Set position to the player/enemy hand (depending on what is set)
        if (_droppedOrThrown)
        {
            
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        Invoke("EnableCollider", .5f);

        originalPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Adds a present to the count after picking it up
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && currentState == PresentStates.pickup)
        {
            _trigger.enabled = false;
            //Increase count by 1
            GameManager.instance.ChangePresentCount(1);
            //Adds it to the object pool (sets it inactive)
            PresentObjectPool.instance.AddToPool(this.gameObject);
        }
    }

    public void ResetParent(int directionToSet, Transform positionToSet)
    {
        transform.parent = null;
        collider.enabled = true;
        if (positionToSet != null)
        {
            transform.position = positionToSet.position;
            directionToSet = -directionToSet;
        }

        _rb2d.isKinematic = false;
        
        _rb2d.velocity = new Vector2(directionToSet, .5f) * 6f;
        ChangePresentState();
    }

    public void ChangePresentState()
    {
        if(currentState == PresentStates.enemy)
        {
            currentState = PresentStates.pickup;
        }
        else
        {
            currentState = PresentStates.enemy;
        }
    }

    public void EnableCollider()
    {
        
        _trigger.enabled = true;
    }
}
