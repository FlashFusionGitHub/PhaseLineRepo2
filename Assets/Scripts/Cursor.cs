using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    public InputDevice m_controller;

    public GameObject cursor;

    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_controller = InputManager.Devices[0];

        transform.position += new Vector3(m_controller.LeftStickX, m_controller.LeftStickY, 0) * 5;

        float markerXPos = Mathf.Clamp(transform.position.x, 0, Screen.width);
        float markerYPos = Mathf.Clamp(transform.position.y, 0, Screen.height);

        transform.position = new Vector3(markerXPos, markerYPos, 0);

        QuickUpdate();
    }


    void QuickUpdate()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);

        pointer.position = cursor.transform.position;

        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointer, raycastResults);

        foreach (RaycastResult cur in raycastResults)
        {
            ExecuteEvents.Execute(cur.gameObject, pointer, ExecuteEvents.pointerExitHandler);

            if(m_controller.Action1.WasPressed)
            {
                ExecuteEvents.Execute<IPointerClickHandler>(cur.gameObject, pointer, ExecuteEvents.pointerClickHandler);
            }
        }
    }
}
