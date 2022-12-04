using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Check Stun End Decision", menuName = "ScriptableObjects/PluggableAI/Check Stun End Decision")]
public class StunFinishedDecision : Decision
{
    public override bool Decide(AIThinker thinker)
    {
        bool stunFinished = CheckForStunEnd(thinker);

        return stunFinished;
    }

    bool CheckForStunEnd(AIThinker thinker)
    {
        if(thinker.stunTimer <= 0)
        {
            thinker.stunTimer = thinker.stunTime;
            thinker.isStunned = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
