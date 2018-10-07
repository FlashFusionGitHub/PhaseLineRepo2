using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.AI;

[System.Serializable]
public class FormationPositionNetworked
{
    public Transform fromPos;
    public TroopActorNetworked assignedUnit;
    public bool taken;
}

[System.Serializable]
public class GunSettingsNetworked
{
    [Header("Gun Stuff")]
    public int damage;
    public float TimeBetweenShots;
    public float m_gunTimerRandomiser;
    public float m_gunTimer;
    public float attackRangeMin;
    public float attackRangeMax;
    public TroopActorNetworked attackTarget;

    [Header("Turret Settings")]
    public Transform turret;
    public float turretAimSpeed;

    [Header("Barrel Settings")]
    public Transform barrel;
    public UnityEvent onShoot;
    public UnityEvent onKillShot;
    public float barrelAimSpeed;
}

public class TroopActorNetworked : NetworkBehaviour
{

    [Header("Health Settings")]
    public const int maxHealth = 100;
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;
    public RectTransform healthBar;
    [SerializeField] UnityEvent onTakeDamage;
    [SerializeField] UnityEvent onDie;

    [Header("Name Settings")]
    public string unitName;
    [SerializeField] string[] possibleNames;

    [Header("Rank Settings")]
    public RankState rankState;
    public TroopActorNetworked myGeneral;
    float killMeAfter = 0.5f;

    [Header("Movement Settings")]
    [SerializeField]
    bool moving;
    [SerializeField] UnityEvent OnMove;
    public GameObject moveTarget;
    [SerializeField] MovementTypes movementType;
    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed;
    public GameObject moveTargetPrefab;
    public GameObject generalMoveTargetPrefab;

