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

    public GameObject DepositPresent()
    {
        return _availableObjects.Dequeue();
    }

    public int GetPoolCount()
    {
        return _availableObjects.Count;
    }

    public void RetrievePresent(GameObject instanceToAdd)
    {
        _availableObjects.Enqueue(instanceToAdd);
        
    }

    public void SetPoolQueue(Queue<GameObject> _queueToAdd)
    {
        Debug.Log(_queueToAdd.Count);
        int queueLength = _queueToAdd.Count;
        for(int i = 0; i <= queueLength; i++)
        {
            GameObject presToAdd = _queueToAdd.Dequeue();
            _availableObjects.Enqueue(presToAdd);
        }


        //Debug.Log(_availableObjects.Count);
    }

    public GameObject GetFromPool()
    {
        var poolInstance = _availableObjects.Dequeue();
        poolInstance.SetActive(true);
        return poolInstance;
    }
}
