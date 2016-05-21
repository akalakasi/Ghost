using UnityEngine;
using System.Collections;

public class Thug : AI
{
    [SerializeField] Weapon pistol;
    [SerializeField] bool has_Gun;
    [SerializeField] bool has_Knife;

    bool _takeAction;

    void Start ()
    {
        // Basic AI-setup
        Setup();

        IdleBehaviour();
        StartCoroutine(ExecuteState());        
        //Invoke("RandomIdleState", Random.Range(1, 2));
        InvokeRepeating("CheckForPlayer", 1, 1);
    }
    
    void CheckForPlayer()
    {
        switch (_spottedPlayer)
        {
            case true:

                // Alert the NPC - once the player is seen
                if (currAIstate != AIstate.AGGRESSIVE)
                {
                    currAIstate = AIstate.AGGRESSIVE;
                    navmeshagent.updateRotation = false;
                    navmeshagent.ResetPath();                                     

                    CancelInvoke("RandomAlertState");
                    CancelInvoke("RandomIdleState");

                    // Attack 
                    //AttackAction();
                }

                break;

            case false:

                // If this Thug was originally Aggressive
                if (currAIstate == AIstate.AGGRESSIVE)
                {
                    // he will continue to be Alert
                    currAIstate = AIstate.ALERT;
                    navmeshagent.updateRotation = true;

                    CancelInvoke("Melee");
                    CancelInvoke("Knife");
                    CancelInvoke("Shoot");

                    // AI stays on alert
                    RandomAlertState();
                }                 

                break;
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
            _sightRadius.y = 0;
            Quaternion lookdir = Quaternion.LookRotation(_sightRadius);
            trans.rotation = Quaternion.Slerp(trans.rotation, lookdir, 2f * Time.deltaTime);

            if (_distance < 50)
            {
                // If player is within range -
                // Shoot at player
                if (!_takeAction)
                {
                    _takeAction = true;

                    // Stop & Fire
                    anim.SetBool("Walk", false);
                    navmeshagent.ResetPath();

                    if (has_Gun)
                    {
                        pistol.Fire();
                    }

                    // Shoot again after a while
                    Invoke("ResetAction", Random.Range(2, 3));
                }
            }
            else
            {
                // If the player is not within range -
                // Chase after the player
                if (!_takeAction)
                {
                    _takeAction = true;
                    _newPos = player.transform.position + Random.insideUnitSphere * 5;

                    // If a walkable spot is found
                    if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
                    {
                        // then walk towards that spot
                        navmeshagent.SetDestination(_hit.position);
                        anim.SetBool("Walk", true);
                    }

                    // Stop-and-Move every few secs
                    Invoke("ResetAction", Random.Range(2, 3));
                }
            }

            yield return null;
        }

        // Restart coroutine
        StartCoroutine(ExecuteState());
    }

    void IdleBehaviour()
    {
        //Idle_Behaviours
    }

