using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Steal Present", menuName = "ScriptableObjects/PluggableAI/Steal Present Decision")]
public class StealPlayerPresentDecision : Decision
{
    public override bool Decide(AIThinker thinker)
    {
        bool hasStolenPresent = StealPresent(thinker);

        return hasStolenPresent;
    }

    bool StealPresent(AIThinker thinker)
    {
        RaycastHit2D hit = Physics2D.Raycast(thinker.transform.position, Vector2.right, thinker.minStealDistance * SetRaycastDirection(thinker), thinker.playerLayer);
        Debug.DrawRay(thinker.transform.position, Vector2.right * thinker.minStealDistance * SetRaycastDirection(thinker));

        if (hit)
        {
            if(GameManager.instance.presentCount > 0 && !thinker.isHoldingPresent)
            {
                
                GameManager.instance.ChangePresentCount(-1);
                thinker.playerTarget.GetComponent<PlayerController>().Knockback(thinker.transform.position);
                thinker.isHoldingPresent = true;
                /*
                thinker.CreatePresent();
                thinker.SetPresentParent();*/
                thinker.GetPresentFromPool();
                thinker.SetPoolPresentParent();
                
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

    int SetRaycastDirection(AIThinker thinker)
    {
        return thinker.transform.position.x < thinker.playerTarget.transform.position.x ? 1 : -1;
    }
}
