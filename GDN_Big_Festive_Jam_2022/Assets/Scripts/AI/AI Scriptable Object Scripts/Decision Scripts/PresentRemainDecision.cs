using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Check remaining presemt", menuName = "ScriptableObjects/PluggableAI/Remaining Present Decision")]
public class PresentRemainDecision : Decision
{
    public override bool Decide(AIThinker thinker)
    {
        bool presentRemain = CheckPresent(thinker);
        return presentRemain;
    }

    public bool CheckPresent(AIThinker thinker)
    {
        if(Physics2D.OverlapCircle(thinker.transform.position, thinker.minChaseDistance, thinker.presentsLayer))
        {
            return true;
        }
        else
        {
            thinker.presentTarget = null;
            return false;
        }
    }

    
}
