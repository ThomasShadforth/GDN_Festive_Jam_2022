using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Find Present", menuName = "ScriptableObjects/PluggableAI/Find Present Decision")]
public class FindPresentDecision : Decision
{
    public override bool Decide(AIThinker thinker)
    {
        bool foundPresent = CheckForPresent(thinker);

        return foundPresent;
    }

    private bool CheckForPresent(AIThinker thinker)
    {
        if(Physics2D.OverlapCircle(thinker.transform.position, thinker.minChaseDistance, thinker.presentsLayer) && !thinker.AIHandPos.GetComponentInChildren<PresentObject>())
        {
            /*
            thinker.targetPresent = Physics2D.OverlapCircle(thinker.transform.position, thinker.minChaseDistance, thinker.presentsLayer).GetComponent<Present>();*/
            thinker.presentTarget = Physics2D.OverlapCircle(thinker.transform.position, thinker.minChaseDistance, thinker.presentsLayer).GetComponent<PresentObject>();
            if (thinker.presentTarget.currentState == PresentStates.pickup)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        
    }
}
