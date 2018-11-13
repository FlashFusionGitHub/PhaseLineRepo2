using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionIndicator : MonoBehaviour {

    [System.Serializable]
    public class SelectionImage
    {
        public UnitClasses unitClass;
        public Image img;
        public bool selected;
    }

    [System.Serializable]
    public struct UIStuff
    {
        public Image portrait;
        public Image[] imagesToColor;
        public Slider sldr;
    }

    public Team team;
    public SelectionImage[] selectionImages;
    public UIStuff uiStuff;
    public TroopController tc;
    public TroopActor BaseTa;
    PortraitData pd; 
	// Use this for initialization
	void Start () {
        foreach (TroopController tci in FindObjectsOfType<TroopController>())
        {
            if (tci.team == team)
            {
                tc = tci;
            }
        }

        foreach (PortraitData pdi in FindObjectsOfType<PortraitData>())
        {
            if (pdi.team == team)
            {
                pd = pdi;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (pd)
        {
            if (uiStuff.portrait.sprite != pd.baseFace)
            {
                uiStuff.portrait.sprite = pd.baseFace;
            }
            if (uiStuff.imagesToColor.Length > 0)
            {
                foreach (Image panel in uiStuff.imagesToColor)
                {
                    if (panel.color != pd.TeamColor)
                        panel.color = pd.TeamColor;
                }
            }

            if (!BaseTa)
            {
                BaseTa = pd.gameObject.GetComponentInChildren<TroopActor>();
            }
        }
        else
        {
            foreach (PortraitData pdi in FindObjectsOfType<PortraitData>())
            {
                if (pdi.team == team)
                {
                    pd = pdi;
                }
            }
        }
        
        if (uiStuff.sldr && BaseTa)
        {
            if (uiStuff.sldr.maxValue != BaseTa.maxHealth)
            {
                uiStuff.sldr.maxValue = BaseTa.maxHealth;
            }
            uiStuff.sldr.value = BaseTa.currentHealth;
           
        }
        foreach (SelectionImage si in selectionImages)
        {
            if (tc.currentSelectedUnit.GetComponent<TroopActor>().unitClass == si.unitClass)
            {
                if (!si.selected)
                {
                    si.selected = true;
                    si.img.color = Color.white;
                }
            }
            else
            {
                if (si.selected)
                {
                    si.selected = false;
                    si.img.color = Color.black;
                }
            }
        }
	}
}
