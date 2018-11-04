using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.AI;

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

[System.Serializable]
public class FormationPositionNetworked
{
    public Transform fromPos;
    public TroopActorNetworked assignedUnit;
    public bool taken;
}

public class TroopActorNetworked : NetworkBehaviour
{
    [Header("Unit Settings")]
    public UnitClasses unitClass;
    public AttackType attackType;
    public Team team;
    [SerializeField] UnitClasses[] strengths;
    [SerializeField] UnitClasses[] vulnerabilities;

    [Header("Rank Settings")]
    public RankState rankState;
    public TroopActorNetworked myGeneral;
    public float killMeAfter = 0.5f;

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

    [Header("Promotion Settings")]
    [SerializeField] int rank;
    [SerializeField] UnityEvent OnGainExperience;
    [SerializeField] UnityEvent OnPromotion;
    [SerializeField] UnityEvent OnBecomeGeneral;
    [SerializeField] string[] rankTitles;
    [SerializeField] float experienceRequiredForNextRank;
    [SerializeField] float currentExperience;
    [SerializeField] float percentageIncreasePerRank;
    [SerializeField] float influenceRadius;

    [Header("Name Settings")]
    public string unitName;
    [SerializeField] string[] possibleNames;

    [Header("Health Settings")]
    public const int maxHealth = 100;
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;
    [SerializeField] UnityEvent onTakeDamage;
    [SerializeField] UnityEvent onDie;
    public RectTransform healthBar;

    [Header("Movement Settings")]
    [SerializeField] bool moving;
    [SerializeField] UnityEvent OnMove;
    public GameObject moveTarget;
    [SerializeField] MovementTypes movementType;
    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed;
    public GameObject moveTargetPrefab;
    public GameObject generalMoveTargetPrefab;

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

    [Header("Target To Attack - Do not set this in inspector")]
    public TroopActorNetworked targetToAttack;

    // Use this for initialization
    void Start()
    {
        attackType = AttackType.AUTO;

        op = FindObjectOfType<ObjectPoolNetworked>();
        SetHealth(maxHealth);
        /*Give a Name Troop*/
        NameUnit();
    }

