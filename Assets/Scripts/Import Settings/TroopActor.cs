using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;

[System.Serializable]
public enum AttackType {
	SELECTED, AUTO
}

[System.Serializable]
public enum UnitClasses
{
    Tank,
    AntAir,
    Helicopter
}

[System.Serializable]
public enum RankState
{
    LookingForGeneral,
    FollowingGeneral,
    IsGeneral,
    dead,
    Base
}

[System.Serializable]
public enum MovementTypes
{
    Air,
    Ground
}

[System.Serializable]
public enum PlacementDirection
{
    left,
    right
}

[System.Serializable]
public class GunSettings
{
    [Header("Gun Stuff")]
    public float damage;
    public float TimeBetweenShots;
    public float m_gunTimerRandomiser;
    public float m_gunTimer;
    public float attackRangeMin;
    public float attackRangeMax;
    public TroopActor attackTarget;

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
public class FormationPosition
{
    public Transform fromPos;
    public TroopActor assignedUnit;
    public bool taken;
}

public class TroopActor : MonoBehaviour
{
    [Header("Unit Settings")]
    [SerializeField]
    public UnitClasses unitClass;
	public AttackType attackType;
    [SerializeField] public Team team;
    [SerializeField] private UnitClasses[] strengths;
    [SerializeField] private UnitClasses[] vulnerabilities;

    [Header("Rank Settings")]
    [SerializeField]
    public RankState rankState;
    [SerializeField] public TroopActor myGeneral;
    public float killMeAfter = 0.5f;

    [Header("Formation Settings")]
    [SerializeField]
    private PlacementDirection placementDirection;
    [SerializeField] private float numOfRows;
    [SerializeField] private float maxNumOfColumns;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector2 distanceBetweenPoints;
    [SerializeField] public List<FormationPosition> formationPositions = new List<FormationPosition>();
    [SerializeField] private Transform oldPos;
    [SerializeField] private PlacementDirection pd;
    [SerializeField] private int count;


    [Header("Promotion Settings")]
    [SerializeField]
    private int rank;
    [SerializeField] private UnityEvent OnGainExperience;
    [SerializeField] private UnityEvent OnPromotion;
    [SerializeField] private UnityEvent OnBecomeGeneral;
    [SerializeField] private string[] rankTitles;
    [SerializeField] private float experienceRequiredForNextRank;
    [SerializeField] private float currentExperience;
    [SerializeField] private float percentageIncreasePerRank;
    [SerializeField] private float influenceRadius;

    [Header("Name Settings")]
    [SerializeField]
    public string unitName;
    [SerializeField] private string[] possibleNames;

    [Header("Health Settings")]
    public float maxHealth;
    [SerializeField] public float currentHealth;
    [SerializeField] private UnityEvent onTakeDamage;
    [SerializeField] private UnityEvent onDie;
    public Image m_healthBar;

    [Header("Movement Settings")]
    [SerializeField]
    private bool moving;
    [SerializeField] private UnityEvent OnMove;
    public Transform moveTarget;
    [SerializeField] private MovementTypes movementType;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    public GameObject moveTargetPrefab;
    public GameObject generalMoveTargetPrefab;

    [Header("Air Movement Settings")]
    [SerializeField]
    private float hoverHeight;
    [SerializeField] private float verticleSpeed;

    [Header("Ground Movement Settings")]
    [SerializeField]
    private NavMeshAgent m_navAgent;

    [Header("Object Avoidance Settings")]
    [SerializeField]
    private float avoidanceRange;

    [Header("Attack Settings")]
    [SerializeField]
    private GunSettings[] guns;

    [Header("Important Optimisation List")]
    public ObjectPool op;

    [Header("Renderer")]
    public Renderer[] primaryRenderers;
    public Renderer[] secondaryRenderers;

    [Header("Chosen Factions")]
    public SelectedFactions selectedFactions;

	[Header("Target To Attack - Do not set this in inspector")]
	public TroopActor targetToAttack;

    [Header("Images")]
    public Sprite[] images;
    public Image imageHolder;

    [Header("Line Renderer")]
    public Gradient gradient;
    LineRenderer lineRenderer;
    ZoneController zc;

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                      / START FUNCTION BELOW \
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        m_navAgent = GetComponent<NavMeshAgent>();
		attackType = AttackType.AUTO;

        zc = FindObjectOfType<ZoneController>();

        if (team == Team.TEAM1)
            onDie.AddListener(zc.UpdatePlayer2KillScore);
        if (team == Team.TEAM2)
            onDie.AddListener(zc.UpdatePlayer1KillScore);

