using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PresentStates
{
    pickup,
    enemy
}

public class Present : MonoBehaviour
{
    public GameObject interactIcon;

    public float interactRadius;

    public PresentStates currentState;

    Rigidbody2D _rb2d;

    bool _canPickup;

    [SerializeField] Collider2D _trigger;
    public Collider2D _collider;

    private void Start()
    {
        
        _rb2d = GetComponent<Rigidbody2D>();

        if (!_trigger.enabled)
        {
            Invoke("EnableCollider", 1f);
        }

    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentState == PresentStates.pickup)
        {
            GameManager.instance.ChangePresentCount(1);
            Destroy(gameObject);
        }
    }

    
    public void ResetParent(int directionToThrow)
    {
        transform.parent = null;
        _rb2d.isKinematic = false;
        _collider.enabled = true;
        _rb2d.velocity = new Vector2(7f * directionToThrow, _rb2d.velocity.y);
        ChangePresentState();
    }


    public void ChangePresentState()
    {
        if(currentState == PresentStates.pickup)
        {
            currentState = PresentStates.enemy;
            
            //Write in logic to set parent to the enemy's hands
        }
        else
        {
            currentState = PresentStates.pickup;
        }
    }

    void EnableCollider()
    {
        _trigger.enabled = true;
    }

}
