using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoll : MonoBehaviour
{
    public Collider2D bagCollider;
    public Collider2D originalCollider;
    public GameObject playerCollider;

    public Transform wallCheckPos;

    public LayerMask whatIsWall;

    Vector3 originalScale;

    private void Start()
    {
        originalScale = playerCollider.transform.localScale;
    }

    // Start is called before the first frame update
    public void PlayerRollAction(bool SetRoll)
    {
        if (SetRoll)
        {
            bagCollider.enabled = false;
            originalCollider.enabled = false;
            playerCollider.SetActive(true);
            playerCollider.transform.localScale = new Vector3(originalScale.x, originalScale.y / 2, originalScale.z);
        }
        else
        {
            bagCollider.enabled = true;
            originalCollider.enabled = true;
            playerCollider.transform.localScale = originalScale;
            playerCollider.SetActive(false);
            CheckForWall();
        }
    }

    public void CheckForWall()
    {
        bool stuck = Physics2D.OverlapCircle(transform.position, .21f, whatIsWall);

        if (stuck)
        {
            StartCoroutine(MovePlayerFromWallCo());
        }
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
