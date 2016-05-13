using UnityEngine;
using System.Collections;

public class Thug : BasicAI
{
    NavMeshHit _hit;
    Vector3 _newPos;

    GunHandler _gunHandler;
    PlayerScript thisPlayerScript;

    [SerializeField] bool _Gun;
    [SerializeField] bool _Knife;    

    // Use this for initialization
    protected override void Start ()
    {
        //Find the player
        player = GameObject.FindWithTag("Player");

        trans = transform;
        navmeshagent = GetComponent<NavMeshAgent>();
        _gunHandler = GetComponent<GunHandler>();
        thisPlayerScript = GetComponent<PlayerScript>();

        if (player == null)
        {
            return;
        }

        currHitPoints = hitPoints;

        StartCoroutine(ExecuteState());

        InvokeRepeating("CheckForPlayer", 1, 0.2f);
        Invoke("RandomIdleState", Random.Range(1, 2));
    }
    
    void CheckForPlayer()
    {
        distance = (player.transform.position - trans.position).sqrMagnitude;
        sightRadius = player.transform.position - trans.position;
        print(distance);
        // Within Distance
        if (distance < 50)
        {
            // Within Sight
            if (Vector3.Angle(trans.forward, sightRadius) < 80)
            {
                Vector3 targetDir = (player.transform.position + Vector3.up * 0.5f) - eyes.transform.position;
                RaycastHit hit;

                // Not Blocked by Walls
                if (Physics.Raycast(eyes.transform.position, targetDir.normalized, out hit, rangeOfVision))
                {
                    PlayerScript playerScript = hit.transform.gameObject.GetComponent<PlayerScript>();
                    if (playerScript != null)
                    {
                        // Spotted Player
                        if (!_playerSpotted)
                        {
                            _playerSpotted = true;

                            // Alert the NPC - once the player is seen
                            navmeshagent.updateRotation = false;

                            CancelInvoke("RandomAlertState");
                            CancelInvoke("RandomIdleState");
                            currAIstate = AIstate.AGGRESSIVE;

                            // Attack 
                            AttackAction();
                        }
                    }
                }
            }
        }
        else
        {
            // Out of distance
            if (_playerSpotted)
            {
                _playerSpotted = false;

                navmeshagent.updateRotation = true;

                // AI stays on alert
                RandomAlertState();
                //currAIstate = AIstate.ALERT;
            }
        }
    }

    IEnumerator ExecuteState()
    {
        while (currAIstate == AIstate.IDLE)
        {
            // Random Idle-animations
            // anim.SetInteger

            yield return null;
        }
        
        while (currAIstate == AIstate.TURNLEFT)
        {
            // Rotate Left or Look Left            
            trans.Rotate(-Vector3.up * 20 * Time.deltaTime);

            yield return null;
        }

        while (currAIstate == AIstate.TURNRIGHT)
        {
            // Rotate Right or Look Right
            trans.Rotate(Vector3.up * 20 * Time.deltaTime);

            yield return null;
        }

        while (currAIstate == AIstate.WALK)
        {
            // Walking animation

            yield return null;
        }

        while (currAIstate == AIstate.AGGRESSIVE)
        {
            // Stare at player            
            sightRadius.y = 0;
            Quaternion rotation = Quaternion.LookRotation(sightRadius);
            trans.rotation = Quaternion.Slerp(trans.rotation, rotation, 0.9f * Time.deltaTime);

            yield return null;
        }

        // Restart coroutine
        StartCoroutine(ExecuteState());
    }

