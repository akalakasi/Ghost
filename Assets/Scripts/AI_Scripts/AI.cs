using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AI : Stats
{    
    protected Animator anim;
    protected Transform trans;
    protected GameObject player;
    protected AudioSource _audio;
    protected NavMeshAgent navmeshagent;
    protected RaycastHit _rayHit;
    protected NavMeshHit _hit;
    protected Vector3 _newPos;
    protected Vector3 _angleViewToPlayer;
    protected Vector3 _targetDir;
    protected Vector3 _spotToLook;
    protected GameObject _target;    
    protected float _distanceToPlayer;

    [Space(10, order = 0)]
    [Header("Sight Vision", order = 1)]
    [Space(5, order = 2)]
    [SerializeField] GameObject eyes;
    [SerializeField] bool ShowFov;
    [SerializeField] float fieldOfVision;
    [SerializeField] float rangeOfVision;

    [Space(10, order = 0)]
    [Header("Weapons", order = 1)]
    [Space(5, order = 2)]
    public WeaponBag weapons;
    public Transform weaponBagPos;
   
    protected bool _spottedPlayer;
    protected bool _executedState;
    protected bool _startled;
    protected bool _possessed;

    // Use this for initialization
    protected void Setup()
    {
        //Find the player
        player = GameObject.FindWithTag("Player");

        trans = transform;
        anim = GetComponentInChildren<Animator>();
        _audio = GetComponent<AudioSource>();
        navmeshagent = GetComponent<NavMeshAgent>();
        
        if (player == null)
        {
            return;
        }                

        StartCoroutine("CheckStatus");
        StartCoroutine("StareAtTarget");
        InvokeRepeating("FindPlayer", 1, 0.2f);
    }

    void FindPlayer()
    {
        _distanceToPlayer = (player.transform.position - trans.position).sqrMagnitude;
        _angleViewToPlayer = player.transform.position - trans.position;
        _targetDir = (player.transform.position + Vector3.up * 0.5f) - eyes.transform.position;

        if (_spottedPlayer)
        {
            // cannot see player - hidden by obstacles
            if (Physics.Raycast(eyes.transform.position, _targetDir.normalized, out _rayHit, rangeOfVision))
            {
                // Collider isn't player
                if (!_rayHit.collider.CompareTag("Player"))
                {
                    _spottedPlayer = false;

                    return;
                }
            }

            // Cannot see player - out of distance/view
            if (_distanceToPlayer >= 50 && Vector3.Angle(trans.forward, _angleViewToPlayer) >= 80)
            {
                _spottedPlayer = false;
            }
            return;
        }
        else if (!_spottedPlayer)
        {
            // Player Out of Distance
            if (_distanceToPlayer >= 50)
            {
                return;
            }
            // Player Within Distance
            else if (_distanceToPlayer < 50)
            {
                // Player Not within sight
                if (Vector3.Angle(trans.forward, _angleViewToPlayer) >= 80)
                {
                    return;
                }
                // Player Within Sight
                else if (Vector3.Angle(trans.forward, _angleViewToPlayer) < 80)
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
                                _target = player;
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator CheckStatus()
    {
        // AI still alive
        while (currHP > 0)
        {
            yield return null;
        }

        // AI dies
        while (currHP <= 0)
        {
            Dead();

            yield break;
        }
    }

    protected IEnumerator StareAtTarget()
    {
        // While there is a target to look at
        while (_target)
        {
            Vector3 targetDir = _target.transform.position - trans.position;
            targetDir.y = 0;
            Quaternion lookDir = Quaternion.LookRotation(targetDir);
            trans.rotation = Quaternion.Slerp(trans.rotation, lookDir, 4f * Time.deltaTime);
            
            yield return null;
        }

        // While there are no targets to look at
        while (!_target)
        {
            yield return null;
        }

        StartCoroutine("StareAtTarget");
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

    protected bool CheckDestination(float _stoppingDistance)
    {
        // Check if the navmeshagent is enabled
        if (navmeshagent.enabled)
        {
            // Check if agent is still computing the path
            if (!navmeshagent.pathPending)
            {
                // Check if agent has reached destination
                if (navmeshagent.remainingDistance <= _stoppingDistance)
                {
                    // Check if there is path / agent velocity
                    if (!navmeshagent.hasPath || navmeshagent.velocity.sqrMagnitude == 0)
                    {
                        // Stop Running
                        navmeshagent.ResetPath();
                        anim.SetBool("Walk", false);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // Makes an AI stop all of its operations
    public virtual void HaltActions()
    {
        // Stop all actions
        if (navmeshagent.enabled)
        {
            navmeshagent.ResetPath();
            navmeshagent.enabled = false;            
        }

        anim.SetBool("Walk", false);
    }

    // Makes an AI continue all of its operations
    public virtual void ContinueActions()
    {
        _startled = false;

        // Continue actions
        navmeshagent.enabled = true;
    }

    // AI hears a noise
    public virtual void HeardNoise(GameObject _noiseSource)
    {
        // Was the AI startled by anything?
        if (!_startled)
        {
            // then the AI becomes startled
            _startled = true;

            // Stop all actions and turn towards the source of the noise
            HaltActions();

            _target = _noiseSource;

            // Continue behaviour after staring at the noise
            Invoke("ContinueActions", 2);
        }
    }

    // AI becomes possessed
    public virtual void PossessAI()
    {
        if (!_possessed)
        {
            _possessed = true;

            // Stop all actions
            HaltActions();
        }
    }

    // AI becomes unpossesed
    public virtual void UnPossessAI()
    {
        if (_possessed)
        {
            _possessed = false;

            // Continue actions
            ContinueActions();
        }
    }

    public virtual void Dead()
    {
        Destroy(gameObject);
    }
}
