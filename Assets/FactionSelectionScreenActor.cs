using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FactionSelectionScreenActor : MonoBehaviour
{
    public Image[] images;

    public Cursor cursor;

    // Use this for initialization
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerEnter(int num)
    {
        images[num].transform.localScale = new Vector3(1.4f, 1.4f, 0);
    }

    public void OnPointerExit(int num)
    {
        images[num].transform.localScale = new Vector3(1, 1, 0);
    }

    public void SelectFaction(int num)
    {
        Debug.Log("Faction " + num + " Selected");

        images[num].transform.localScale = new Vector3(1, 1, 0);
        images[num].color = Color.black;
    }
}