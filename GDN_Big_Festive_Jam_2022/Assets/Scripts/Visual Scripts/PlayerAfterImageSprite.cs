using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    float _activeTime = .1f;
    float _timeActivated;
    float _alpha;
    [SerializeField]
    float _alphaSet;
    float _alphaMultiplier = .85f;

    Transform player;

    SpriteRenderer _renderer;

    SpriteRenderer _playerRenderer;

    Color _color;

    private void OnEnable()
    {
        _renderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerRenderer = player.GetComponent<SpriteRenderer>();

        _alpha = _alphaSet;
        _renderer.sprite = _playerRenderer.sprite;

        transform.position = player.position;
        transform.rotation = player.rotation;

        _timeActivated = Time.time;
    }

    private void Update()
    {
        _alpha *= _alphaMultiplier;
        _color = new Color(1f, 1f, 1f, _alpha);
        _renderer.color = _color;

        if(Time.time >= (_timeActivated + _activeTime))
        {
            //Return to object pool
            AfterImageObjectPool.instance.AddToPool(gameObject);
        }
    }
}
