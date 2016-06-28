using UnityEngine;
using System.Collections;

public enum Thug_Behaviours { IDLE, TURN_LEFT, TURN_RIGHT, PATROL, SEARCH_FOR_PLAYER, CHASE_PLAYER, SHOOT, MELEE, DEAD }

[System.Serializable]
public struct ThugBehaviour
{
    [Space(5, order = 0)]
    [Header("Choice Of Behaviour", order = 1)]
    [Space(5, order = 2)]
    public string name;
    public Thug_Behaviours behaviour;
    public float duration;

    [Space(10, order = 0)]
    [Header("Movement Path", order = 1)]
    [Space(5, order = 2)]
    public Transform targetDestination;
}

public class Thug : AI
{
    [SerializeField] Weapon pistol;
    [SerializeField] bool rangeWeapon;
    [SerializeField] bool meleeWeapon;
    [SerializeField] float rangeAttackDelay;
    [SerializeField] float meleeAttackDelay;

    [Space(10, order = 0)]
    [Header("Thug Behaviour", order = 1)]
    [Space(5, order = 2)]
    [SerializeField] Thug_Behaviours currState;
    [SerializeField] ThugBehaviour[] normalBehaviour;
    [SerializeField] ThugBehaviour[] alertBehaviour;
    [SerializeField] ThugBehaviour[] attackBehaviour;
    [SerializeField] bool randomizeBehaviour;

    ThugBehaviour[] _currBehaviourPattern;
    int _currBehaviour;
    bool _onOffensive;
    bool _isAttacking;

    [SerializeField] AudioClip deathClip;

    void Start ()
    {
        // Basic AI-setup
        Setup();

        _currBehaviourPattern = normalBehaviour;

        // Activate behaviours
        StartCoroutine("BehaviourPattern");
        StartCoroutine("ExecuteStates");      
        InvokeRepeating("CheckForPlayer", 1, 1);
    }
    
    void CheckForPlayer()
    {
        // Cannot see player
        if (!_spottedPlayer)
        {
            // If the AI was on the Offensive, it will become Alert
            // instead of returning to Normal behaviour
            if (_onOffensive)
            {
                _onOffensive = false;
                _startled = false;
                navmeshagent.updateRotation = true;

                // Stop the current Behaviour Pattern
                StopCoroutine("BehaviourPattern");

                _currBehaviour = 0;
                _currBehaviourPattern = alertBehaviour;

                // Restart Behaviour Pattern             
                StartCoroutine("BehaviourPattern");
            }
        }
        // Spotted player
        else if (_spottedPlayer)
        {
            // Alert the NPC - once the player is seen
            if (!_onOffensive)
            {
                _onOffensive = true;
                _startled = true;

                // Stop the current Behaviour Pattern
                StopCoroutine("BehaviourPattern");
                CancelInvoke("ContinueActions");

                navmeshagent.enabled = true;
                navmeshagent.updateRotation = false;
                navmeshagent.ResetPath();

                _currBehaviour = 0;
                _currBehaviourPattern = attackBehaviour;

                // Change to Offensive Behaviour
                StartCoroutine("BehaviourPattern");
            }
        }
    }

