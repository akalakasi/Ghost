using UnityEngine;
using System.Collections;

public class Gangster : BasicAI
{
    GunHandler _gunHandler;
    PlayerScript thisPlayerScript;

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

        InvokeRepeating("CheckStatus", 0, 1.0f);
        InvokeRepeating("ShootAtPlayer", 0, shootDelay);

        StartCoroutine(ExecuteAction());
    }

    IEnumerator ExecuteAction()
    {
        while (currAIstate == AIstate.IDLE)
        {
            // Stand still
            if (!_newAction)
            {
                _newAction = true;

                float rndAction = Random.value;

                if (rndAction > 0.5f)
                {
                    print("left");
                }
                else
                {
                    print("right");
                }
            }

            yield return null;
        }
        
        while (currAIstate == AIstate.WALK)
        {
            yield return null;
        }

        while (currAIstate == AIstate.AGGRESSIVE)
        {
            // Stare at player
            yield return null;
        }

        // Restart coroutine
        StartCoroutine(ExecuteAction());
    }
}
