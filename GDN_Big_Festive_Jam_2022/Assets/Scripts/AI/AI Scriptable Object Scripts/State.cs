using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "State", menuName = "ScriptableObjects/PluggableAI/State")]
public class State : ScriptableObject
{
    public Action[] actions;
    public Transition[] transitions;

    public void UpdateState(AIThinker thinker)
    {
        ExecuteActions(thinker);
        CheckTransitions(thinker);
    }

    void ExecuteActions(AIThinker thinker)
    {
        for(int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(thinker);
        }
    }

    void CheckTransitions(AIThinker thinker)
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            bool decisionSucceeded = transitions[i].decision.Decide(thinker);

            if (decisionSucceeded)
            {
                thinker.TransitionToState(transitions[i].trueState);
            }
            else
            {
                thinker.TransitionToState(transitions[i].falseState);
            }
        }
    }
}
