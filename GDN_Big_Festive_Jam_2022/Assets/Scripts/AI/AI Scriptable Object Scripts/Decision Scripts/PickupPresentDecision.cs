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
        RaycastHit2D hit = Physics2D.Raycast(thinker.transform.position, Vector2.right, thinker.minStealDistance * CheckPresentDirection(thinker), thinker.presentsLayer);

        if (hit)
        {
            thinker.SetPresentParent();
            thinker.playerTarget = GameObject.FindGameObjectWithTag("Player");
            return true;
        }
        else
        {
            return false;
        }

        /*
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
        }*/
    }

    int CheckPresentDirection(AIThinker thinker)
    {
        return thinker.transform.position.x < thinker.targetPresent.transform.position.x ? 1 : -1;
    }
}
