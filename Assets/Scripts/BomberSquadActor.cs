using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InControl;

public class BomberSquadActor : MonoBehaviour {
	
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
