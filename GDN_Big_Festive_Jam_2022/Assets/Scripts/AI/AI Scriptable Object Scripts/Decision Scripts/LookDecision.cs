using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Look Decision", menuName = "ScriptableObjects/PluggableAI/Look")]
public class LookDecision : Decision
{
    public float checkRadius = 2f;

    public override bool Decide(AIThinker thinker)
    {
        bool foundPlayer = Look(thinker);
        return foundPlayer;
    }

    bool Look(AIThinker thinker)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if(Vector2.Distance(playerObj.transform.position, thinker.transform.position) <= checkRadius)
        {
            thinker.playerTarget = playerObj;
            thinker.waitTimer = thinker.waitTime;
            return true;
        }
        else
        {
            return false;
        }
    }
}
