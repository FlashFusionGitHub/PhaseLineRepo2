using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMEPLEASE : MonoBehaviour {

    public Team t;
    PortraitData pd;
    Renderer r;
	// Use this for initialization
	void Start () {
        r = GetComponent<Renderer>();
        foreach (PortraitData p in FindObjectsOfType<PortraitData>())
        {
            if (p.team == t)
            {
                pd = p;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (pd)
        {
            if (r)
            {
                Material mat = r.material;
                if (mat && mat.color != pd.TeamColor)
                {
                    mat.color = pd.TeamColor;
                    mat.SetColor("_EmissionColor", pd.TeamColor);
                }
            }
        }
        else
        {
            foreach (PortraitData p in FindObjectsOfType<PortraitData>())
            {
                if (p.team == t)
                {
                    pd = p;
                }
            }
      
        }
	}
}
