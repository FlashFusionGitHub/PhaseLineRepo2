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
        public Image panel;
        public Slider sldr;
    }

    public Team team;
    public SelectionImage[] selectionImages;
    public UIStuff uiStuff;
    public TroopController tc;
    public TroopActor BaseTa;
	// Use this for initialization
	void Start () {
        foreach (TroopController tci in FindObjectsOfType<TroopController>())
        {
            if (tci.team == team)
            {
                tc = tci;
            }
        }
        foreach (PortraitData pd in FindObjectsOfType<PortraitData>())
        {
            if (pd.team == team)
            {
                uiStuff.portrait.sprite = pd.baseFace;
                uiStuff.panel.color = pd.TeamColor;
                BaseTa = pd.gameObject.GetComponentInChildren<TroopActor>();
                uiStuff.sldr.maxValue = BaseTa.maxHealth;
                uiStuff.sldr.value = BaseTa.currentHealth;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        uiStuff.sldr.value = BaseTa.currentHealth;
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
