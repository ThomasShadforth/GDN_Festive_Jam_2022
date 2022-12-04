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
            thinker.targetPresent.ResetParent();
            thinker.targetPresent = null;

            return true;
        }
        else
        {
            return false;
        }
    }
}