    IEnumerator BehaviourPattern()
    {
        if (_currBehaviourPattern.Length > 0)
        {
            // Behaviour follows according to the pattern
            if (currState != Thug_Behaviours.CHASE_PLAYER)
            {
                if (_currBehaviour < _currBehaviourPattern.Length)
                {
                    currState = _currBehaviourPattern[_currBehaviour].behaviour;
                }
                else
                {
                    // In case the current Behaviour doesn't follow the Behaviour pattern
                    // reset to 0
                    _currBehaviour = 0;
                    currState = _currBehaviourPattern[0].behaviour;
                }
            }

            if (currState == Thug_Behaviours.PATROL)
            {
                // Assign the destination for the AI if it is patrolling
                _newPos = _currBehaviourPattern[_currBehaviour].targetDestination.position;
            }

            // Reset booleans
            _target = null;
            _isAttacking = false;
            _executedState = false;

            // How long does the Behaviour last?
            yield return new WaitForSeconds(_currBehaviourPattern[_currBehaviour].duration);

            if (!randomizeBehaviour)
            {
                if (_currBehaviour < _currBehaviourPattern.Length - 1)
                {
                    // Proceed to the next Behaviour
                    _currBehaviour++;
                }
                else
                {
                    // Return back to the first Behaviour
                    _currBehaviour = 0;
                }
            }
            else
            {
                // Randomize the Behaviour based on behaviours set
                _currBehaviour = Random.Range(0, _currBehaviourPattern.Length);
            }            

            // Start the next Behaviour
            StartCoroutine("BehaviourPattern");
        }
    }

