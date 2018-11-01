﻿using System.Collections;
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

	void Start () {
        spawnerState = SpawnerStates.waiting;
        m_timer = timeBetweenSpawns;
        spawnPoints = GetComponentsInChildren<HangerSpawner>();
	}

    void ResetSpawnTimer()
    {
        timeBetweenSpawns += timeBetweenSpawns * timeStackPercent/100;
        m_timer = timeBetweenSpawns;
    }
	// Update is called once per frame
	void Update () {
		
        if (spawnerState == SpawnerStates.waiting)
        {
            timeBeforeSpawnStart -= Time.deltaTime;
            if (timeBeforeSpawnStart <= 0)
            {
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
                ResetSpawnTimer();
            }
        }
	}
}
