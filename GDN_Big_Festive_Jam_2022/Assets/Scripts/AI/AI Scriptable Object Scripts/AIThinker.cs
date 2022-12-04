using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIThinker : MonoBehaviour
{
    public State currentState;
    public State remainState;

    [HideInInspector]
    public Rigidbody2D rb2d;

    public Transform[] patrolPointList;

    [HideInInspector]
    public GameObject playerTarget;

    public float moveSpeed;
    public float waitTime = 2f;
    [HideInInspector]
    public float waitTimer;
    public float minPatrolDist = .5f;

    public int currentWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        waitTimer = waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void TransitionToState(State nextState)
    {
        if(nextState != remainState)
        {
            currentState = nextState;
        }
    }

    public void SetWaitTimer()
    {
        waitTimer = waitTime;
    }
}
