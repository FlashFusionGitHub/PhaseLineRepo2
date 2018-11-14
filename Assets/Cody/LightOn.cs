using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOn : MonoBehaviour {


    public GameObject light1;


    void Start()
    {
        light1.SetActive(false);
    }

    void OnTriggerStay(Collider col)

    {
        Debug.Log("i hit this bitch");
        if (col.gameObject.tag == "light")
        {
            light1.SetActive(true);

        }
    }
}
