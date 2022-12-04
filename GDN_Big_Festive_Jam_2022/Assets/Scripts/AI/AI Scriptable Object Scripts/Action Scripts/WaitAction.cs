using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wait", menuName = "ScriptableObjects/PluggableAI/Wait")]
public class WaitAction : Action
{
    public override void Act(AIThinker thinker)
    {
        Wait(thinker);
    }

    void Wait(AIThinker thinker)
    {
        thinker.rb2d.velocity = Vector2.zero;
        thinker.waitTimer -= Time.deltaTime;
    }
}
