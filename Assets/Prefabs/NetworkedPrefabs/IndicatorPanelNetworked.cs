using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPanelNetworked : MonoBehaviour {

    public GameObject arrowImage;

    public NavigationMarkerNetworked m_nmn;

    public Camera m_camera;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 screenpos = m_camera.WorldToScreenPoint(m_nmn.m_currentMarker.transform.position);

        if(screenpos.z > 0 && screenpos.x > 0 && screenpos.x < Screen.width && screenpos.y > 0 && screenpos.y < Screen.height)
        {
            arrowImage.SetActive(false);
        }
        else
        {
            arrowImage.SetActive(true);

            if (screenpos.z < 0)
            {
                screenpos *= -1;
            }

            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

            screenpos -= screenCenter;

            float angle = Mathf.Atan2(screenpos.y, screenpos.x);
            angle -= 90.0f * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            screenpos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

            float m = cos / sin;

            Vector3 screenBounds = screenCenter * 0.9f;

            if (cos > 0)
                screenpos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            else
                screenpos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);

            if (screenpos.x > screenBounds.x)
                screenpos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
            else if (screenpos.x < -screenBounds.x)
                screenpos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);

            screenpos += screenCenter;

            arrowImage.transform.localPosition = screenpos;
            arrowImage.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
	}
}
