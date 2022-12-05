using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentSprite : MonoBehaviour
{
    public Sprite[] presentSprites;
    SpriteRenderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        RandomiseSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RandomiseSprite()
    {
        int rndIndex = Random.Range(0, presentSprites.Length);
        _renderer.sprite = presentSprites[rndIndex];
    }
}
