using UnityEngine;
using System.Collections;

public enum Civilian_Behaviours { IDLE, TURN_LEFT, TURN_RIGHT, WALK, ALERT, FLEE, DEAD }

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

    CivilianBehaviour[] _currBehaviourPattern;
    int _currBehaviour;
    bool _onFrightened;

    [SerializeField] AudioClip screamClip;
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
            //if (_onFrightened)
            //{
                //if (currState == Civilian_Behaviours.ALERT)
                //{
                //    _onFrightened = false;
                //    navmeshagent.updateRotation = true;

                //    // Stop the current Behaviour Pattern
                //    StopCoroutine("BehaviourPattern");

                //    _currBehaviour = 0;
                //    _currBehaviourPattern = alertBehaviour;

                //    // Restart Behaviour Pattern             
                //    StartCoroutine("BehaviourPattern");
                //}
            //}
        }
        // Spotted player
        else if (_spottedPlayer)
        {
            // Alert the NPC - once the player is seen
            if (!_onFrightened)
            {
                _onFrightened = true;
                _startled = true;

                // AI screams
                _audio.PlayOneShot(screamClip);

                // Stop the current Behaviour Pattern
                StopCoroutine("BehaviourPattern");
                CancelInvoke("ContinueActions");

                navmeshagent.enabled = true;
                navmeshagent.updateRotation = false;

                navmeshagent.ResetPath();
                anim.SetBool("Walk", false);

                _currBehaviour = 0;
                currState = Civilian_Behaviours.ALERT;
            }
        }
    }

    IEnumerator BehaviourPattern()
    {
        if (_currBehaviourPattern.Length > 0)
        {
            // Behaviour follows according to the pattern
            if (_currBehaviour < _currBehaviourPattern.Length)
            {
                currState = _currBehaviourPattern[_currBehaviour].behaviour;
            }
            else
            {
                _currBehaviour = 0;
                currState = _currBehaviourPattern[0].behaviour;
            }

            if (currState == Civilian_Behaviours.WALK)
            {
                _newPos = _currBehaviourPattern[_currBehaviour].targetDestination.position;
            }

            // Reset booleans
            _target = null;
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
        while (currState == Civilian_Behaviours.IDLE)
        {
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

        while (currState == Civilian_Behaviours.TURN_LEFT)
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

        while (currState == Civilian_Behaviours.TURN_RIGHT)
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

        while (currState == Civilian_Behaviours.WALK)
        {
            if (!_executedState)
            {
                _executedState = true;

                // Find a new path
                navmeshagent.speed = 1;
                navmeshagent.SetDestination(_newPos);
                navmeshagent.updateRotation = true;

                // Walking animation
                anim.SetBool("Walk", true);
            }
            else if (_executedState)
            {
                // Checks whether AI has reached its destination
                CheckDestination(0);                
            }

            yield return null;
        }

        while (currState == Civilian_Behaviours.ALERT)
        {
            // If the AI was frightened
            if (_onFrightened)
            {
                // it will turn around to check if the player is still there
                _target = player;

                if (!_executedState)
                {
                    _executedState = true;

                    // Stop moving & go to Alert animation
                    anim.SetBool("Walk", false);
                    navmeshagent.ResetPath();

                    // Seconds Delay before the AI becomes unfrightened
                    Invoke("ReturnToAlert", 3);
                }
                else
                {
                    // Check for the player and run away once it spots the player and the player is nearby
                    if (_spottedPlayer)
                    {
                        if (_distanceToPlayer < 30)
                        {
                            CancelInvoke("ReturnToAlert");
                            _executedState = false;

                            currState = Civilian_Behaviours.FLEE;
                        }
                    }
                }
            }

            yield return null;
        }

        while (currState == Civilian_Behaviours.FLEE)
        {
            // then run away from the player
            if (!_executedState)
            {
                _executedState = true;

                // Find a spot around the area
                _newPos = trans.position + Random.insideUnitSphere * 10;

                // If a walkable spot is found
                if (NavMesh.SamplePosition(_newPos, out _hit, 10, 1))
                {
                    if (navmeshagent.enabled)
                    {
                        // run towards that spot
                        navmeshagent.speed = 3;
                        navmeshagent.SetDestination(_hit.position);
                        navmeshagent.updateRotation = true;

                        // Running animation
                        anim.SetBool("Walk", true);

                        // stop staring at the player
                        _target = null;
                    }
                }
            }
            else if (_executedState)// while running away from the player
            {
                // Checks whether AI has reached its destination
                if (CheckDestination(0))
                {
                    // Go to Alert state
                    currState = Civilian_Behaviours.ALERT;

                    _executedState = false;
                }                
            }

            yield return null;
        }

        while (currState == Civilian_Behaviours.DEAD)
        {
            // Stop staring at the target
            _target = null;

            // Stop AI behaviour
            StopCoroutine("BehaviourPattern");

            // Stop the agent from moving            
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

    void ReturnToAlert()
    {
        if (_onFrightened)
        {
            _target = null;
            _onFrightened = false;
            _startled = false;

            // Update the transform rotation to the agent
            navmeshagent.updateRotation = true;

            // Stop the current Behaviour Pattern
            StopCoroutine("BehaviourPattern");

            _currBehaviour = 0;
            _currBehaviourPattern = alertBehaviour;

            // Restart Behaviour Pattern             
            StartCoroutine("BehaviourPattern");
        }
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
        currState = Civilian_Behaviours.IDLE;

        StartCoroutine("BehaviourPattern");
    }

    public override void HeardNoise(GameObject _noiseSource)
    {
        if (!_onFrightened)
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
        currState = Civilian_Behaviours.DEAD;
    }
}