        try
        {
            selectedFactions = FindObjectOfType<SelectedFactions>();

            if (team == Team.TEAM1)
            {
                for (int i = 0; i < primaryRenderers.Length; i++)
                    primaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team1.primaryColour);

                for (int i = 0; i < secondaryRenderers.Length; i++)
                    secondaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team1.secondaryColour);
            }
            else
            {
                for (int i = 0; i < primaryRenderers.Length; i++)
                    primaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team2.primaryColour);

                for (int i = 0; i < secondaryRenderers.Length; i++)
                    secondaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team2.secondaryColour);
            }
        }
        catch(System.Exception)
        {
            for (int i = 0; i < primaryRenderers.Length; i++)
                primaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), Color.black);

            for (int i = 0; i < secondaryRenderers.Length; i++)
                secondaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), Color.black);
        }

        op = FindObjectOfType<ObjectPool>();
        SetHealth(maxHealth);
        NameUnit();
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                     \ START FUNCTION ABOVE /
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetHealth(float newHealth)
    {
        currentHealth = newHealth;

        if (m_healthBar)
        m_healthBar.fillAmount = currentHealth / maxHealth;
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
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                     /  UPDATE FUNCTION BELOW \
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    float timer = 1f;
    void Update()
    {
        RankAction();

        if (rankState != RankState.dead && rankState != RankState.Base) {

            timer -= Time.deltaTime;
            if (timer <= 0 && lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                timer = 1f;
            }

            UpdateImage();

            Move ();

			if (attackType == AttackType.AUTO) {
				AttackClosestEnemy ();
			} else
            {
                if (moveTarget.gameObject.activeInHierarchy == true)
                {
                    moveTarget.gameObject.SetActive(false);
                }

                AttackTarget ();
			}
		}
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                      \ UPDATE FUNCTION ABOVE /
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	public void SetAttackType(AttackType type) {
		attackType = type;
	}

	void AttackTarget() {
		if (targetToAttack.rankState != RankState.dead)
			AttackSelectedEnemy (targetToAttack);
		else {
			attackType = AttackType.AUTO;
			Move ();
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
            TroopActor AssignToMe = RandomGeneral();
            if (AssignToMe)
                AssignToGeneral(AssignToMe);
            if (killMeAfter <= 0)
            {
                Die(this);
            }
            else
            {
                killMeAfter -= Time.deltaTime;
            }
        }
        if (rankState == RankState.dead)
        {
            if (moveTarget)
                moveTarget.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    void AssignToGeneral(TroopActor ta)
    {
        rankState = RankState.FollowingGeneral;
        myGeneral = ta;
        if (moveTarget)
            Destroy(moveTarget.gameObject);
        moveTarget = ta.AllocateTarget(this);
    }

    public Transform AllocateTarget(TroopActor ta)
    {
        foreach (FormationPosition fp in formationPositions)
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
        FormationPosition newFP = new FormationPosition();
        {
            GameObject newFormPos = new GameObject("dad");
            GameObject trackedFormPos = Instantiate(moveTargetPrefab, NextFormPos() + positionOffset, transform.rotation);
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
            oldPos.parent = moveTarget;
            oldPos.localPosition = Vector3.zero;
            oldPos.localPosition -= new Vector3(0, 0, distanceBetweenPoints.y);
            count = 0;
            return oldPos.position;
        }
        else if (oldPos.parent != moveTarget)
        {
            oldPos.parent = moveTarget;
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

	bool withinRange(GunSettings gun, TroopActor troop) {
		if (Vector3.Distance (troop.transform.position, transform.position) > gun.attackRangeMin && Vector3.Distance (troop.transform.position, transform.position) < gun.attackRangeMax) 
		{
			return true;
		}
		else 
		{
			return false;
		}
	}

	void AttackSelectedEnemy(TroopActor troopToAttack)
	{
		foreach (GunSettings gun in guns)
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
                    DrawLineRender(gun.turret, troopToAttack.transform);

                    ResetGunTimer(gun);
					Fire(gun);
				}
			}
		}
	}


    void AttackClosestEnemy()
    {
        foreach (GunSettings gun in guns)
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
                    DrawLineRender(gun.turret, gun.attackTarget.transform);

                    ResetGunTimer(gun);
                    Fire(gun);
                }
            }
        }
    }

    void DrawLineRender(Transform orgin, Transform target)
    {
        lineRenderer.colorGradient = gradient;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, orgin.position);
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        float dist = Vector3.Distance(orgin.position, target.position);

        float x = Mathf.Lerp(0, dist, Time.deltaTime);

        Vector3 pointAlongLine = x * Vector3.Normalize(orgin.position - target.position) + target.position;

        lineRenderer.SetPosition(1, pointAlongLine);
    }

    void ResetGunTimer(GunSettings gun)
    {
        gun.m_gunTimer = Random.Range((gun.TimeBetweenShots - (gun.TimeBetweenShots * gun.m_gunTimerRandomiser)), (gun.TimeBetweenShots + (gun.TimeBetweenShots * gun.m_gunTimerRandomiser)));
    }

    void Fire(GunSettings gun)
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

    bool CanHitTarget(TroopActor checkThis)
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

    bool EnemyInSights(GunSettings gun)
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

    TroopActor ClosestEnemy(GunSettings gun)
    {
        float dis = 0f;
        TroopActor closestEnemy = null;
        foreach (TroopActor ta in op.allTroopActors)
        {
            if (ta.rankState != RankState.dead && IsAttackableUnit(ta))
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

    TroopActor ClosestAlly()
    {
        float dis = 0f;
        TroopActor closestAlly = null;
        foreach (TroopActor ta in op.allTroopActors)
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

    TroopActor RandomGeneral()
    {
        List<TroopActor> Generals = new List<TroopActor>();
        foreach (TroopActor ta in op.allTroopActors)
        {
            if (ta.rankState != RankState.dead)
                if (ta != this && ta.team == team && ta.rankState == RankState.IsGeneral)
                {
                    Generals.Add(ta);
                }
        }
        return Generals[Random.Range(0, Generals.Count)];
    }


    TroopActor ClosestGeneral()
    {
        float dis = 0f;
        TroopActor closestAlly = null;
        foreach (TroopActor ta in op.allTroopActors)
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
        foreach (TroopActor ta in op.allTroopActors)
        {
            if (ta != this && ta.team == team && ta.rankState == RankState.IsGeneral)
            {
                return true;
            }
        }
        return false;
    }
    bool EnemyInRange(Transform enemy, GunSettings gun)
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
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                     MOVE STUFF
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
        else if(moveTarget.gameObject.activeInHierarchy == false)
        {
            moveTarget.gameObject.SetActive(true);
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
                if (m_navAgent.speed != moveSpeed)
                {
                    m_navAgent.speed = moveSpeed;
                }
                if (m_navAgent.angularSpeed != turnSpeed)
                {
                    m_navAgent.angularSpeed = turnSpeed;
                }
            }
            else if (m_navAgent && moveTarget)
            {
                if (m_navAgent.speed != moveSpeed)
                {
                    m_navAgent.speed = moveSpeed;
                }
                if (m_navAgent.angularSpeed != turnSpeed)
                {
                    m_navAgent.angularSpeed = turnSpeed;
                }
                Debug.DrawLine(transform.position, moveTarget.position, Color.red);
                if (m_navAgent.isOnNavMesh)
                {
                    m_navAgent.stoppingDistance = (attackType == AttackType.AUTO) ? 0 : guns[0].attackRangeMin;
					m_navAgent.SetDestination((attackType == AttackType.AUTO) ? moveTarget.position : targetToAttack.transform.position);
                }
                else
                {
                    if (rankState != RankState.IsGeneral && myGeneral)
                    {
                        	transform.position = moveTarget.position;

                        m_navAgent.enabled = false;
                        Invoke("NavOn", 0.1f);
                    }
                }
                if (Vector3.Distance(new Vector3(moveTarget.position.x, transform.position.y, moveTarget.position.z), transform.position) > moveSpeed * Time.deltaTime)
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
                if (attackType == AttackType.AUTO)
                {
                    MoveTowardsMoveTarget();
                }
                else
                {
                    if (Vector3.Distance(moveTarget.position, transform.position) > guns[0].attackRangeMin)
                    {
                        MoveTowardsMoveTarget();
                    }
                }
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


    void CreateMoveTarget()
    {
        if (rankState == RankState.IsGeneral)
        {
            GameObject keepThisAlive = Instantiate(generalMoveTargetPrefab, transform.position, transform.rotation);
            keepThisAlive.name = gameObject.name + "'s MoveTarget";
            moveTarget = keepThisAlive.transform;
        }
        else if (rankState != RankState.dead && rankState != RankState.Base)
        {
            GameObject keepThisAlive = Instantiate(moveTargetPrefab, transform.position, transform.rotation);
            keepThisAlive.name = gameObject.name + "'s MoveTarget";
            moveTarget = keepThisAlive.transform;
        }
        else
        {

        }
    }

    bool ClearLineOfSight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (moveTarget.position - transform.position), out hit, avoidanceRange))
        {
            if (hit.transform != moveTarget)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                return false;
            }
            else
            {
                Debug.DrawLine(transform.position, moveTarget.position, Color.grey);
                return true;
            }
        }
        else
        {
            Debug.DrawLine(transform.position, moveTarget.position, Color.grey);
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
        if(attackType == AttackType.AUTO)
        {
            if (Vector3.Distance(new Vector3(moveTarget.position.x, transform.position.y, moveTarget.position.z), transform.position) > avoidanceRange)
            {
                transform.position += (new Vector3(moveTarget.position.x, transform.position.y, moveTarget.position.z) - transform.position).normalized * moveSpeed * Time.deltaTime;
                moving = true;
            }
            else if (Vector3.Distance(new Vector3(moveTarget.position.x, moveTarget.position.y + hoverHeight, moveTarget.position.z), transform.position) > moveSpeed * Time.deltaTime)
            {
                transform.position += (new Vector3(moveTarget.position.x, moveTarget.position.y + hoverHeight, moveTarget.position.z) - transform.position).normalized * verticleSpeed * Time.deltaTime;
                moving = true;
            }
            else
            {
                moving = false;
            }
        }
        else
        {
            if (Vector3.Distance(new Vector3(targetToAttack.transform.position.x, transform.position.y, targetToAttack.transform.position.z), transform.position) > avoidanceRange)
            {
                transform.position += (new Vector3(targetToAttack.transform.position.x, transform.position.y, targetToAttack.transform.position.z) - transform.position).normalized * moveSpeed * Time.deltaTime;
                moving = true;
            }
            else if (Vector3.Distance(new Vector3(targetToAttack.transform.position.x, targetToAttack.transform.position.y + hoverHeight, targetToAttack.transform.position.z), transform.position) > moveSpeed * Time.deltaTime)
            {
                transform.position += (new Vector3(targetToAttack.transform.position.x, targetToAttack.transform.position.y + hoverHeight, targetToAttack.transform.position.z) - transform.position).normalized * verticleSpeed * Time.deltaTime;
                moving = true;
            }
            else
            {
                moving = false;
            }
        }
    }

    void UpdateImage()
    {
        if (currentHealth > 70f)
        {
            imageHolder.sprite = images[0];
        }
        else if (currentHealth < 70f && currentHealth > 20f)
        {
            imageHolder.sprite = images[1];
        }
        else if (currentHealth < 20f)
        {
            imageHolder.sprite = images[2];
        }
    }

    public void TakeDamage(float damageToTake)
    {
        onTakeDamage.Invoke();
        currentHealth -= damageToTake;
        if (currentHealth <= 0)
        {
            Die(this);
        }

        m_healthBar.fillAmount = currentHealth / maxHealth;
    }

    public void Die(TroopActor ta)
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
        foreach (TroopActor ta in op.allTroopActors)
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
        foreach (GunSettings gun in guns)
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

    public void PromoteToGeneral(TroopActor ta)
    {
        ta.OnBecomeGeneral.Invoke();
        ta.rankState = RankState.IsGeneral;
        ta.moveTarget = null;

        if (ta.team == Team.TEAM1)
            op.team1Generals.Add(ta);

        if (ta.team == Team.TEAM2)
            op.team2Generals.Add(ta);
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    //BUFFS\\
    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    public void AddHealth(float percentIncrease)
    {
        currentHealth += maxHealth * (percentIncrease / 100);
        if (currentHealth > maxHealth)
        {
            maxHealth = currentHealth;
        }
    }

    public void AddRange(float percentIncrease)
    {
        foreach (GunSettings gun in guns)
        {
            gun.attackRangeMax += gun.attackRangeMax * (percentIncrease / 100);
        }
    }

    public void AddInfluence(float percentIncrease)
    {
        influenceRadius += influenceRadius * (percentIncrease / 100);
    }

    public void AddDamage(float percentIncrease)
    {
        foreach (GunSettings gun in guns)
        {
            gun.damage += gun.damage * (percentIncrease / 100);
        }
    }

    public void AddRandomBuff(float percentIncrease)
    {
        int selection = Random.Range(0, 3 + 1);
        if (selection == 0)
        {
            AddHealth(percentIncrease);
        }
        else if (selection == 1)
        {
            AddRange(percentIncrease);
        }
        else if (selection == 2)
        {
            AddInfluence(percentIncrease);
        }
        else if (selection == 3)
        {
            AddDamage(percentIncrease);
        }
    }
}