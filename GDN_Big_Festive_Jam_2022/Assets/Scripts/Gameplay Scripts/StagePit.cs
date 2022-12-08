using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            PlayerController player = other.GetComponent<PlayerController>();
            StartCoroutine(MovePlayerCo(player));
        }
    }

    IEnumerator MovePlayerCo(PlayerController player)
    {
        player.GetComponent<Rigidbody2D>().gravityScale = 0f;
        player._beingMoved = true;
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1f);
        player.transform.position = StagePitPosition.lastCheckPointPos;
        player._beingMoved = false;
        yield return new WaitForSeconds(.1f);
        UIFade.instance.FadeFromBlack();
    }
}
