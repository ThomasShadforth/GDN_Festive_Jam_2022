using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stunned", menuName = "ScriptableObjects/PluggableAI/Stunned")]
public class StunnedAction : Action
{
    public override void Act(AIThinker thinker)
    {
        CountdownStun(thinker);
    }

    void CountdownStun(AIThinker thinker)
    {
        thinker.stunTimer -= Time.deltaTime;
    }
}
