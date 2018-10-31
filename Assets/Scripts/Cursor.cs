﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    PointerEventData pointer;

    Controller[] m_controllers;

    Controller m_controller;

    // Use this for initialization
    void Start()
    {
        m_controllers = FindObjectsOfType<Controller>();
        m_controller = m_controllers[0];
    }

    void Update()
    {

       transform.position += new Vector3(m_controller.RightAnalogStick().X, m_controller.RightAnalogStick().Y, 0) * Time.unscaledDeltaTime * (PlayerPrefs.GetFloat("CursorSpeed") * 1000);

        float markerXPos = Mathf.Clamp(transform.position.x, 0, Screen.width);
        float markerYPos = Mathf.Clamp(transform.position.y, 0, Screen.height);

        transform.position = new Vector3(markerXPos, markerYPos, 0);

        QuickUpdate();
    }

    // objects that were under the cursor last frame
    List<GameObject> oldRaycasts = new List<GameObject>();

    void QuickUpdate()
    {
        pointer = new PointerEventData(EventSystem.current);

        pointer.position = transform.position;

        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointer, raycastResults);

        List<GameObject> raycasts = new List<GameObject>();
        foreach (RaycastResult cur in raycastResults)
        {
            // its new this frame!
            if (oldRaycasts.Contains(cur.gameObject) == false)
                ExecuteEvents.Execute(cur.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
            // store the game objects under the cursor this frame
            raycasts.Add(cur.gameObject);

            // button presses
            if (m_controller.Action1WasPress())
            {
                ExecuteEvents.Execute(cur.gameObject, pointer, ExecuteEvents.pointerClickHandler);
            }
        }

        // iterate over everythign that was under the cursor last frame...
        foreach (GameObject obj in oldRaycasts)
        {
            // is it no longer under the cursor?
            if (raycasts.Contains(obj) == false)
            {
                // the cursor has left us this frame - fire off event
                ExecuteEvents.Execute(obj, pointer, ExecuteEvents.pointerExitHandler);
            }
        }

        // store this for comparison next frame
        oldRaycasts = raycasts;
    }

    public void SwapController()
    {
        if(m_controller == m_controllers[0])
        {
            m_controller = m_controllers[1];
        }
        else
        {
            m_controller = m_controllers[0];
        }
    }
}
