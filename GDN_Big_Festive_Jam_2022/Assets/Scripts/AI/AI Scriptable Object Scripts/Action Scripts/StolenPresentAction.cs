using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stolen Present", menuName = "ScriptableObjects/PluggableAI/Stolen Present Action")]
public class StolenPresentAction : Action
{
    public override void Act(AIThinker thinker)
    {
        RunWithPresent(thinker);
    }

    void RunWithPresent(AIThinker thinker)
    {
        thinker.rb2d.velocity = new Vector2(thinker.moveSpeed * CheckPlayerDirection(thinker), thinker.rb2d.velocity.y);
    }

    int CheckPlayerDirection(AIThinker thinker)
    {
        return thinker.playerTarget.transform.position.x < thinker.transform.position.x ? 1 : -1;
    }
}
