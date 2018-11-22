using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

/*A class for spawning troops after a timer*/
public class SpawnAfterTime : MonoBehaviour {

    /*the available spawn states*/
    [System.Serializable]
    public enum SpawnerStates
    {
        waiting,
        ready
    }

    [SerializeField] SpawnerStates spawnerState; /*The state of the Troop when it is spawned*/
    [SerializeField] HangerSpawner[] spawnPoints; /*The available spawn points*/
    [SerializeField] float delayPerSpawnPoint; /*The amount of delay added per each spawn*/

    [SerializeField] float timeBeforeSpawnStart;
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] float timeStackPercent;

    public int unitIndex;

    public float m_timer;

	void Start () {
        spawnerState = SpawnerStates.waiting;
        m_timer = timeBetweenSpawns;
        spawnPoints = GetComponentsInChildren<HangerSpawner>();
	}

    void ResetSpawnTimer()
    {
        timeBetweenSpawns += timeBetweenSpawns * timeStackPercent / 100;
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

        if (spawnerState == SpawnerStates.ready)
        {
            m_timer -= Time.deltaTime;

            if(m_timer <= 0)
            {
                unitIndex++;

                if (unitIndex > spawnPoints.Length - 1)
                    unitIndex = 0;

                SpawnUnits();

                ResetSpawnTimer();
            }
        }
	}

    /*Checks the spawn poinst is valid, before proceeding*/
    void SpawnUnits()
    {
        spawnPoints[unitIndex].CheckSpawn();
    }
}
