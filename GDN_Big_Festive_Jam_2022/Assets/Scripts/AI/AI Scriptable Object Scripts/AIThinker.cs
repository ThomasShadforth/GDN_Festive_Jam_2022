using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AIThinker : MonoBehaviour
{
    public State currentState;
    public State remainState;

    [HideInInspector]
    public Rigidbody2D rb2d;

    public Transform[] patrolPointList;

    [HideInInspector]
    public GameObject playerTarget;
    public Present targetPresent;
    public Present presentPrefab;

    public Transform AIHandPos;

    public LayerMask presentsLayer;
    public LayerMask playerLayer;

    public float moveSpeed;
    public float waitTime = 2f;
    [HideInInspector]
    public float waitTimer;
    public float stunTime = 3f;
    [HideInInspector]
    public float stunTimer;
    [HideInInspector]
    public bool isStunned;
    public float minPatrolDist = .5f;
    public float minStealDistance = 1.5f;
    public float minChaseDistance = 4f;

    public int currentWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        waitTimer = waitTime;
        stunTimer = stunTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePause.gamePaused)
        {
            rb2d.velocity = Vector2.zero;
            return;
        }

        currentState.UpdateState(this);
        CheckForXScale();
    }

    public void TransitionToState(State nextState)
    {
        if(nextState != remainState)
        {
            currentState = nextState;
        }
    }

    public void CreatePresent()
    {
        targetPresent = Instantiate(presentPrefab, AIHandPos.position, Quaternion.identity);
        StartCoroutine(CinemachineCamShake.CamShakeCo(.1f, FindObjectOfType<CinemachineVirtualCamera>()));
    }

    public void SetPresentParent()
    {
        targetPresent.transform.parent = AIHandPos;
        targetPresent.transform.position = AIHandPos.position;
        targetPresent.GetComponent<Rigidbody2D>().isKinematic = true;
        targetPresent._collider.enabled = false;
        targetPresent.ChangePresentState();
    }

    public void SetWaitTimer()
    {
        waitTimer = waitTime;
    }

    public void CheckForXScale()
    {
        Vector3 scalar = transform.localScale;
        if(rb2d.velocity.x > 0 && transform.localScale.x < 0)
        {
            scalar.x = Mathf.Abs(scalar.x) * 1;
        } else if(rb2d.velocity.x < 0 && transform.localScale.x > 0)
        {
            scalar.x = Mathf.Abs(scalar.x) * -1;
        }

        transform.localScale = scalar;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(transform.position, minChaseDistance);
    }
}
