using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBag : MonoBehaviour
{
    public LayerMask whatIsWall;

    public Transform wallCheckPos;

    private void OnEnable()
    {
        bool stuck = Physics2D.OverlapCircle(transform.position, .25f, whatIsWall);

        if (stuck)
        {
            StartCoroutine(MovePlayerFromWallCo());
        }
        else
        {
            Debug.Log("NOT DETECTED");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(wallCheckPos.position, .25f);
    }

    IEnumerator MovePlayerFromWallCo()
    {
        UIFade.instance.FadeToBlack();
        FindObjectOfType<PlayerController>()._beingMoved = true;
        yield return new WaitForSeconds(1f);
        FindObjectOfType<PlayerController>().transform.position = StagePitPosition.lastCheckPointPos;
        UIFade.instance.FadeFromBlack();
        FindObjectOfType<PlayerController>()._beingMoved = false;

    }
}
