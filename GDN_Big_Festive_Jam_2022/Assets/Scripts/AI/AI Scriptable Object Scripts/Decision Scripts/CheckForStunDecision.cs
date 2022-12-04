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
            thinker.targetPresent.ResetParent(SetPresentDropDirection(thinker));
            thinker.targetPresent = null;

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
}
