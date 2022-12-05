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
            if(GameManager.instance.presentCount > 0)
            {
                GameManager.instance.ChangePresentCount(-1);
                thinker.playerTarget.GetComponent<PlayerController>().Knockback(thinker.transform.position);
                thinker.CreatePresent();
                thinker.SetPresentParent();
                
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

        /*
        if(Vector2.Distance(thinker.transform.position, thinker.playerTarget.transform.position) <= thinker.minStealDistance)
        {
            if (GameManager.instance.presentCount > 0)
            {
                GameManager.instance.ChangePresentCount(-1);
                thinker.CreatePresent();
                thinker.SetPresentParent();
                //spawn a present in the hand, deactivate collider and rigidbody
                return true;
            }
        }
        else
        {
            return false;
        }*/

        


        
    }

    int SetRaycastDirection(AIThinker thinker)
    {
        return thinker.transform.position.x < thinker.playerTarget.transform.position.x ? 1 : -1;
    }
}
