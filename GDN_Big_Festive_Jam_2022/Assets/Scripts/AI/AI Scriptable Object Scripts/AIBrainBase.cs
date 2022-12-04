using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AI Brain Base", menuName = "Scriptable Objects/AI Brains/AI Brain Base")]
public class AIBrainBase : ScriptableObject
{
    public virtual void Think(AIThinker ai)
    {

    }
}
