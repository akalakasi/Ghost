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

    int _currBehaviour;
    bool _onAlert;
    bool _isAttacking;

    void Start ()
    {
        // Basic AI-setup
        Setup();

        // Activate behaviours
        StartCoroutine("BehaviourPattern");
        StartCoroutine("ExecuteStates");      
        InvokeRepeating("CheckForPlayer", 1, 1);
    }
    
    void CheckForPlayer()
    {
        //if (!_spottedPlayer)
        //{
        //    if (_isOffensive)
        //    {
        //        _isOffensive = false;

        //        _currBehaviour = 0;
        //        navmeshagent.updateRotation = true;

        //        // Change to Regular Behaviour
        //        StopCoroutine("BehaviourPattern");
        //        StartCoroutine("BehaviourPattern");
        //    }
        //}
        if (_spottedPlayer)
        {
            // Alert the NPC - once the player is seen
            if (!_onAlert)
            {
                _onAlert = true;

                _currBehaviour = 0;
                navmeshagent.updateRotation = false;
                navmeshagent.ResetPath();

                // Change to Offensive Behaviour
                StopCoroutine("BehaviourPattern");
                StartCoroutine("BehaviourPattern");
            }
            else if (_onAlert)
            {
                _onAlert = false;
            }
        }
    }

    IEnumerator BehaviourPattern()
    {
        ThugBehaviour[] _behaviourPattern;

        if (!_spottedPlayer)
        {
            if (!_onAlert)
            {
                _behaviourPattern = normalBehaviour;
            }
            else
            {
                _behaviourPattern = alertBehaviour;
            }
        }
        else
        {
            _behaviourPattern = attackBehaviour;
        }

        if (_behaviourPattern.Length > 0)
        {
            // Behaviour follows according to the pattern
            if (_currBehaviour < _behaviourPattern.Length)
            {
                currState = _behaviourPattern[_currBehaviour].behaviour;
            }
            else
            {
                _currBehaviour = 0;
                currState = _behaviourPattern[0].behaviour;
            }

            if (currState == Thug_Behaviours.PATROL)
            {
                _newPos = _behaviourPattern[_currBehaviour].targetDestination.position;
            }

            // How long does the Behaviour last?
            yield return new WaitForSeconds(_behaviourPattern[_currBehaviour].duration);

            if (!randomizeBehaviour)
            {
                if (_currBehaviour < _behaviourPattern.Length - 1)
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
                _currBehaviour = Random.Range(0, _behaviourPattern.Length);
            }

            // Stop staring at the player
            _stareAtPlayer = false;
            _isAttacking = false;

            // Start the next Behaviour
            StartCoroutine(BehaviourPattern());
        }
    }

    IEnumerator ExecuteStates()
    {
        while (currState == Thug_Behaviours.IDLE)
        {
            // Stop the navmeshagent from moving
            if (navmeshagent.hasPath)
            {
                // Idle animation
                anim.SetBool("Walk", false);
                navmeshagent.ResetPath();
            }

            yield return null;
        }

        while (currState == Thug_Behaviours.TURN_LEFT)
        {
            if (navmeshagent.hasPath)
            {
                // Stop the AI from moving
                navmeshagent.updateRotation = false;
                navmeshagent.ResetPath();
                anim.SetBool("Walk", false);
            }

            trans.Rotate(-Vector3.up * 20 * Time.deltaTime);

            yield return null;
        }

        while (currState == Thug_Behaviours.TURN_RIGHT)
        {
            if (navmeshagent.hasPath)
            {
                // Stop the AI from moving
                navmeshagent.updateRotation = false;
                navmeshagent.ResetPath();
                anim.SetBool("Walk", false);
            }

            trans.Rotate(Vector3.up * 20 * Time.deltaTime);

            yield return null;
        }

        while (currState == Thug_Behaviours.PATROL)
        {
            navmeshagent.speed = 1;
            navmeshagent.SetDestination(_newPos);
            navmeshagent.updateRotation = true;

            // Walking animation
            anim.SetBool("Walk", true);

            // Stop the AI once it has reached its destination
            if (Vector3.Distance(_newPos, trans.position) < 0.2f)
            {
                if (navmeshagent.hasPath)
                {
                    navmeshagent.ResetPath();
                    anim.SetBool("Walk", false);
                }

                currState = Thug_Behaviours.IDLE;
            }

            yield return null;
        }

        // Search for the player
        while (currState == Thug_Behaviours.SEARCH_FOR_PLAYER)
        {
            // keep searching until AI spots the player
            if (!_spottedPlayer)
            {
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

                // Stop the AI once it has reached its destination
                if (Vector3.Distance(_newPos, trans.position) < 0.2f)
                {
                    if (navmeshagent.hasPath)
                    {
                        navmeshagent.ResetPath();
                        anim.SetBool("Walk", false);
                    }

                    currState = Thug_Behaviours.IDLE;
                }
            }

            yield return null;
        }

        // Chase after the player
        while (currState == Thug_Behaviours.CHASE_PLAYER)
        {
            if (navmeshagent.enabled)
            {
                // Run towards the player
                navmeshagent.speed = 3;
                navmeshagent.SetDestination(player.transform.position);
                navmeshagent.updateRotation = true;

                // Walking animation
                anim.SetBool("Walk", true);
            }

            // Stop the AI once it has reached its destination - only when it has spotted the player
            if (_spottedPlayer)
            {
                _stareAtPlayer = true;

                // Using a range weapon
                if (rangeWeapon)
                {
                    if (Vector3.Distance(_newPos, trans.position) < 20)
                    {
                        if (navmeshagent.hasPath)
                        {
                            navmeshagent.ResetPath();
                            anim.SetBool("Walk", false);
                        }

                        currState = Thug_Behaviours.IDLE;
                    }
                }
                // Using a melee weapon
                else if (meleeWeapon)
                {
                    if (Vector3.Distance(_newPos, trans.position) < 4)
                    {
                        if (navmeshagent.hasPath)
                        {
                            navmeshagent.ResetPath();
                            anim.SetBool("Walk", false);
                        }

                        currState = Thug_Behaviours.IDLE;
                    }
                }
            }

            yield return null;
        }

        // Melee the player
        while (currState == Thug_Behaviours.MELEE)
        {
            // Stop AI from moving
            anim.SetBool("Walk", false);
            navmeshagent.ResetPath();

            // Within Aim
            if (Vector3.Angle(trans.forward, _sightRadius) < 5)
            {
                // Melee Attack
                if (!_isAttacking)
                {
                    _isAttacking = true;

                    // Stop staring at the player
                    _stareAtPlayer = false;

                    // Melee Attack
                    Melee();
                }
            }
            else
            {
                if (!_isAttacking)
                {
                    // Stare at player to aim
                    _stareAtPlayer = true;
                }
            }

            yield return null;
        }

        // Shoot at playera
        while (currState == Thug_Behaviours.SHOOT)
        {
            // Stop AI from moving
            anim.SetBool("Walk", false);
            navmeshagent.ResetPath();

            // Within Aim
            if (Vector3.Angle(trans.forward, _sightRadius) < 5)
            {
                // Within distance
                if (_distance < 50)
                {                    
                    // Fire pistol
                    if (!_isAttacking)
                    {
                        _isAttacking = true;

                        // Stop staring to aim
                        _stareAtPlayer = false;                        

                        // Fire after a delay
                        Invoke("Fire", rangeAttackDelay);                        
                    }                    
                }
            }
            else
            {
                if (!_isAttacking)
                {
                    // Stare at player to aim
                    _stareAtPlayer = true;
                }
            }

            yield return null;
        }

        while (currState == Thug_Behaviours.DEAD)
        {
            // Stop AI behaviour
            StopCoroutine("BehaviourPattern");

            // Stop the agent from moving
            if (navmeshagent.hasPath)
            {
                anim.SetBool("Walk", false);
                navmeshagent.ResetPath();
            }

            // Death animation
            //anim.SetTrigger("Death");  

            yield break;          
        }

        StartCoroutine("ExecuteStates");
    }

    void Melee()
    {
        currState = Thug_Behaviours.MELEE;

        // Melee attack code
        // anim.SetTrigger("Melee");
    }

    void Fire()
    {
        currState = Thug_Behaviours.IDLE;

        pistol.Fire();

        // Fire - animation
        // anim.SetTrigger("Fire");
        // anim.SetTrigger("Reload");
    }

    public override void PossessAI()
    {
        if (!_possessed)
        {
            _possessed = true;
            navmeshagent.ResetPath();
            navmeshagent.enabled = false;

            // Stop all actions
            StopCoroutine("ExecuteStates");
            StopCoroutine("BehaviourPattern");

            CancelInvoke("CheckForPlayer");
        }
    }

    public override void UnPossessAI()
    {
        if (_possessed)
        {
            _possessed = false;
            navmeshagent.enabled = true;

            // Continue actions
            StartCoroutine("BehaviourPattern");
            StartCoroutine("ExecuteStates");

            InvokeRepeating("CheckForPlayer", 3, 1);
        }
    }
}
