using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentObjectPool : MonoBehaviour
{
    Queue<GameObject> _availableObjects = new Queue<GameObject>();

    public static PresentObjectPool instance;

    private void Start()
    {
        instance = this;
        
    }

    public void AddToPool(GameObject instanceToAdd)
    {
        instanceToAdd.SetActive(false);
        _availableObjects.Enqueue(instanceToAdd);
    }

    public GameObject GetFromPool()
    {
        var poolInstance = _availableObjects.Dequeue();
        poolInstance.SetActive(true);
        return poolInstance;
    }
}
