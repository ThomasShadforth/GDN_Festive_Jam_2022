using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            StagePitPosition.lastCheckPointPos = transform.position;

            Debug.Log("CHECKPOINT IS SET TO: " + StagePitPosition.lastCheckPointPos);
        }
    }
}
