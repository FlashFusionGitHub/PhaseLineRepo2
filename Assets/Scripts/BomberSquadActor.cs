using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InControl;

public class BomberSquadActor : MonoBehaviour {

    public List<GameObject> m_planes;

    public GameObject m_bombs;

    public GameObject m_marker;

    public bool m_beginStrike;

    public float timer = 5;

    public Team team;

	// Use this for initialization
	void Start () {
	}

    // Update is called once per frame
    void Update() {

        //Start Planes Fight
        foreach (GameObject plane in m_planes)
            plane.transform.Translate(Vector3.forward * 50 * Time.deltaTime);
        
        Destroy(gameObject, 15);
        
        timer -= Time.deltaTime;
        
        //Drop bombs
        DropBombs();
	}

    void DropBombs()
    {
        if(timer <= 0.0f)
        {
            m_bombs.SetActive(true);

            Destroy(gameObject, 2);
        }
    }
}
