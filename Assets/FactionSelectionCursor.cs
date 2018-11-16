using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FactionSelectionCursor : MonoBehaviour {

    PointerEventData pointer;

    public InputDevice m_controller;

    public int playerIndex;

    public Image previewImage;

    public Text previewTextBox;

    public Light previewLight;

    public Text name;

    float markerXPos;
    float markerYPos;

    public GameObject spawnPoint;

    public GameObject[] bases;

    public bool playerChosen;

    public SelectedFactions selected_Factions;

    Image image;

    GameObject currentBase = null;

    GameObject prevBase = null;

    // Use this for initialization
    void Start () {
        m_controller = InputManager.Devices[playerIndex];
	}
	
	// Update is called once per frame
	void Update () {

        if(!playerChosen)
        {
            transform.position += new Vector3(m_controller.LeftStick.X, m_controller.LeftStick.Y, 0) * Time.unscaledDeltaTime * (PlayerPrefs.GetFloat("CursorSpeed") * 1000);

            markerXPos = Mathf.Clamp(transform.position.x, 0, Screen.width);
            markerYPos = Mathf.Clamp(transform.position.y, 0, Screen.height);

            transform.position = new Vector3(markerXPos, markerYPos, 0);

            QuickUpdate();
        }
        else
        {
            gameObject.SetActive(false);
        }
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
            if(cur.gameObject.GetComponent<FactionElements>())
            {
                // its new this frame!
                if (oldRaycasts.Contains(cur.gameObject) == false)
                    ExecuteEvents.Execute(cur.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
                // store the game objects under the cursor this frame
                raycasts.Add(cur.gameObject);

                image = cur.gameObject.GetComponent<Image>();

                OnPointerEnter();

                // button presses
                if (m_controller.Action1.WasPressed)
                {
                    ExecuteEvents.Execute(cur.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                    SelectFaction();
                }
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

                OnPointerExit();

                image = null;
            }
        }

        // store this for comparison next frame
        oldRaycasts = raycasts;
    }

    bool baseSpawned, pointerExit;
    public void OnPointerEnter()
    {
        if (image.color != Color.white)
        {
            baseSpawned = false;
            Destroy(currentBase);
            return;
        }

        if (baseSpawned && pointerExit)
        {
            baseSpawned = false;
            Destroy(currentBase);
        }

        pointerExit = false;

        image.transform.localScale = new Vector3(1.2f, 1.2f, 0);
        image.rectTransform.sizeDelta = new Vector2(95, 150);

        previewImage.sprite = image.GetComponent<FactionElements>().baseFace;
        previewTextBox.text = image.GetComponent<FactionElements>().description;
        previewLight.color = image.GetComponent<FactionElements>().primaryColour;
        name.text = image.GetComponent<FactionElements>().name;

        foreach (GameObject b in bases)
        {
            if(!baseSpawned)
            {
                if (b.name.ToLower() == image.GetComponent<FactionElements>().name.ToLower())
                {
                    currentBase = Instantiate(b, spawnPoint.transform.position, spawnPoint.transform.rotation);
                    currentBase.SetActive(true);
                    baseSpawned = true;
                }
            }
        }
    }

    public void OnPointerExit()
    {
        if (image.color != Color.white)
            return;

        pointerExit = true;

        image.transform.localScale = new Vector3(1, 1, 0);
        image.rectTransform.sizeDelta = new Vector2(50, 95);
    }

    public void SelectFaction()
    {
        if (image.color != Color.white)
            return;

        image.transform.localScale = new Vector3(1, 1, 0);

        image.rectTransform.sizeDelta = new Vector2(95, 120);

        currentBase.SetActive(true);
        image.color = Color.green;
        image.GetComponent<FactionElements>().crossText.enabled = true;
        selected_Factions.SetFactionElement(playerIndex, image.GetComponent<FactionElements>());
        DontDestroyOnLoad(image.GetComponent<FactionElements>().commentator);
        DontDestroyOnLoad(image.GetComponent<FactionElements>().bigBase);

        playerChosen = true;
    }
}
