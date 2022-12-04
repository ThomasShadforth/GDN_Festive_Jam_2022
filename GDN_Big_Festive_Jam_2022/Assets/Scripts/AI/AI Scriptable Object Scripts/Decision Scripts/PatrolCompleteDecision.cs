
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrol Complete", menuName = "ScriptableObjects/PluggableAI/Patrol Complete Decision")]
public class PatrolCompleteDecision : Decision
{
    public override bool Decide(AIThinker thinker)
    {
        bool isNearWaypoint = CheckWaypointDistance(thinker);

        return isNearWaypoint;
    }

    private bool CheckWaypointDistance(AIThinker thinker)
    {
        if(Vector2.Distance(thinker.transform.position, thinker.patrolPointList[thinker.currentWaypoint].position) <= thinker.minPatrolDist)
        {
            thinker.currentWaypoint++;
            if(thinker.currentWaypoint >= thinker.patrolPointList.Length)
            {
                thinker.currentWaypoint = 0;
            }

            return true;
        }
        else
        {
            return false;
        }
    }
}
