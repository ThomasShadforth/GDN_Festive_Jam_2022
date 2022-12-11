using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjects : MonoBehaviour, IDamageInterface
{
    public int objectDurability;

    public float breakKnockForce;

    public LayerMask brokenObjectLayer;

    Transform _playerPosition;

    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void DamageObject(int damageTaken)
    {
        objectDurability -= damageTaken;
        if(objectDurability <= 0)
        {
            _playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
            if(_animator != null)
            {
                _animator.SetBool("isBroken", true);
                gameObject.layer = LayerMask.NameToLayer("BrokenObjs");
            }
            //gameObject.SetActive(false);
        }
    }

    public void BreakObject()
    {
        
        Vector2 direction = transform.position - _playerPosition.position;
        direction = direction.normalized;
        direction.y = .5f;

        GetComponent<Rigidbody2D>().velocity = direction * breakKnockForce;
        
    }

    public void SetObjectInactive()
    {
        gameObject.SetActive(false);
    }

    void SetKinematic()
    {

    }
}