    void RandomIdleState()
    {
        float rndAction = Random.value;
        float minDelay = 1;
        float maxDelay = 2;

        if (rndAction > 0.75f)
        {
            // continue to IDLE (play idle-animations here)
            currAIstate = AIstate.IDLE;
        }
        else if (rndAction < 0.25f)
        {
            // NPC will Walk
            navmeshagent.updateRotation = true;
            currAIstate = AIstate.WALK;

            _newPos = trans.position + Random.insideUnitSphere * 5;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                //anim.SetBool("Walk", true);

                // then walk towards that spot
                navmeshagent.SetDestination(_hit.position);
            }
        }
        else if (rndAction >= 0.25f && rndAction < 0.5f)
        {
            // NPC will 'Look right' / 'Turn right'
            navmeshagent.updateRotation = false;
            currAIstate = AIstate.TURNRIGHT;

            minDelay = 0.5f;
            maxDelay = 1.5f;
        }
        else if (rndAction <= 0.75f && rndAction >= 0.5f)
        {
            // NPC will 'Look left' / 'Turn left'
            navmeshagent.updateRotation = false;
            currAIstate = AIstate.TURNLEFT;

            minDelay = 0.5f;
            maxDelay = 1.5f;
        }

        // Invoke a new random behaviour
        Invoke("RandomIdleState", Random.Range(minDelay, maxDelay));
    }

    void RandomAlertState()
    {
        float rndAction = Random.value;
        float minDelay = 1;
        float maxDelay = 2;

        if (rndAction > 0.75f)
        {
            // continue to IDLE (play idle-animations here)
            currAIstate = AIstate.IDLE;
        }
        else if (rndAction < 0.25f)
        {
            // NPC will Walk
            navmeshagent.updateRotation = true;
            currAIstate = AIstate.WALK;

            _newPos = trans.position + Random.insideUnitSphere * 5;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                //anim.SetBool("Walk", true);

                // then walk towards that spot
                navmeshagent.SetDestination(_hit.position);
            }
        }
        else if (rndAction >= 0.25f && rndAction < 0.5f)
        {
            // NPC will 'Look right' / 'Turn right'
            navmeshagent.updateRotation = false;
            currAIstate = AIstate.TURNRIGHT;

            minDelay = 0.5f;
            maxDelay = 1.5f;
        }
        else if (rndAction <= 0.75f && rndAction >= 0.5f)
        {
            // NPC will 'Look left' / 'Turn left'
            navmeshagent.updateRotation = false;
            currAIstate = AIstate.TURNLEFT;

            minDelay = 0.5f;
            maxDelay = 1.5f;
        }

        // Invoke a new random behaviour
        Invoke("RandomAlertState", Random.Range(minDelay, maxDelay));
    }

    void AttackAction()
    {
        if (_Gun)
        {
            // NPC has gun
            Shoot();
        }
        else if (_Knife)
        {
            // NPC has knife
            Knife();
        }
        else
        {
            // NPC has no weapons
            Melee();
        }
    }

    void Melee()
    {
        if (distance < 3)
        {
            // Punch

            // Punch again after a while
            Invoke("Melee", Random.Range(1, 2));
        }
        else
        {
            // Move towards the player
            _newPos = player.transform.position + Random.insideUnitSphere * 3;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                //anim.SetBool("Walk", true);

                // then walk towards that spot
                navmeshagent.SetDestination(_hit.position);
            }

            Invoke("Melee", 3);
        }
    }

    void Shoot()
    {
        if (distance < 50)
        {
            // If player is within range -
            // Shoot at player
            _gunHandler.Fire();

            // Shoot again after a while
            Invoke("Shoot", Random.Range(2, 3));
        }
        else
        {
            // If the player is not within range -
            // Chase after the player
            _newPos = player.transform.position + Random.insideUnitSphere * 5;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                //anim.SetBool("Walk", true);

                // then walk towards that spot
                navmeshagent.SetDestination(_hit.position);
            }

            Invoke("Shoot", 3);
        }
    }

    void Knife()
    {
        if (distance < 3)
        {
            // Knife the player

            // Knife again
            Invoke("Knife", Random.Range(1, 3));
        }
        else
        {
            // Move towards the player
            _newPos = player.transform.position + Random.insideUnitSphere * 3;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                //anim.SetBool("Walk", true);

                // then walk towards that spot
                navmeshagent.SetDestination(_hit.position);
            }

            Invoke("Knife", 3);
        }
    }
}
