using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AI : Stats
{    
    protected Animator anim;
    protected Transform trans;
    protected GameObject player;
    protected NavMeshAgent navmeshagent;
    protected RaycastHit _rayHit;
    protected NavMeshHit _hit;
    protected Vector3 _newPos;
    protected Vector3 _sightRadius;
    protected Vector3 _targetDir;
    protected float _distance;

    [Space(10, order = 0)]
    [Header("Sight Vision", order = 1)]
    [Space(5, order = 2)]
    [SerializeField] GameObject eyes;
    [SerializeField] bool ShowFov;
    [SerializeField] float fieldOfVision;
    [SerializeField] float rangeOfVision;

    public enum AIstate
    {
        IDLE,
        TURNLEFT,
        TURNRIGHT,
        WALK,
        AGGRESSIVE,
        ALERT,
        POSSESSED,
        DEAD,
    }

    [Space(10, order = 0)]
    [Header("Behaviours", order = 1)]
    [Space(5, order = 2)]
    public AIstate currAIstate;

    [Space(10, order = 0)]
    [Header("Weapons", order = 1)]
    [Space(5, order = 2)]
    public WeaponBag weapons;
    public Transform weaponBagPos;

    protected bool _stareAtPlayer;
    protected bool _possessed;
    protected bool _spottedPlayer;
    protected bool _dead;

    // Use this for initialization
    protected void Setup()
    {
        //Find the player
        player = GameObject.FindWithTag("Player");
        
        trans = transform;
        anim = GetComponentInChildren<Animator>();
        navmeshagent = GetComponent<NavMeshAgent>();
        
        if (player == null)
        {
            return;
        }                

        StartCoroutine("CheckStatus");
        StartCoroutine("StareAtPlayer");
        InvokeRepeating("FindPlayer", 1, 0.2f);
    }

    void FindPlayer()
    {
        _distance = (player.transform.position - trans.position).sqrMagnitude;
        _sightRadius = player.transform.position - trans.position;
        _targetDir = (player.transform.position + Vector3.up * 0.5f) - eyes.transform.position;

        if (_spottedPlayer)
        {
            // cannot see player - hidden by obstacles
            if (Physics.Raycast(eyes.transform.position, _targetDir.normalized, out _rayHit, rangeOfVision))
            {
                if (!_rayHit.collider.CompareTag("Player"))
                {
                    _spottedPlayer = false;
                    return;
                }
            }

            // Cannot see player - out of distance/view
            if (_distance >= 50 || Vector3.Angle(trans.forward, _sightRadius) >= 80)
            {
                _spottedPlayer = false;        
            }
            return;
        }
        else if (!_spottedPlayer)
        {
            // Player Out of Distance
            if (_distance >= 50)
            {
                return;
            }
            // Player Within Distance
            else if (_distance < 50)
            {
                // Player Not within sight
                if (Vector3.Angle(trans.forward, _sightRadius) >= 80)
                {
                    return;
                }
                // Player Within Sight
                else if (Vector3.Angle(trans.forward, _sightRadius) < 80)
                {
                    // Player Not Blocked by Walls
                    if (Physics.Raycast(eyes.transform.position, _targetDir.normalized, out _rayHit, rangeOfVision))
                    {
                        if (!_rayHit.collider.CompareTag("Player"))
                        {
                            return;
                        }
                        else if (_rayHit.collider.CompareTag("Player"))
                        {
                            PlayerScript playerScript = _rayHit.transform.gameObject.GetComponent<PlayerScript>();

                            if (playerScript != null)
                            {
                                // Spotted Player    
                                _spottedPlayer = true;
                            }
                        }
                    }
                }
            }
        }
    }

    protected IEnumerator StareAtPlayer()
    {
        while (_stareAtPlayer)
        {
            // Stare at player            
            _sightRadius.y = 0;
            Quaternion lookdir = Quaternion.LookRotation(_sightRadius);
            trans.rotation = Quaternion.Slerp(trans.rotation, lookdir, 2f * Time.deltaTime);

            yield return null;
        }

        while (!_stareAtPlayer)
        {   
            yield return null;
        }

        StartCoroutine("StareAtPlayer");
    }

    IEnumerator CheckStatus()
    {
        while (!_possessed)
        {
            // AI dies
            if (currHP <= 0)
            {
                currHP = 0;

                if (currAIstate != AIstate.DEAD)
                {
                    currAIstate = AIstate.DEAD;
                    Invoke("Death", 1.5f);
                }
            }

            yield return null;
        }

        while (_possessed)
        {
            yield return null;
        }

        StartCoroutine("CheckStatus");
    }

    void FieldOfVisionDisplay(float fov, GameObject eyePos, float range)
    {
        Vector3 forward = trans.forward;
        Vector3 leftRayRotation = Quaternion.AngleAxis(-fov*0.5f, trans.up) * forward;
        Vector3 rightRayRotation = Quaternion.AngleAxis(fov*0.5f, trans.up) * forward;

        Debug.DrawRay(trans.position, forward.normalized * range, Color.red);
        Debug.DrawRay(trans.position, leftRayRotation.normalized * range, Color.red);
        Debug.DrawRay(trans.position, rightRayRotation.normalized * range, Color.red);
    }

	void LateUpdate ()
    {
        //For Game Designers :)
        if (ShowFov)
        {
            FieldOfVisionDisplay(fieldOfVision, eyes, rangeOfVision);
        }
    }

    // AI acquires weapon
    //public void GetWeapon()
    //{
    //    WeaponsList.Add()
    //}

    // AI becomes possessed
    public virtual void PossessAI(){}

    // AI becomes unpossesed
    public virtual void UnPossessAI() { }

    void Death()
    {
        Destroy(gameObject);
    }
}