    IEnumerator ExecuteStates()
    {
        while (currState == Thug_Behaviours.IDLE)
        {
            // Stop the navmeshagent from moving
            if (!_executedState)
            {
                _executedState = true;

                // Idle animation
                if (navmeshagent.hasPath)
                {
                    navmeshagent.ResetPath();
                    anim.SetBool("Walk", false);
                }
            }

            yield return null;
        }

        while (currState == Thug_Behaviours.TURN_LEFT)
        {
            if (!_executedState)
            {
                _executedState = true;

                // Idle animation
                navmeshagent.ResetPath();
                anim.SetBool("Walk", false);
            }

            trans.Rotate(-Vector3.up * 20 * Time.deltaTime);

            yield return null;
        }

        while (currState == Thug_Behaviours.TURN_RIGHT)
        {
            if (!_executedState)
            {
                _executedState = true;

                // Idle animation
                navmeshagent.ResetPath();
                anim.SetBool("Walk", false);
            }

            trans.Rotate(Vector3.up * 20 * Time.deltaTime);

            yield return null;
        }

        while (currState == Thug_Behaviours.PATROL)
        {
            if (!_executedState)
            {
                _executedState = true;

                // Patrol towards the next destination
                navmeshagent.speed = 1;
                navmeshagent.SetDestination(_newPos);
                navmeshagent.updateRotation = true;

                // Walking animation
                anim.SetBool("Walk", true);
            }

            // Checks whether AI has reached its destination
            CheckDestination(0);

            yield return null;
        }

        // Search for the player
        while (currState == Thug_Behaviours.SEARCH_FOR_PLAYER)
        {
            // keep searching for the player
            if (!_spottedPlayer)
            {
                if (!_executedState)
                {
                    _executedState = true;

                    // Find a spot around the area
                    _newPos = trans.position + Random.insideUnitSphere * 30;

                    // If a walkable spot is found
                    if (NavMesh.SamplePosition(_newPos, out _hit, 50, NavMesh.AllAreas))
                    {
                        if (navmeshagent.enabled)
                        {
                            // walk towards that spot
                            navmeshagent.speed = 3;
                            navmeshagent.SetDestination(_hit.position);
                            navmeshagent.updateRotation = true;

                            // Walking animation
                            anim.SetBool("Walk", true);
                        }
                    }
                }
                else if (_executedState) // After finding (or not) a destination
                {
                    // Checks whether AI has reached its destination
                    CheckDestination(0);                    
                }
            }

            yield return null;
        }

        // Chase after the player
        while (currState == Thug_Behaviours.CHASE_PLAYER)
        {
            if (!_executedState)
            {
                _executedState = true;

                // Run towards the player
                navmeshagent.speed = 3;
                navmeshagent.SetDestination(player.transform.position);
                navmeshagent.updateRotation = true;

                // Running animation
                anim.SetBool("Walk", true);
            }

            // Stop the AI once it has reached its destination - only when it has spotted the player
            if (_spottedPlayer)
            {
                // Stare at the player
                _target = player;

                // Using a range weapon
                if (rangeWeapon)
                {                   
                    // Checks whether AI has reached its destination
                    if (CheckDestination(30))
                    {
                        // Go to SHOOT state
                        currState = Thug_Behaviours.SHOOT;
                    }
                }
                // Using a melee weapon
                else if (meleeWeapon)
                {
                    // Checks whether AI has reached its destination
                    if (CheckDestination(0))
                    {
                        // Go to SHOOT state
                        currState = Thug_Behaviours.SHOOT;
                    }
                }
            }

            yield return null;
        }

        // Melee the player
        while (currState == Thug_Behaviours.MELEE)
        {
            // Stop AI from moving
            if (!_executedState)
            {
                _executedState = true;

                anim.SetBool("Walk", false);
                navmeshagent.ResetPath();
            }


            // Within Aim
            if (Vector3.Angle(trans.forward, _angleViewToPlayer) < 30)
            {
                // Melee Attack
                if (!_isAttacking)
                {
                    _isAttacking = true;

                    // Stop staring at the player
                    _target = null;
                    // Melee Attack
                    Melee();
                }
            }
            else // Not within aim
            {
                if (!_isAttacking)
                {
                    // Stare at player to aim
                    _target = player;
                }
            }

            yield return null;
        }

        // Shoot at playera
        while (currState == Thug_Behaviours.SHOOT)
        {            
            // Within distance
            if (_distanceToPlayer < 50)
            {
                // Stop AI from moving
                if (!_executedState)
                {
                    _executedState = true;

                    // Make sure the AI stops moving to shoot
                    anim.SetBool("Walk", false);
                    navmeshagent.ResetPath();
                }

                // Within Aim
                if (Vector3.Angle(trans.forward, _angleViewToPlayer) < 10)
                {              
                    // Fire pistol
                    if (!_isAttacking)
                    {
                        _isAttacking = true;

                        // Stop staring to aim                   
                        _target = null;

                        // Fire after a delay
                        Invoke("Fire", rangeAttackDelay);
                    }
                }
                else // Not within aim
                {
                    if (!_isAttacking)
                    {
                        // Stare at player to aim
                        _target = player;
                    }
                }
            }
            else if (_distanceToPlayer >= 50) // Outta distance
            {
                // Chase after the player if they are not close enough
                currState = Thug_Behaviours.CHASE_PLAYER;
            }

            yield return null;
        }

        while (currState == Thug_Behaviours.DEAD)
        {
            // Stop staring at the target
            _target = null;

            // Stop AI behaviour
            StopCoroutine("BehaviourPattern");

            // Stop moving
            navmeshagent.ResetPath();
            anim.SetBool("Walk", false);

            // Death animation
            //anim.SetTrigger("Death");  

            // Death-voice
            _audio.PlayOneShot(deathClip);

            yield return new WaitForSeconds(3);

            Destroy(gameObject);
            yield break;          
        }

        StartCoroutine("ExecuteStates");
    }

    void Melee()
    {
        // Melee attack code
        // anim.SetTrigger("Melee");

        currState = Thug_Behaviours.IDLE;
    }

    void Fire()
    {        
        pistol.Fire();

        // Fire - animation
        // anim.SetTrigger("Fire");
        // anim.SetTrigger("Reload");

        currState = Thug_Behaviours.IDLE;
    }
    
    public override void HaltActions()
    {
        base.HaltActions();

        StopCoroutine("BehaviourPattern");
    }

    public override void ContinueActions()
    {
        base.ContinueActions();

        // Go to Idle State
        currState = Thug_Behaviours.IDLE;

        StartCoroutine("BehaviourPattern");
    }

    public override void HeardNoise(GameObject _noiseSource)
    {
        if (!_onOffensive)
        {
            base.HeardNoise(_noiseSource);
        }
    }

    public override void PossessAI()
    {
        base.PossessAI();
    }

    public override void UnPossessAI()
    {
        base.UnPossessAI();
    }

    public override void Dead()
    {
        currState = Thug_Behaviours.DEAD;
    }
}
