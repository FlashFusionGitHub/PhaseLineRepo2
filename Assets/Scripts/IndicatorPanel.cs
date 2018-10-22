using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPanel : MonoBehaviour {

    public NavigationArrowActor m_navMarker;

    public GameObject m_arrow;

    public Camera m_camera;

    public GameObject panel;

    public bool player1;
    public bool player2;

    // TODO read these off parent panel
    public float width = 1920;
    public float startheight = 540;
    public float endHeight = 1080;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 screenPos = m_camera.WorldToScreenPoint(m_navMarker.m_currentMarker.transform.position);

        screenPos.z = 0;

        clampX(screenPos);
        clampY(screenPos);
        Rotate(screenPos);

        m_arrow.gameObject.SetActive(true);
        //m_arrow.transform.position = screenPos;
    }


    void clampX(Vector3 screenPos)
    {
        if (screenPos.x < 0)
        {
            screenPos.y = (0 - width / 2) / (screenPos.x - width / 2) * screenPos.y;
            screenPos.x = 0;
        }

        if (screenPos.x > width)
        {
            screenPos.y = (width - width / 2) / (screenPos.x - width / 2) * screenPos.y;
            screenPos.x = width;
        }

        m_arrow.transform.position = screenPos;
    }

    void clampY(Vector3 screenPos)
    {
        if (screenPos.y < startheight)
        {
            screenPos.x = (startheight - endHeight) / (screenPos.y - endHeight) * screenPos.x;
            screenPos.y = startheight;
        }

        if (screenPos.y > endHeight)
        {
            screenPos.x = (startheight * 2 - endHeight) / (screenPos.y - endHeight) * screenPos.x;
            screenPos.y = startheight * 2;
        }

        m_arrow.transform.position = screenPos;
    }

    void Rotate(Vector3 screenPos)
    {
        float angle = Mathf.Atan2(screenPos.y, screenPos.x);

        angle -= 90 * Mathf.Deg2Rad;

        m_arrow.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
    }

    void TopScreen(Vector3 screenPos)
    {
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < panel.transform.GetComponent<RectTransform>().rect.width && screenPos.y > 540 && screenPos.y < Screen.height)
        {
            m_arrow.gameObject.SetActive(false);
        }
        else
        {
            MoveIndicator(screenPos);
        }
    }

    void BottomScreen(Vector3 screenPos)
    {
        if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < panel.transform.GetComponent<RectTransform>().rect.width && screenPos.y > 0 && screenPos.y < panel.transform.GetComponent<RectTransform>().rect.height)
        {
            m_arrow.gameObject.SetActive(false);
        }
        else
        {
            MoveIndicator(screenPos);
        }
    }

    void MoveIndicator(Vector3 screenPos)
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
