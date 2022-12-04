using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Active Chase", menuName = "ScriptableObjects/PluggableAI/Active Chase")]
public class ActiveChaseDecision : Decision
{
    public float chaseRadius = 2f;

    public override bool Decide(AIThinker thinker)
    {
        bool inRadius = CheckForRadius(thinker);

        return inRadius;
    }

    bool CheckForRadius(AIThinker thinker)
    {
        if(Vector2.Distance(thinker.transform.position, thinker.playerTarget.transform.position) <= chaseRadius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
