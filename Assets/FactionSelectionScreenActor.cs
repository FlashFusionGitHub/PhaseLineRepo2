﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FactionSelectionScreenActor : MonoBehaviour
{
    public Image[] images;
    public Image[] masks;

    public Cursor cursor;

    enum Player { player1, player2 };

    Player player;

    bool playerChosen;

    public float pos = -112.7f;

    public SelectedFactions selected_Factions;

    public SceneLoader sceneLoader;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerChosen)
        {
            player = Player.player1;
        }
        else
        {
            player = Player.player2;
        }
    }

    public void OnPointerEnter(int num)
    {
        if (masks[num].color != Color.white)
            return;

        images[num].transform.localScale = new Vector3(1.4f, 1.4f, 0);
        masks[num].rectTransform.sizeDelta = new Vector2(95, 300);
    }

    public void OnPointerExit(int num)
    {
        if (masks[num].color != Color.white)
            return;

        images[num].transform.localScale = new Vector3(1, 1, 0);
        masks[num].rectTransform.sizeDelta = new Vector2(95, 95);
    }

    public void SelectFaction(int num)
    {
        if (masks[num].color != Color.white)
            return;

        Debug.Log("Faction " + num + " Selected");

        images[num].transform.localScale = new Vector3(1, 1, 0);

        masks[num].rectTransform.sizeDelta = new Vector2(110, 300);

        if (player == Player.player1)
        {
            masks[num].color = Color.green;
            selected_Factions.SetFactionElement(0, masks[num].GetComponent<FactionElements>());
        }
        if (player == Player.player2)
        {
            masks[num].color = Color.red;
            selected_Factions.SetFactionElement(1, masks[num].GetComponent<FactionElements>());
            sceneLoader.LoadScene(2);
        }

        playerChosen = true;
    }
}