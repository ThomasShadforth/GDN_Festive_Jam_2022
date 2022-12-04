using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "PatrolAction", menuName = "ScriptableObjects/PluggableAI/Patrol")]
public class PatrolAction : Action
{
    public float minimumDistanceFromPoint;

    public override void Act(AIThinker thinker)
    {
        Patrol(thinker);
    }

    void Patrol(AIThinker thinker)
    {
        //Have the AI move toward it's next waypoint
        thinker.rb2d.velocity = new Vector2(thinker.moveSpeed * SetDirection(thinker), thinker.rb2d.velocity.y);
    }

    int SetDirection(AIThinker thinker)
    {
        return thinker.patrolPointList[thinker.currentWaypoint].position.x > thinker.transform.position.x ? 1 : -1;
    }
}
