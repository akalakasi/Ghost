using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public class PlayerScript : MonoBehaviour {
    public enum PossesState
    {
        Enemy,
        Possessed,
        Player,
        Neutral,
        Dead
    }
    public GameObject previousBody;
    public float HP;
    public float currHP;
    public PossesState currentPState;
    private GunHandler _gunHandler;
	// Use this for initialization
	void Start () {
        _gunHandler = GetComponent<GunHandler>();
        currHP = HP;
        InvokeRepeating("CheckForDeath", 1.0f, 1.0f);
    }
    void CheckForDeath()
    {
        if(currHP <= 0)
        {
            currHP = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
    
    void Possession()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit))
        {

            //Collider target = hit.collider; // What did I hit?
            //float distance = hit.distance; // How far out?
            //Vector3 location = hit.point; // Where did I make impact?
            GameObject targetGameObject = hit.collider.gameObject; // What's the GameObject?
            Debug.Log(targetGameObject);
            PlayerScript _hitPlayerScript = hit.collider.gameObject.GetComponent<PlayerScript>();
            BasicAI _hitBasicAI = hit.collider.gameObject.GetComponent<BasicAI>();

            if (_hitPlayerScript != null)
            {
                switch (_hitPlayerScript.currentPState)
                {
                    case PossesState.Enemy:
                        _hitPlayerScript.previousBody = this.gameObject;
                        _hitPlayerScript.GetComponent<CharacterController>().enabled = true;
                        _hitPlayerScript.GetComponent<AudioSource>().enabled = true;
                        _hitPlayerScript.GetComponent<FirstPersonController>().enabled = true;
                        _hitPlayerScript.GetComponent<BasicAI>().enabled = false;
                        _hitPlayerScript.currentPState = PossesState.Possessed;
                        _hitPlayerScript.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        _hitPlayerScript.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        transform.position = Vector3.Slerp(transform.position, _hitPlayerScript.gameObject.transform.position, 0.1f);
                        transform.gameObject.SetActive(false);
                        break;
                    case PossesState.Neutral:
                        break;
                }
            }

        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (currentPState)
        {
            case PossesState.Player:
                if (Input.GetMouseButtonDown(0))
                {
                    _gunHandler.Fire();
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Possession();
                }
                break;
            case PossesState.Possessed:
                if (Input.GetMouseButtonDown(0))
                {
                    _gunHandler.Fire();
                }
                break;
        }
	}
}