    [Header("Formation Settings")]
    [SerializeField] PlacementDirection placementDirection;
    [SerializeField] float numOfRows;
    [SerializeField] float maxNumOfColumns;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] Vector2 distanceBetweenPoints;
    public List<FormationPositionNetworked> formationPositions = new List<FormationPositionNetworked>();
    [SerializeField] Transform oldPos;
    [SerializeField] PlacementDirection pd;
    [SerializeField] int count;

    [Header("Unit Settings")]
    public UnitClasses unitClass;
    public Team team;
    [SerializeField] UnitClasses[] strengths;
    [SerializeField] UnitClasses[] vulnerabilities;

    [Header("Promotion Settings")]
    [SerializeField]
    int rank;
    [SerializeField] UnityEvent OnGainExperience;
    [SerializeField] UnityEvent OnPromotion;
    [SerializeField] UnityEvent OnBecomeGeneral;
    [SerializeField] string[] rankTitles;
    [SerializeField] float experienceRequiredForNextRank;
    [SerializeField] float currentExperience;
    [SerializeField] float percentageIncreasePerRank;
    [SerializeField] float influenceRadius;

    [Header("Air Movement Settings")]
    [SerializeField] float hoverHeight;
    [SerializeField] float verticleSpeed;

    [Header("Ground Movement Settings")]
    [SerializeField] NavMeshAgent m_navAgent;

    [Header("Object Avoidance Settings")]
    [SerializeField] float avoidanceRange;

    [Header("Attack Settings")]
    [SerializeField] GunSettingsNetworked[] guns;

    [Header("Important Optimisation List")]
    [SerializeField] ObjectPoolNetworked op;

    // Use this for initialization
    void Start()
    {
        op = FindObjectOfType<ObjectPoolNetworked>();
        /*Give a Name Troop*/
        NameUnit();
    }

    // Update is called once per frame
    void Update()
    {
        //Keep Troop state updated each frame 
        RankAction();

        if (rankState != RankState.dead)
        {
            Move();
            AttackClosestEnemy();
        }

        CmdUpdateMoveTargetPosition(moveTarget.transform.position, moveTarget.transform.rotation); 
    }

    [Command]
    void CmdUpdateMoveTargetPosition(Vector3 pos, Quaternion rot)
    {
        moveTarget.transform.position = pos;
        moveTarget.transform.rotation = rot;
    }

    public void CreateMoveTarget()
    {
        Destroy(moveTarget);

        if (generalMoveTargetPrefab)
        {
            moveTarget = Instantiate(generalMoveTargetPrefab, transform.position, transform.rotation);
            moveTarget.name = gameObject.name + "'s MoveTarget";
            moveTarget.AddComponent<NetworkIdentity>();
            moveTarget.AddComponent<NetworkTransform>();
            moveTarget.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
            moveTarget.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        }
        else
        {
            moveTarget = Instantiate(moveTargetPrefab, transform.position, transform.rotation);
            moveTarget.name = gameObject.name + "'s MoveTarget";
            moveTarget.AddComponent<NetworkIdentity>();
            moveTarget.AddComponent<NetworkTransform>();
            moveTarget.GetComponent<NetworkIdentity>().localPlayerAuthority = true;
            moveTarget.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        }
    }

    public void TakeDamage(int damageToTake)
    {
        //onTakeDamage.Invoke();
        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            Die(this);
        }
    }

    void NameUnit()
    {
        if (possibleNames.Length > 0)
        {
            unitName = possibleNames[Random.Range(0, possibleNames.Length)];
        }
        else
        {
            unitName = "Unit_" + Random.Range(0, 1000);
        }
        name = unitName;
    }

    void RankAction()
    {
        if (rankState == RankState.FollowingGeneral)
        {
            if (myGeneral.rankState != RankState.IsGeneral)
            {
                rankState = RankState.LookingForGeneral;
                killMeAfter = 0.5f;
            }
        }
        if (rankState == RankState.LookingForGeneral)
        {
            if (ClosestGeneral())
                AssignToGeneral(ClosestGeneral());
            if (killMeAfter <= 0)
            {
                Die(this);
            }
            else
            {
                killMeAfter -= Time.deltaTime;
            }
        }
        if (rankState == RankState.dead && gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }

    void AssignToGeneral(TroopActorNetworked ta)
    {
        rankState = RankState.FollowingGeneral;
        myGeneral = ta;
        if (moveTarget)
            Destroy(moveTarget);
        moveTarget = ta.AllocateTarget(this).gameObject;
    }

    public void Die(TroopActorNetworked ta)
    {
        ta.onDie.Invoke();
        if (ta.rankState == RankState.IsGeneral)
        {
            if (ClosestAlly())
                ta.PromoteToGeneral(ClosestAlly());
            ta.rankState = RankState.dead;
            if (moveTarget)
                Destroy(moveTarget.gameObject);
        }
        ta.rankState = RankState.dead;

        if (ta.team == Team.TEAM1)
            op.team1Generals.Remove(ta);

        if (ta.team == Team.TEAM2)
            op.team2Generals.Remove(ta);

        gameObject.SetActive(false);
    }

    TroopActorNetworked ClosestGeneral()
    {
        float dis = 0f;
        TroopActorNetworked closestAlly = null;
        foreach (TroopActorNetworked ta in op.allTroopActors)
        {
            if (ta.rankState != RankState.dead)
                if ((dis == 0f || Vector3.Distance(ta.transform.position, transform.position) < dis) && ta != this && ta.team == team && ta.rankState == RankState.IsGeneral)
                {
                    dis = Vector3.Distance(ta.transform.position, transform.position);
                    closestAlly = ta;
                }
        }
        return closestAlly;
    }

    TroopActorNetworked ClosestAlly()
    {
        float dis = 0f;
        TroopActorNetworked closestAlly = null;
        foreach (TroopActorNetworked ta in op.allTroopActors)
        {
            if (ta.rankState != RankState.dead)
                if ((dis == 0f || Vector3.Distance(ta.transform.position, transform.position) < dis) && Vector3.Distance(ta.transform.position, transform.position) < influenceRadius && ta != this && ta.team == team && ta.rankState != RankState.IsGeneral)
                {
                    dis = Vector3.Distance(ta.transform.position, transform.position);
                    closestAlly = ta;
                }
        }
        return closestAlly;
    }

    TroopActorNetworked ClosestEnemy(GunSettingsNetworked gun)
    {
        float dis = 0f;
        TroopActorNetworked closestEnemy = null;
        foreach (TroopActorNetworked ta in op.allTroopActors)
        {
            if (ta.rankState != RankState.dead)
            {
                if ((dis == 0f || Vector3.Distance(ta.transform.position, transform.position) < dis) && EnemyInRange(ta.transform, gun) && ta != this && ta.team != team)
                {
                    dis = Vector3.Distance(ta.transform.position, transform.position);
                    closestEnemy = ta;
                }
            }
        }
        return closestEnemy;
    }

    public Transform AllocateTarget(TroopActorNetworked ta)
    {
        foreach (FormationPositionNetworked fp in formationPositions)
        {
            if (fp.assignedUnit.rankState == RankState.dead)
            {
                fp.taken = false;
            }
            if (!fp.taken)
            {
                fp.assignedUnit = ta;
                fp.taken = true;
                return fp.fromPos;
            }
        }
        FormationPositionNetworked newFP = new FormationPositionNetworked();
        {
            GameObject newFormPos = new GameObject("dad");
            GameObject trackedFormPos = Instantiate(newFormPos, NextFormPos() + positionOffset, transform.rotation);
            Destroy(newFormPos);
            trackedFormPos.name = "Formation Position " + formationPositions.Count + 1;
            if (!moveTarget)
            {
                CreateMoveTarget();
            }
            trackedFormPos.transform.parent = moveTarget.transform;
            newFP.fromPos = trackedFormPos.transform;
            newFP.taken = true;
            newFP.assignedUnit = ta;
            formationPositions.Add(newFP);
            return newFP.fromPos;
        }
    }

    public void PromoteToGeneral(TroopActorNetworked ta)
    {
        ta.OnBecomeGeneral.Invoke();
        ta.rankState = RankState.IsGeneral;
        ta.moveTarget = null;

        if (ta.team == Team.TEAM1)
            op.team1Generals.Add(ta);

        if (ta.team == Team.TEAM2)
            op.team2Generals.Add(ta);
    }

    bool EnemyInRange(Transform enemy, GunSettingsNetworked gun)
    {
        if (Vector3.Distance(enemy.position, transform.position) > gun.attackRangeMin && Vector3.Distance(enemy.position, transform.position) < gun.attackRangeMax)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void AttackClosestEnemy()
    {
        foreach (GunSettingsNetworked gun in guns)
        {
            if (ClosestEnemy(gun))
            {
                gun.attackTarget = ClosestEnemy(gun);
                gun.turret.rotation = Quaternion.Slerp(gun.turret.rotation, Quaternion.LookRotation(gun.attackTarget.transform.position - gun.turret.position), gun.turretAimSpeed * Time.deltaTime);
                gun.turret.localEulerAngles = new Vector3(0, gun.turret.localEulerAngles.y, 0);

                gun.barrel.rotation = Quaternion.Slerp(gun.barrel.rotation, Quaternion.LookRotation(gun.attackTarget.transform.position - gun.barrel.position), gun.barrelAimSpeed * Time.deltaTime);
                gun.barrel.localEulerAngles = new Vector3(gun.barrel.localEulerAngles.x, 0, 0);
                gun.m_gunTimer -= Time.deltaTime;

                if (gun.m_gunTimer < Time.deltaTime)
                {
                    ResetGunTimer(gun);
                    Fire(gun);
                }
            }
        }
    }

    void ResetGunTimer(GunSettingsNetworked gun)
    {
        gun.m_gunTimer = Random.Range((gun.TimeBetweenShots - (gun.TimeBetweenShots * gun.m_gunTimerRandomiser)), (gun.TimeBetweenShots + (gun.TimeBetweenShots * gun.m_gunTimerRandomiser)));
    }

    void Fire(GunSettingsNetworked gun)
    {
        gun.onShoot.Invoke();

        if (EnemyInSights(gun) && CanHitTarget(gun.attackTarget))
        {
            if (gun.attackTarget.currentHealth - gun.damage <= 0)
            {
                gun.onKillShot.Invoke();
            }

            gun.attackTarget.TakeDamage(gun.damage);
        }
    }

    bool CanHitTarget(TroopActorNetworked checkThis)
    {
        bool canHit = false;

        foreach (UnitClasses uc in strengths)
        {
            if (checkThis.unitClass == uc)
            {
                canHit = true;
            }
        }

        return canHit;
    }

    bool EnemyInSights(GunSettingsNetworked gun)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (gun.attackTarget.transform.position - transform.position), out hit, Vector3.Distance(transform.position, gun.attackTarget.transform.position)))
        {
            if (hit.transform != gun.attackTarget.transform)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    Vector3 NextFormPos()
    {
        //making Sure Old Pos Exists and is parented correctly
        if (!oldPos)
        {
            oldPos = new GameObject("oldPos " + gameObject.name).transform;
            oldPos.parent = moveTarget.transform;
            oldPos.localPosition = Vector3.zero;
            oldPos.localPosition -= new Vector3(0, 0, distanceBetweenPoints.y);
            count = 0;
            return oldPos.position;
        }
        else if (oldPos.parent != moveTarget)
        {
            oldPos.parent = moveTarget.transform;
            oldPos.localPosition = Vector3.zero;
            oldPos.localPosition -= new Vector3(0, 0, distanceBetweenPoints.y);
            count = 0;
            return oldPos.position;
        }

        if (count < maxNumOfColumns / 2)
        {
            count++;
            oldPos.localPosition += (pd == PlacementDirection.right) ? new Vector3(distanceBetweenPoints.x, 0, 0) : new Vector3(-distanceBetweenPoints.x, 0, 0);
        }
        else
        {
            count = 0;
            if (pd == PlacementDirection.left)
                oldPos.localPosition -= new Vector3(0, 0, distanceBetweenPoints.y);

            pd = (pd == PlacementDirection.right) ? PlacementDirection.left : PlacementDirection.right;
            oldPos.localPosition = (pd == PlacementDirection.right) ? new Vector3(0, 0, oldPos.localPosition.z) : new Vector3(-distanceBetweenPoints.x, 0, oldPos.localPosition.z);
        }

        return oldPos.position;
    }

    void Move()
    {
        if (!moveTarget)
        {
            CreateMoveTarget();
        }
        if (movementType == MovementTypes.Ground)
        {
            if (!m_navAgent)
            {
                m_navAgent = gameObject.AddComponent<NavMeshAgent>();
                m_navAgent.speed = moveSpeed;
                m_navAgent.angularSpeed = turnSpeed;
            }
            else if (!m_navAgent.isActiveAndEnabled)
            {
                m_navAgent.enabled = true;
            }
            else if (m_navAgent && moveTarget)
            {
                Debug.DrawLine(transform.position, moveTarget.transform.position, Color.red);
                m_navAgent.SetDestination(moveTarget.transform.position);

                if (Vector3.Distance(new Vector3(moveTarget.transform.position.x, transform.position.y, moveTarget.transform.position.z), transform.position) > moveSpeed * Time.deltaTime)
                    moving = true;
                else
                    moving = false;
            }

            LookAtMoveTarget();
        }
        if (movementType == MovementTypes.Air)
        {
            if (m_navAgent)
            {
                m_navAgent.enabled = false;
            }

            LookAtMoveTarget();

            if (ClearLineOfSight())
            {
                MoveTowardsMoveTarget();
            }
            else
            {
                Evade();
            }
        }
        if (moving)
        {
            OnMove.Invoke();
        }
    }

    bool ClearLineOfSight()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, (moveTarget.transform.position - transform.position), out hit, avoidanceRange))
        {
            if (hit.transform != moveTarget)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                return false;
            }
            else
            {
                Debug.DrawLine(transform.position, moveTarget.transform.position, Color.grey);
                return true;
            }
        }
        else
        {
            Debug.DrawLine(transform.position, moveTarget.transform.position, Color.grey);
            return true;
        }
    }

    public Vector3 previousPos;
    void LookAtMoveTarget()
    {
        Vector3 lookRotation = transform.position - previousPos;

        if (moveTarget && moving)
            if (lookRotation != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((transform.position - previousPos)), turnSpeed * Time.deltaTime);

        previousPos = transform.position;

        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }

    void Evade()
    {
        transform.position += Vector3.up * verticleSpeed * Time.deltaTime;
    }

    void MoveTowardsMoveTarget()
    {
        if (Vector3.Distance(new Vector3(moveTarget.transform.position.x, transform.position.y, moveTarget.transform.position.z), transform.position) > avoidanceRange)
        {
            transform.position += (new Vector3(moveTarget.transform.position.x, transform.position.y, moveTarget.transform.position.z) - transform.position).normalized * moveSpeed * Time.deltaTime;
            moving = true;
        }
        else if (Vector3.Distance(new Vector3(moveTarget.transform.position.x, moveTarget.transform.position.y + hoverHeight, moveTarget.transform.position.z), transform.position) > moveSpeed * Time.deltaTime)
        {
            transform.position += (new Vector3(moveTarget.transform.position.x, moveTarget.transform.position.y + hoverHeight, moveTarget.transform.position.z) - transform.position).normalized * verticleSpeed * Time.deltaTime;
            moving = true;
        }
        else
        {
            moving = false;
        }
    }

    void OnChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }
}
