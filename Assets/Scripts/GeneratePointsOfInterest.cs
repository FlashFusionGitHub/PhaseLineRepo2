using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePointsOfInterest : MonoBehaviour {

    public int width, length;
    public GameObject prefab;

    Transform spawner;

    int numToSpawn;

    int numOfRows;
    public int spacing;

    List<Transform> objs;

    ObjectPool op;
	// Use this for initialization
	void Start () {
        op = GetComponent<ObjectPool>();

        spawner = new GameObject("Spawner").transform;

        spawner.parent = transform;

        spawner.position = transform.position;

        if(length / spacing > 0)
        {
            numOfRows = length / spacing;
        }

        for(int i = 0; i < numOfRows; i++)
        {
            GenerateRow();
            MoveSpawnerUp();
        }

        op.pointsOfInterest = objs;
    }

    void SpawnPrefab()
    {
        GameObject obj = Instantiate(prefab, spawner);
        obj.transform.parent = transform;
        objs.Add(obj.transform);
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
	}
}
