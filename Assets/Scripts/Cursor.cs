using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    PointerEventData pointer; /*reference to the point object*/

    public Controller[] m_controllers; /*reference to the available controllers*/

    Controller m_controller; /*referecne to the the current controller*/

	public bool Player1, Player2; /*The current player using the cursor*/

    public List<GameObject> buttons; /*reference to the avail button to toggle to*/

    /*private variable to store the cursors position*/
    float markerXPos;
    float markerYPos;

    // Use this for initialization
    void Start()
    {
        /*set the cotroller*/
		m_controllers = FindObjectsOfType<Controller> ();

		if (Player1)
			m_controller = m_controllers [1];
		if (Player2)
			m_controller = m_controllers [0];
    }
		
    void Update()
    {
        /*if we are not currently toogling buttons, the nallopw the mouse to move freely*/
        if(!ToggleButtons())
            transform.position += new Vector3(m_controller.LeftAnalogStick().X, m_controller.LeftAnalogStick().Y, 0) * Time.unscaledDeltaTime * (PlayerPrefs.GetFloat("CursorSpeed") * 1000);
        else
            ToggleButtons();

        markerXPos = Mathf.Clamp(transform.position.x, 0, Screen.width);
        markerYPos = Mathf.Clamp(transform.position.y, 0, Screen.height);

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

    public int index;/*index of the button we have toggle through*/
    bool ToggleButtons()
    {
        if(m_controller.DpadLeftWasPress())
        {
            index++;

            if (index >= buttons.Count)
                index = 0;

            transform.position = buttons[index].transform.position;

            return true;
        }

        if (m_controller.DpadRightWasPress())
        {
            if (index <= 0)
                index = buttons.Count;

            index--;

            transform.position = buttons[index].transform.position;

            return true;
        }

        return false;
    }

    /*allow the cursor to swap its current controller*/
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
