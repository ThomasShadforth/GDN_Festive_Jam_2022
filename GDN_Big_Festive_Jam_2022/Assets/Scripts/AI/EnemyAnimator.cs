using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    Animator _animator;

    bool stunAnimPlaying;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetStunAnim()
    {
        _animator.Play("Elf_Idle");
    }

    public void AnimateAI(AIThinker thinker)
    {
        if (thinker.rb2d.velocity.x != 0)
        {
            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }

        if (thinker.isStunned && !stunAnimPlaying)
        {
            _animator.Play("Stunned");
            stunAnimPlaying = true;
        }

        if (!thinker.isStunned && stunAnimPlaying)
        {
            ResetStunAnim();
            stunAnimPlaying = false;
        }

        if (!thinker.grounded)
        {
            if(thinker.rb2d.velocity.y > 0)
            {
                _animator.SetBool("isFalling", false);
                _animator.SetBool("isJumping", true);
            }

            if(thinker.rb2d.velocity.y < 0)
            {
                _animator.SetBool("isFalling", true);
                _animator.SetBool("isJumping", false);
            }
        }
        else
        {
            _animator.SetBool("isFalling", false);
            _animator.SetBool("isJumping", false);
        }
    }
}
