using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjects : MonoBehaviour, IDamageInterface
{
    public int objectDurability;

    public void DamageObject(int damageTaken)
    {
        objectDurability -= damageTaken;
        if(objectDurability <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
