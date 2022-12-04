using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : ScriptableObject
{
    // Start is called before the first frame update
    public abstract void Act(AIThinker thinker);
    
}
