using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePointsOfInterest : MonoBehaviour {

    public int width, length;
    public PointOfInterest prefab;

    Transform spawner;

    int numToSpawn;

    int numOfRows;
    public int spacing;


    public bool GenerateNow = false;

    public bool KillAll = false;


    void GoThroughCuberty() //this naming convention was rowens idea
    {

        spawner = new GameObject("Spawner").transform;

        spawner.parent = transform;

        spawner.position = transform.position;

        if (length / spacing > 0)
        {
            numOfRows = length / spacing;
        }

        for (int i = 0; i < numOfRows; i++)
        {
            GenerateRow();
            MoveSpawnerUp();
        }
        DestroyImmediate(spawner.gameObject);
    }

    void SpawnPrefab()
    {
        GameObject obj = Instantiate(prefab.gameObject, spawner)as GameObject;
        obj.transform.parent = transform;
        PointOfInterest p = obj.GetComponent<PointOfInterest>();
    }

    void MoveSpawnerRight()
    {
        spawner.position += new Vector3(spacing, 0, 0);
    }

    void MoveSpawnerUp()
    {
        spawner.localPosition = new Vector3(0, 0, spawner.localPosition.z);
        spawner.position += new Vector3 (0, 0, spacing);
    }

    void GenerateRow()
    {
        if (width / spacing > 0)
        {
            numToSpawn = width / spacing;
        }

        for (int i = 0; i < numToSpawn; i++)
        {
            SpawnPrefab();
            MoveSpawnerRight();
        }
    }
	
	// Update is called once per frame
	void OnDrawGizmosSelected () {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3 (transform.position.x + (width/2), transform.position.y, transform.position.z + (length/2)), new Vector3(width, 0, length));

        if (GenerateNow)
        {
            GenerateNow = false;
            GoThroughCuberty();
        }

        if (KillAll)
        {
            KillAll = false;
            foreach (PointOfInterest p in FindObjectsOfType<PointOfInterest>())
            {
                if (p)
                   DestroyImmediate(p.gameObject);
            }
        }
	}
}
