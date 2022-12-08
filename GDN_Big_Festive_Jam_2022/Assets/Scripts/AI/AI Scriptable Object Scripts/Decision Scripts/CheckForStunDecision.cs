using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Check For Stun Decision", menuName = "ScriptableObjects/PluggableAI/Check Stun Decision")]
public class CheckForStunDecision : Decision
{
    public override bool Decide(AIThinker thinker)
    {
        bool willStun = CheckForStun(thinker);

        return willStun;
    }

    private bool CheckForStun(AIThinker thinker)
    {
        if (thinker.isStunned)
        {
            thinker.rb2d.velocity = Vector2.zero;

            //Drop the present
            if (thinker.targetPresent != null && thinker.isHoldingPresent)
            {
                thinker.targetPresent.ResetParent(SetPresentDropDirection(thinker));
                thinker.isHoldingPresent = false;
                thinker.targetPresent = null;
            }

            if(thinker.presentTarget != null && thinker.isHoldingPresent)
            {
                thinker.presentTarget.ResetParent(SetPresentDropDirection(thinker), CheckWallDist(thinker));
                thinker.isHoldingPresent = false;
                thinker.presentTarget = null;

            }

            return true;
        }
        else
        {
            return false;
        }
    }

    int SetPresentDropDirection(AIThinker thinker)
    {
        return thinker.transform.position.x < thinker.playerTarget.transform.position.x ? -1 : 1;
    }

    Transform CheckWallDist(AIThinker thinker)
    {
        RaycastHit2D hit = Physics2D.Raycast(thinker.transform.position, thinker.transform.right, thinker.jumpCheckDist * SetPresentDropDirection(thinker), thinker.whatIsGround);

        if (hit)
        {
            return thinker.otherSidePos;
        }
        else
        {
            return null;
        }
    }
}
