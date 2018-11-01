using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour {

    public Vector3 spawnerScale;

    public int maxCarePackages;
    int carePackageCount;

    public float timeBetweenSpawns;
    public float randomiseMultiplier;
    float spawnTimer;

    public GameObject CarePackagePrefab;

	// Use this for initialization
	void Start () {
        ResetTimer();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (spawnTimer < 0)
        {
            if (carePackageCount < maxCarePackages)
            {
                SpawnCarePackage();
                carePackageCount++;
            }
            ResetTimer();
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
	}

    public void adjustSpawnCount(int changeBy)
    {
        carePackageCount += changeBy;
    }
    void ResetTimer()
    {
        spawnTimer = Random.Range (timeBetweenSpawns - (timeBetweenSpawns * randomiseMultiplier), timeBetweenSpawns + (timeBetweenSpawns * randomiseMultiplier));
    }

    void SpawnCarePackage()
    {
        GameObject lastCarePackage = Instantiate(CarePackagePrefab, transform);
        lastCarePackage.transform.position = new Vector3(transform.position.x + (Random.Range(-spawnerScale.x, spawnerScale.x)), transform.position.y + (Random.Range(-spawnerScale.y, spawnerScale.y)), transform.position.z + (Random.Range(-spawnerScale.z, spawnerScale.z)));
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnTimer < 0)
            Gizmos.color = Random.ColorHSV();
        Gizmos.DrawWireCube(transform.position, spawnerScale * 2);
    }
}
