using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickup present", menuName = "ScriptableObjects/PluggableAI/Pickup Present")]
public class PickupPresentDecision : Decision
{
    public override bool Decide(AIThinker thinker)
    {
        bool hasPickedUp = CheckPresentDist(thinker);

        return hasPickedUp;
    }

    private bool CheckPresentDist(AIThinker thinker)
    {
        if(Vector2.Distance(thinker.transform.position, thinker.targetPresent.transform.position) < thinker.minStealDistance)
        {
            //Pickup the present
            thinker.SetPresentParent();
            thinker.playerTarget = GameObject.FindGameObjectWithTag("Player");
            return true;
        }
        else
        {
            return false;
        }
    }
}
