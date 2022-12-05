using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageObjectPool : MonoBehaviour
{
    public int afterImageNum;

    [SerializeField]
    GameObject _afterImagePrefab;

    Queue<GameObject> _availableObjects = new Queue<GameObject>();

    public static AfterImageObjectPool instance { get; private set; }

    private void Awake()
    {
        instance = this;
        GrowPool();
    }

    void GrowPool()
    {
        for(int i = 0; i < afterImageNum; i++)
        {
            var instanceToAdd = Instantiate(_afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instanceToAdd)
    {
        instanceToAdd.SetActive(false);
        _availableObjects.Enqueue(instanceToAdd);
    }

    public GameObject GetFromPool()
    {
        if(_availableObjects.Count == 0)
        {
            GrowPool();
        }

        var poolInstance = _availableObjects.Dequeue();
        poolInstance.SetActive(true);

        return poolInstance;
    }
}
