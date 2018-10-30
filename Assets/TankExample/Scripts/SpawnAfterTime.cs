using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

public class SpawnAfterTime : MonoBehaviour {

    [System.Serializable]
    public enum SpawnerStates
    {
        waiting,
        ready
    }

    [SerializeField] SpawnerStates spawnerState;
    [SerializeField] HangerSpawner[] spawnPoints;
    [SerializeField] float delayPerSpawnPoint;

    [SerializeField] float timeBeforeSpawnStart;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] float timeStackPercent;
    float m_timer;

    protected InputDevice m_controller;
    public int playerIndex;

    int unitIndex;
    GameObject unitToSpawn;

    public Image unitTypeImage;

    UnitSpawnTypePanel ustp;

	void Start () {
        spawnerState = SpawnerStates.waiting;
        m_timer = timeBetweenSpawns;
        spawnPoints = GetComponentsInChildren<HangerSpawner>();

        FindObjectOfType<UnitSpawnTypePanel>();

        foreach(UnitSpawnTypePanel u in FindObjectsOfType<UnitSpawnTypePanel>())
        {
            if(u.playerIndex == playerIndex)
            {
                ustp = u;
            }
        }
	}
    void ResetSpawnTimer()
    {
        timeBetweenSpawns += timeBetweenSpawns * timeStackPercent/100;
        m_timer = timeBetweenSpawns;
    }
	// Update is called once per frame
	void Update () {

        try
        {
            m_controller = InputManager.Devices[playerIndex];
        }
        catch (System.Exception)
        {
            return;
        }

        if(m_controller.DPadUp.WasPressed)
        {
            if (unitIndex < 3)
                unitIndex += 1;
            else
                unitIndex = 0;
        }

        if(m_controller.DPadDown.WasPressed)
        {
            if (unitIndex > -1)
                unitIndex -= 1;
            else
                unitIndex = 2;
        }

        if (unitIndex == 0)
            ustp.GetComponent<Text>().text = "T";
        if (unitIndex == 1)
            ustp.GetComponent<Text>().text = "AA";
        if (unitIndex == 2)
            ustp.GetComponent<Text>().text = "H";

        if (spawnerState == SpawnerStates.waiting)
        {
            timeBeforeSpawnStart -= Time.deltaTime;
            if (timeBeforeSpawnStart <= 0)
            {
                SpawnUnits();
                spawnerState = SpawnerStates.ready;
            }
        }
        else if (spawnerState == SpawnerStates.ready)
        {
            if (m_timer > 0)
            {
                m_timer -= Time.deltaTime;
            }
            else
            {
                SpawnUnits();
                ResetSpawnTimer();
            }
        }
	}

    void SpawnUnits()
    {
        spawnPoints[unitIndex].CheckSpawn();
    }
}