    void RandomIdleState()
    {
        float rndAction = Random.value;
        float minDelay = 1;
        float maxDelay = 2;

        if (rndAction > 0.75f)
        {
            // continue to IDLE (play idle-animations here)
            anim.SetBool("Walk", false);
            if (navmeshagent.enabled)
            {
                navmeshagent.ResetPath();
            }
            currAIstate = AIstate.IDLE;
        }
        else if (rndAction < 0.25f)
        {
            _newPos = trans.position + Random.insideUnitSphere * 5;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                // walk towards that spot
                if (navmeshagent.enabled)
                {
                    navmeshagent.SetDestination(_hit.position);
                    navmeshagent.updateRotation = true;
                    anim.SetBool("Walk", true);
                }
                
                currAIstate = AIstate.WALK;
            }
        }
        else if (rndAction >= 0.25f && rndAction < 0.5f)
        {
            // NPC will 'Look right' / 'Turn right'
            navmeshagent.updateRotation = false;
            anim.SetBool("Walk", false);
            navmeshagent.ResetPath();
            currAIstate = AIstate.TURNRIGHT;

            minDelay = 0.5f;
            maxDelay = 1.5f;
        }
        else if (rndAction <= 0.75f && rndAction >= 0.5f)
        {
            // NPC will 'Look left' / 'Turn left'
            navmeshagent.updateRotation = false;
            anim.SetBool("Walk", false);
            navmeshagent.ResetPath();
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
            anim.SetBool("Walk", false);
            navmeshagent.ResetPath();
            currAIstate = AIstate.IDLE;
        }
        else if (rndAction < 0.25f)
        {       
            _newPos = trans.position + Random.insideUnitSphere * 5;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                // walk towards that spot
                navmeshagent.SetDestination(_hit.position);
                anim.SetBool("Walk", true);

                navmeshagent.updateRotation = true;
                currAIstate = AIstate.WALK;
            }
        }
        else if (rndAction >= 0.25f && rndAction < 0.5f)
        {
            // NPC will 'Look right' / 'Turn right'
            navmeshagent.updateRotation = false;
            anim.SetBool("Walk", false);
            navmeshagent.ResetPath();
            currAIstate = AIstate.TURNRIGHT;

            minDelay = 0.5f;
            maxDelay = 1.5f;
        }
        else if (rndAction <= 0.75f && rndAction >= 0.5f)
        {
            // NPC will 'Look left' / 'Turn left'
            navmeshagent.updateRotation = false;
            anim.SetBool("Walk", false);
            navmeshagent.ResetPath();
            currAIstate = AIstate.TURNLEFT;

            minDelay = 0.5f;
            maxDelay = 1.5f;
        }

        // Invoke a new random behaviour
        Invoke("RandomAlertState", Random.Range(minDelay, maxDelay));
    }

    void ResetAction()
    {
        _takeAction = false;
    }

    void AttackAction()
    {
        if (has_Gun)
        {
            // NPC has gun
            Shoot();
        }
        else if (has_Knife)
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
        if (_distance < 6)
        {
            // Punch
            anim.SetBool("Walk", false);
            navmeshagent.ResetPath();

            // Punch again after a while
            Invoke("Melee", Random.Range(1, 2));

            return;
        }
        else
        {
            // Move towards the player
            _newPos = player.transform.position + Random.insideUnitSphere * 3;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                // then walk towards that spot
                navmeshagent.SetDestination(_hit.position);
                anim.SetBool("Walk", true);
            }

            Invoke("Melee", 3);
        }
    }

    void Shoot()
    {
        if (_distance < 50)
        {
            // If player is within range -
            // Shoot at player
            anim.SetBool("Walk", false);
            navmeshagent.ResetPath();
            pistol.Fire();

            // Shoot again after a while
            Invoke("Shoot", Random.Range(2, 3));

            return;
        }
        else
        {
            // If the player is not within range -
            // Chase after the player
            _newPos = player.transform.position + Random.insideUnitSphere * 5;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                // then walk towards that spot
                navmeshagent.SetDestination(_hit.position);
                anim.SetBool("Walk", true);
            }

            Invoke("Shoot", 3);
        }
    }

    void Knife()
    {
        if (_distance < 6)
        {
            // Knife the player
            navmeshagent.ResetPath();
            anim.SetBool("Walk", false);

            // Knife again
            Invoke("Knife", Random.Range(1, 3));

            return;
        }
        else
        {
            // Move towards the player
            _newPos = player.transform.position + Random.insideUnitSphere * 3;

            // If a walkable spot is found
            if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
            {
                // walk towards that spot
                navmeshagent.SetDestination(_hit.position);
                anim.SetBool("Walk", true);
            }

            Invoke("Knife", 3);
        }
    }

    public override void PossessAI()
    {
        navmeshagent.ResetPath();
        navmeshagent.enabled = false;

        // Change AI state to Possessed
        currAIstate = AIstate.POSSESSED;

        CancelInvoke("CheckForPlayer");
        CancelInvoke("Melee");
        CancelInvoke("Knife");
        CancelInvoke("Shoot");
        CancelInvoke("RandomAlertState");
        CancelInvoke("RandomIdleState");
    }

    public override void UnPossessAI()
    {
        navmeshagent.enabled = true;

        // Change AI state to Idle
        currAIstate = AIstate.IDLE;

        InvokeRepeating("CheckForPlayer", 3, 1);
        Invoke("RandomIdleState", 3);
    }
}
