using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jump", menuName = "ScriptableObjects/PluggableAI/Jump Action")]
public class JumpAction : Action
{
    public override void Act(AIThinker thinker)
    {
        CheckForJump(thinker);
    }

    void CheckForJump(AIThinker thinker)
    {
        RaycastHit2D hit = Physics2D.Raycast(thinker.transform.position, thinker.transform.right, thinker.jumpCheckDist * GetMoveDirection(thinker), thinker.whatIsGround);
        Debug.DrawRay(thinker.transform.position, thinker.transform.right * thinker.jumpCheckDist * GetMoveDirection(thinker));

        if (hit && thinker.grounded)
        {
            thinker.rb2d.velocity = new Vector2(thinker.rb2d.velocity.x, 7f);
        }

    }

    int GetMoveDirection(AIThinker thinker)
    {
        return thinker.rb2d.velocity.x > 0 ? 1 : -1;
    }
}
