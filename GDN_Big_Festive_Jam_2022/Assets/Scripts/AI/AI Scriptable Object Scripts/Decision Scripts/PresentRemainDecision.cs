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
        if (thinker.targetPresent != null)
        {
            
            return true;
        }
        else if(thinker.presentTarget != null)
        {
            return true;
        }
        else
        {
            Debug.Log("NO PRESENT");
            thinker.targetPresent = null;
            return false;
        }
    }

    
}
