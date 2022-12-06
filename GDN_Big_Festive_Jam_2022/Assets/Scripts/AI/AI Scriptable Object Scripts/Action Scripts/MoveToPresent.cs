using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move To Present", menuName = "ScriptableObjects/PluggableAI/Move To Present Action")]
public class MoveToPresent : Action
{
    public override void Act(AIThinker thinker)
    {
        MoveTowardsPresent(thinker);
    }

    void MoveTowardsPresent(AIThinker thinker)
    {
        thinker.rb2d.velocity = new Vector2(thinker.moveSpeed * SetMoveDirection(thinker), thinker.rb2d.velocity.y);
    }

    int SetMoveDirection(AIThinker thinker)
    {
        if (thinker.targetPresent != null || thinker.presentTarget != null)
        {
            return thinker.presentTarget.transform.position.x > thinker.transform.position.x ? 1 : -1;
        }

        return 0;
    }
}
