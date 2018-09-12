using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangerSpawner : MonoBehaviour {

    [System.Serializable]
    public enum DoorState
    {
        closed,
        closing,
        open,
        opening
    }

    [System.Serializable]
    public class HangerDoorData
    {
        public Transform m_hangerDoor;
		public Transform m_openPos;
		public Transform m_closedPos;

    }

    [Header("Spawn Stuff")]
    public TroopActor.UnitClasses spawnClass;
    public Team team;
    public Transform spawnPoint;
    public GameObject lastSpawnedObject;
    public float disBeforeNextSpawn;
    [SerializeField] bool spawnWaiting;

    [Header("Door Stuff")]
    [SerializeField] private HangerDoorData[] m_hangerDoors;
    [SerializeField] private DoorState doorState;
    float progress;
    [SerializeField] private float m_doorOpenTime;
    [SerializeField] private float m_doorCloseTime;

    [Header("Object Pool")]
    [SerializeField] private ObjectPool objectPool;

    // Use this for initialization
    void Start () {
        objectPool = FindObjectOfType<ObjectPool>();
	}
	
	// Update is called once per frame
	void Update () {
        if (spawnWaiting)
        {
            CheckSpawn();
        }
        if (safeToClose() && (doorState == DoorState.open))
        {
            StartClosingDoors();
        }

        if (doorState == DoorState.opening)
        {
            OpenDoors();
        }
        else if (doorState == DoorState.closing)
        {
            CloseDoors();
        }
        UpdateDoors();
    }

    //---------------------------------------------------------------------Door Voids-------------------------------------------------------

    void StartOpeningDoors()
    {
        doorState = DoorState.opening;
    }
    void StartClosingDoors()
    {
        doorState = DoorState.closing;
    }


    void OpenDoors()
    {
        if (progress < 1)
        {
            progress += Time.deltaTime / m_doorOpenTime;
        }
        else
        {
            progress = 1;
            doorState = DoorState.open;
        }
    }

    void CloseDoors()
    {
        if (progress > 0)
        {
            progress -= Time.deltaTime / m_doorCloseTime;
        }
        else
        {
            progress = 0;
            doorState = DoorState.closed;
        }
    }
    void UpdateDoors()
    {
        foreach (HangerDoorData hangerDoor in m_hangerDoors)
        { 
			hangerDoor.m_hangerDoor.localPosition = Vector3.Lerp(hangerDoor.m_closedPos.localPosition, hangerDoor.m_openPos.localPosition, progress);
			hangerDoor.m_hangerDoor.localScale = Vector3.Lerp(hangerDoor.m_closedPos.localScale, hangerDoor.m_openPos.localScale, progress);
			hangerDoor.m_hangerDoor.localEulerAngles = Vector3.Lerp(hangerDoor.m_closedPos.localEulerAngles, hangerDoor.m_openPos.localEulerAngles, progress);
        }
    }

    //------------------------------------------------------------------Spawn Voids-------------------------------------------------------------

    public void CheckSpawn()
    {
        if (doorState == DoorState.closed && ObjectToSpawnAvaliable())
        {
            SpawnObject();
            spawnWaiting = false;
            StartOpeningDoors();
        }
        else
        {
            spawnWaiting = true;
        }
    }
    TroopActor ObjectToSpawnAvaliable()
    {
        foreach (TroopActor ta in objectPool.allTroopActors)
        {
            if (ta.rankState == TroopActor.RankState.dead && ta.team == team && ta.unitClass == spawnClass)
            {
                return ta;
            }
        }
        return null;
    }
    void SpawnObject()
    {
        TroopActor ta = ObjectToSpawnAvaliable();
        lastSpawnedObject = ta.gameObject;
        if (ta.gameObject.activeInHierarchy == false)
        {
            ta.gameObject.SetActive(true);
        }
        ta.SetHealth(ta.maxHealth);
        ta.rankState = TroopActor.RankState.LookingForGeneral;
        ta.transform.position = spawnPoint.position;

    }
    bool safeToClose()
    {
        if (lastSpawnedObject)
        {
            if (Vector3.Distance(spawnPoint.position, lastSpawnedObject.transform.position) > disBeforeNextSpawn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint.position, disBeforeNextSpawn);
    }
}
