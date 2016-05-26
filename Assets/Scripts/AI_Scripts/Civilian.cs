using UnityEngine;
using System.Collections;

public enum Civilian_Behaviours { IDLE, TURN_LEFT, TURN_RIGHT, WALK, RUN_FROM_PLAYER, DEAD }

[System.Serializable]
public struct CivilianBehaviour
{
    [Space(5, order = 0)]
    [Header("Choice Of Behaviour", order = 1)]
    [Space(5, order = 2)]
    public string name;
    public Civilian_Behaviours behaviour;
    public float duration;

    [Space(10, order = 0)]
    [Header("Movement Path", order = 1)]
    [Space(5, order = 2)]
    public Transform targetDestination;
}

public class Civilian : AI
{
    [Space(10, order = 0)]
    [Header("Civilian Behaviour", order = 1)]
    [Space(5, order = 2)]
    [SerializeField] Civilian_Behaviours currState;
    [SerializeField] CivilianBehaviour[] normalBehaviour;
    [SerializeField] CivilianBehaviour[] alertBehaviour;
    [SerializeField] bool randomizeBehaviour;

    int _currBehaviour;
    bool _onAlert;
    bool _runFromPlayer;

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
        }
    }

    IEnumerator BehaviourPattern()
    {
        CivilianBehaviour[] _behaviourPattern;

        if (!_onAlert)
        {
            _behaviourPattern = normalBehaviour;
        }
        else
        {
            _behaviourPattern = alertBehaviour;
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

            if (currState == Civilian_Behaviours.WALK)
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

            // Start the next Behaviour
            StartCoroutine(BehaviourPattern());
        }
    }

    IEnumerator ExecuteStates()
    {
        while (currState == Civilian_Behaviours.IDLE)
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

        while (currState == Civilian_Behaviours.TURN_LEFT)
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

        while (currState == Civilian_Behaviours.TURN_RIGHT)
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

        while (currState == Civilian_Behaviours.WALK)
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

                currState = Civilian_Behaviours.IDLE;
            }

            yield return null;
        }

        while (currState == Civilian_Behaviours.WALK)
        {
            // Walk to a choosen spot
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

                currState = Civilian_Behaviours.IDLE;
            }

            yield return null;
        }

        while (currState == Civilian_Behaviours.RUN_FROM_PLAYER)
        {
            // If the player is within sight
            if (_spottedPlayer)
            {
                // If the player is within distance
                if (_distance < 20)
                {
                    // then run away from the player
                    if (!_runFromPlayer)
                    {
                        _runFromPlayer = true;

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
                    else
                    {
                        // while running away from the player
                        // - Stop the AI once it has reached its destination
                        if (Vector3.Distance(_newPos, trans.position) < 0.2f)
                        {
                            if (navmeshagent.hasPath)
                            {
                                navmeshagent.ResetPath();
                                anim.SetBool("Walk", false);
                            }

                            currState = Civilian_Behaviours.IDLE;
                        }
                    }
                }
            }

            yield return null;
        }

        while (currState == Civilian_Behaviours.DEAD)
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
