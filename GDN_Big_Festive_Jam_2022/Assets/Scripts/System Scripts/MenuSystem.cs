using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    public static MenuSystem instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    
}
