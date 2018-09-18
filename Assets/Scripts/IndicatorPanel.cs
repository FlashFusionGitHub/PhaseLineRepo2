using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPanel : MonoBehaviour {

    public NavigationArrowActor m_navMarker;

    public MarkerIndicatorArrow m_arrow;

    public Camera m_camera;

    public GameObject panel;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 screenPos = m_camera.WorldToScreenPoint(m_navMarker.m_currentMarker.transform.position);

        //float relation = ??;

        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width
            && screenPos.y > 0 && screenPos.y < Screen.height)
        {
            m_arrow.enabled = false;
        }
        else
        {
            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }

            m_arrow.gameObject.SetActive(true);

            Vector3 screenCenter = new Vector3(panel.transform.GetComponent<RectTransform>().rect.width, panel.transform.GetComponent<RectTransform>().rect.height, 0) / 2;

            screenPos -= screenCenter;

            float angle = Mathf.Atan2(screenPos.y, screenPos.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = -Mathf.Sin(angle);

            screenPos = screenCenter + new Vector3(sin * 100, cos * 100, 0);

            float m = cos / sin;

            Vector3 screenBounds = screenCenter * 0.9f;

            if (cos > 0)
            {
                screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
            }
            else
            {
                screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
            }

            if (screenPos.x > screenBounds.x)
            {
                screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
            }
            else if (screenPos.x < -screenBounds.x)
            {
                screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
            }

            screenPos += screenCenter;

            m_arrow.transform.localPosition = screenPos;
            m_arrow.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
	}
}
