using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FactionSelectionScreenActor : MonoBehaviour
{
    public Image[] images;
    public Image[] masks;


    public Cursor cursor;
    public Text cursorText;

    enum Player { player1, player2 };

    Player player;

    bool playerChosen;

    public float pos = -112.7f;

    public SelectedFactions selected_Factions;

    public SceneLoader sceneLoader;

	public Image previewImageTeam1;
	public Image previewImageTeam2;

	public Text previewTextBoxTeam1;
	public Text previewTextBoxTeam2;

    public Text nameTeam1;
    public Text nameTeam2;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!playerChosen)
        {
            player = Player.player1;
        }
        else
        {
            player = Player.player2;
        }
    }

    public void OnPointerEnter(int num)
    {
        if (masks[num].color != Color.white)
            return;

        images[num].transform.localScale = new Vector3(1.4f, 1.4f, 0);
        masks[num].rectTransform.sizeDelta = new Vector2(95, 150);

		if (player == Player.player1) {
			previewImageTeam1.sprite = masks [num].GetComponent<FactionElements>().baseFace;
			previewTextBoxTeam1.text = masks [num].GetComponent<FactionElements>().description;
            nameTeam1.text = masks[num].GetComponent<FactionElements>().name;
		}
		if (player == Player.player2) {
			previewImageTeam2.sprite = masks [num].GetComponent<FactionElements>().baseFace;
			previewTextBoxTeam2.text = masks [num].GetComponent<FactionElements>().description;
            nameTeam2.text = masks[num].GetComponent<FactionElements>().name;
		}
    }

    public void OnPointerExit(int num)
    {
        if (masks[num].color != Color.white)
            return;

        images[num].transform.localScale = new Vector3(1, 1, 0);
        masks[num].rectTransform.sizeDelta = new Vector2(50, 95);
    }

    public void SelectFaction(int num)
    {
        if (masks[num].color != Color.white)
            return;

        images[num].transform.localScale = new Vector3(1, 1, 0);

        masks[num].rectTransform.sizeDelta = new Vector2(95, 120);

        if (player == Player.player1)
        {
            masks[num].color = Color.green;
            selected_Factions.SetFactionElement(0, masks[num].GetComponent<FactionElements>());
            DontDestroyOnLoad(masks[num].GetComponent<FactionElements>().commentator);
            DontDestroyOnLoad(masks[num].GetComponent<FactionElements>().bigBase);
            cursor.SwapController();
            cursorText.text = "P2";
        }

        if (player == Player.player2)
        {
            masks[num].color = Color.red;
            selected_Factions.SetFactionElement(1, masks[num].GetComponent<FactionElements>());
            DontDestroyOnLoad(masks[num].GetComponent<FactionElements>().commentator);
            DontDestroyOnLoad(masks[num].GetComponent<FactionElements>().bigBase);
            sceneLoader.LoadScene(2);
        }

        playerChosen = true;
    }
}