using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BasicAI : MonoBehaviour
{
    NavMeshAgent navmeshagent;
    Transform trans;
    GameObject player;

    public GameObject eyes;
    public bool ShowFov;

    public float fieldOfVision;
    public float rangeOfVision;
    public float shootDelay;
    public float hitPoints;
    public float currHitPoints;

    public enum EnemyState
    {
        DEAD,
        POSSESSED,
        NEUTRAL
    }    
    
    public EnemyState currEnemyState;
    private GunHandler _gunHandler;
    PlayerScript thisPlayerScript;

    // Use this for initialization
    void Start ()
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
	}
    void CheckStatus()
    {
        switch (thisPlayerScript.currentPState)
        {
            case PlayerScript.PossesState.Enemy:
                if (currHitPoints <= 0)
                {
                    currHitPoints = 0;
                    currEnemyState = EnemyState.DEAD;
                    Invoke("Death", 1.5f);
                }
                break;
            case PlayerScript.PossesState.Neutral:
                break;
            case PlayerScript.PossesState.Player:
                break;
            case PlayerScript.PossesState.Possessed:
                if(thisPlayerScript.currHP <= 0)
                {
                    thisPlayerScript.currHP = 0;
                    currEnemyState = EnemyState.DEAD;
                    Invoke("SpawnPlayerOnDeath", 0.5f);
                }
                break;
        }
    }
    void SpawnPlayerOnDeath()
    {
        thisPlayerScript.previousBody.transform.position = trans.position;
        thisPlayerScript.previousBody.SetActive(true);
    }
    void Death()
    {
        Destroy(this.gameObject);
    }
    void ShootAtPlayer()
    {
        if (DistanceChecker(rangeOfVision, eyes))
        {
            if (FieldOfVision(fieldOfVision, eyes))
            {
                if (ObstacleChecker(eyes))
                {
                    _gunHandler.Fire();
                }
            }
        }
    }
    void RotateToPlayer()
    {
        if(DistanceChecker(rangeOfVision, eyes))
        {
            //Debug.Log("InRange");
            if(FieldOfVision(fieldOfVision, eyes))
            {
                //Debug.Log("WithinView");
                if (ObstacleChecker(eyes))
                {
                    //Debug.Log("CanSee");
                    navmeshagent.updateRotation = false;
                    Vector3 dir = player.transform.position - trans.position;
                    dir.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(dir);
                    trans.rotation = Quaternion.Slerp(trans.rotation, rotation, 0.2f);

                }
            }
        }
        else
        {
            navmeshagent.updateRotation = true;
        }
       
    }
    bool DistanceChecker(float range, GameObject eyePos)
    {
        bool isNear = false;
        //find which direction is the player from this AI
        Vector3 heading = player.transform.position - eyePos.transform.position;

        //find the square of the distance between the player and AI
        float sqrDist = heading.magnitude;

        //check if the distance is within the range
        if (sqrDist < range * range)
        {
            isNear = true;
        }
        else
        {
            isNear = false;
        }
        return isNear;
    }
    void FieldOfVisionDisplay(float fov, GameObject eyePos, float range)
    {
        Vector3 targetDir = player.transform.position - eyePos.transform.position;
        Vector3 forward = trans.forward;
        Vector3 leftRayRotation = Quaternion.AngleAxis(-fov*0.5f, trans.up) * trans.forward;
        Vector3 rightRayRotation = Quaternion.AngleAxis(fov*0.5f, trans.up) * trans.forward;

        Debug.DrawRay(trans.position, forward.normalized * range, Color.red);
        Debug.DrawRay(trans.position, leftRayRotation.normalized * range, Color.red);
        Debug.DrawRay(trans.position, rightRayRotation.normalized * range, Color.red);


    }
	bool FieldOfVision(float fov, GameObject eyePos)
    {
        bool withinFov;

        //find which direction is the player from this AI
        Vector3 targetDir = player.transform.position - eyePos.transform.position;
        Vector3 forward = trans.forward;

        //check against where the ai is facing and what the angle is between that and the player's position
        float angle = Vector3.Angle(targetDir, forward);

        //is that angle within the field of vision? "fov"
        if (angle < fov * 0.5f)
        {
            withinFov = true;
        }
        else
        {
            withinFov = false;
        }
        return withinFov;
    }
    bool ObstacleChecker(GameObject eyePos)
    {
        bool canSee = false;
        Ray ray = new Ray(eyePos.transform.position, player.transform.position);
        
        Vector3 targetDir = (player.transform.position + Vector3.up*0.5f) - eyePos.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(eyePos.transform.position, targetDir.normalized, out hit, rangeOfVision))
        {
            //Debug.DrawRay(transform.position, targetDir.normalized, Color.green);
            PlayerScript playerScript = hit.transform.gameObject.GetComponent<PlayerScript>();
            if (playerScript != null)
            {
                switch (playerScript.currentPState)
                {
                    case PlayerScript.PossesState.Enemy:
                        //Do nothing
                        canSee = false;
                        break;
                    case PlayerScript.PossesState.Neutral:
                        //Do nothing
                        canSee = false;
                        break;
                    case PlayerScript.PossesState.Player:
                        // OH LOOK, IT'S THE PLAYER, SHOOT HIM!
                        canSee = true;
                        break;
                    case PlayerScript.PossesState.Possessed:
                        // FUCK ONE OF OUR GUYS IS POSSESSED, SHOOT!
                        canSee = true;
                        break;
                }
            }
        }
        return canSee;
    }
	// Update is called once per frame
	void LateUpdate () {

        //For Game Designers :)
        if (ShowFov)
        {
            FieldOfVisionDisplay(fieldOfVision, eyes, rangeOfVision);
        }
        

        //Well, rotate this to face the player.
        if(currEnemyState != EnemyState.DEAD)
        {
            RotateToPlayer();
        }
    }
}
