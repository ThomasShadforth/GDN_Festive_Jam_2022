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
            /*
            thinker.SetPresentParent();
            thinker.isHoldingPresent = true;
            thinker.playerTarget = GameObject.FindGameObjectWithTag("Player");*/

            //thinker.heldPresentCount = 1;

            thinker.SetPoolPresentParent();
            thinker.isHoldingPresent = true;
            thinker.playerTarget = GameObject.FindGameObjectWithTag("Player");

            return true;
        } else if (thinker.AIHandPos.GetComponentInChildren<PresentObject>())
        {
            thinker.isHoldingPresent = true;
            thinker.playerTarget = GameObject.FindGameObjectWithTag("Player");
            return true;
        }
        else
        {
            return false;
        }

        
    }

    int CheckPresentDirection(AIThinker thinker)
    {
        if (thinker.targetPresent != null || thinker.presentTarget != null)
        {
            return thinker.transform.position.x < thinker.presentTarget.transform.position.x ? 1 : -1;
        }

        return 0;
    }
}
