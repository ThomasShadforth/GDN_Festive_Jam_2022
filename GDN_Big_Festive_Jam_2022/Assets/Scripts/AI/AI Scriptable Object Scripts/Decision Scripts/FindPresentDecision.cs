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
        if(Physics2D.OverlapCircle(thinker.transform.position, 2f, thinker.presentsLayer))
        {
            thinker.targetPresent = Physics2D.OverlapCircle(thinker.transform.position, 2f, thinker.presentsLayer).GetComponent<Present>();
            return true;
        }
        else
        {
            return false;
        }

        
    }
}