    public void SetHealth(int newHealth)
    {
        currentHealth = newHealth;
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

    // Update is called once per frame
    void Update()
    {
        //Keep Troop state updated each frame 
        RankAction();

        if (rankState != RankState.dead && rankState != RankState.Base)
        {
            Move();
            if (attackType == AttackType.AUTO)
            {
                AttackClosestEnemy();
            }
            else
            {
                AttackTarget();
            }
        }
    }

    public void SetAttackType(AttackType type)
    {
        attackType = type;
    }

    void AttackTarget()
    {
        if (targetToAttack.rankState != RankState.dead)
            AttackSelectedEnemy(targetToAttack);
        else
        {
            attackType = AttackType.AUTO;
            Move();
            targetToAttack = null;
        }
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

    bool IsAttackableUnit(TroopActor t)
    {
        foreach (UnitClasses unitclass in strengths)
        {
            if (t.unitClass == unitclass)
            {
                return true;
            }
        }

        return false;
    }

    bool withinRange(GunSettingsNetworked gun, TroopActorNetworked troop)
    {
        if (Vector3.Distance(troop.transform.position, transform.position) > gun.attackRangeMin && Vector3.Distance(troop.transform.position, transform.position) < gun.attackRangeMax)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void AttackSelectedEnemy(TroopActorNetworked troopToAttack)
    {
        foreach (GunSettingsNetworked gun in guns)
        {
            if (troopToAttack && withinRange(gun, troopToAttack))
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

        RaycastHit hit;

        if (gun.attackTarget.currentHealth - gun.damage <= 0)
        {
            gun.onKillShot.Invoke();
        }

        if (Physics.Raycast(transform.position, (gun.attackTarget.transform.position - transform.position), out hit, Vector3.Distance(transform.position, gun.attackTarget.transform.position)))
        {
            if (hit.transform.gameObject.GetComponent<TroopActorNetworked>() != null)
                hit.transform.gameObject.GetComponent<TroopActorNetworked>().TakeDamage(10);
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

    TroopActorNetworked RandomGeneral()
    {
        List<TroopActorNetworked> Generals = new List<TroopActorNetworked>();
        foreach (TroopActorNetworked ta in op.allTroopActors)
        {
            if (ta.rankState != RankState.dead)
                if (ta != this && ta.team == team && ta.rankState == RankState.IsGeneral)
                {
                    Generals.Add(ta);
                }
        }
        return Generals[Random.Range(0, Generals.Count)];
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
    bool GeneralsRemaining()
    {
        foreach (TroopActorNetworked ta in op.allTroopActors)
        {
            if (ta != this && ta.team == team && ta.rankState == RankState.IsGeneral)
            {
                return true;
            }
        }
        return false;
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

    void NavON()
    {
        m_navAgent.enabled = true;
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
                if (m_navAgent.isOnNavMesh)
                {
                    m_navAgent.SetDestination((attackType == AttackType.AUTO) ? moveTarget.transform.position : targetToAttack.transform.position);
                }
                else
                {
                    if (rankState != RankState.IsGeneral && myGeneral)
                    {
                        transform.position = moveTarget.transform.position;

                        m_navAgent.enabled = false;
                        Invoke("NavOn", 0.1f);
                    }
                }
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

    public void CreateMoveTarget()
    {
        Destroy(moveTarget);
        moveTarget = generalMoveTargetPrefab ? Instantiate(generalMoveTargetPrefab, transform.position, transform.rotation) :
            Instantiate(moveTargetPrefab, transform.position, transform.rotation);

        moveTarget.name = gameObject.name + "'s MoveTarget";

        moveTarget.AddComponent<NetworkIdentity>();
        moveTarget.AddComponent<NetworkTransform>();
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

    public void TakeDamage(int damageToTake)
    {
        onTakeDamage.Invoke();
        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            Die(this);
        }

        OnChangeHealth(currentHealth);
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

        if (!GeneralsRemaining())
        {
            Invoke("Victory", 2f);
            if (team == Team.TEAM1)
                FindObjectOfType<FindWinner>().TriggerTeam2Win();
            else
                FindObjectOfType<FindWinner>().TriggerTeam1Win();
        }
    }

    void Victory()
    {
        foreach (TroopActorNetworked ta in op.allTroopActors)
        {
            if (ta.team == FindObjectOfType<FindWinner>().winner && ta.rankState != RankState.dead && ta.rankState != RankState.Base)
            {
                foreach (Animator anim in ta.gameObject.GetComponentsInChildren<Animator>())
                {
                    anim.SetTrigger("Win");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (GunSettingsNetworked gun in guns)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, gun.attackRangeMin);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, gun.attackRangeMax);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, influenceRadius);

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, avoidanceRange);
    }

    public void grantExperience(float expToGrant)
    {
        OnGainExperience.Invoke();
        currentExperience += expToGrant;
        if (currentExperience >= experienceRequiredForNextRank)
        {
            RankUp();
        }
    }

    void RankUp()
    {

        if (rank + 1 < rankTitles.Length)
        {
            OnPromotion.Invoke();
            rank++;
            name = rankTitles[rank] + " " + unitName;
            experienceRequiredForNextRank += experienceRequiredForNextRank * percentageIncreasePerRank / 100f;
            currentExperience = 0f;
            influenceRadius += influenceRadius * percentageIncreasePerRank / 100f;
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

    void OnChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }

    [Command]
    public void CmdUpdateMoveTargetPosition(Vector3 pos, Quaternion rot)
    {
        moveTarget.transform.position = pos;
        moveTarget.transform.rotation = rot;
    }

    [ClientRpc]
    public void RpcUpdateMoveTargetPosition(Vector3 pos, Quaternion rot)
    {
        moveTarget.transform.position = pos;
        moveTarget.transform.rotation = rot;
    }
}
