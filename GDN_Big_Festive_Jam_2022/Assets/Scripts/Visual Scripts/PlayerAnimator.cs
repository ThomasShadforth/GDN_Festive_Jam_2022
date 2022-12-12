using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator _animator;
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        AnimatePlayer();
    }

    public void AnimatePlayer()
    {
        if(player._rb2d.velocity.x != 0)
        {
            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }

        if (!player.grounded)
        {
            if(player._rb2d.velocity.y > 0)
            {
                _animator.SetBool("isJumping", true);
                _animator.SetBool("isFalling", false);
                
            }

            if(player._rb2d.velocity.y < 0)
            {
                _animator.SetBool("isJumping", false);
                _animator.SetBool("isDoubleJumping", false);
                _animator.SetBool("isFalling", true);
            }
        }
        else
        {
            
            _animator.SetBool("isJumping", false);
            _animator.SetBool("isFalling", false);
        }
    }
}
