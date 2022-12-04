using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase", menuName = "ScriptableObjects/PluggableAI/Chase")]
public class ChaseAction : Action
{
    public int chaseSpeed;

    public override void Act(AIThinker thinker)
    {
        ChasePlayer(thinker);
    }

    void ChasePlayer(AIThinker thinker)
    {
        thinker.rb2d.velocity = new Vector2(chaseSpeed * SetMoveDirection(thinker), thinker.rb2d.velocity.y);
    }

    int SetMoveDirection(AIThinker thinker)
    {
        return thinker.transform.position.x < thinker.playerTarget.transform.position.x ? 1 : -1;
    }
}
