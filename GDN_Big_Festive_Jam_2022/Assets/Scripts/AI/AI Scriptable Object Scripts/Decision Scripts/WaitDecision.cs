using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wait Check", menuName = "ScriptableObjects/PluggableAI/Wait Check")]
public class WaitDecision : Decision
{
    public override bool Decide(AIThinker thinker)
    {
        bool finishedWaiting = checkWaiting(thinker);

        return finishedWaiting;
    }

    bool checkWaiting(AIThinker thinker)
    {
        if(thinker.waitTimer <= 0)
        {
            thinker.waitTimer = thinker.waitTime;
            return true;
        }
        else
        {
            return false;
        }
    }
}
