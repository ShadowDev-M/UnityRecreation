using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class UIBGM : MonoBehaviour
{
    public static UIBGM instance;
 
    void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
 