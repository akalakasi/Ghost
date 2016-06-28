using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public class PlayerScript : Stats
{
    Transform _trans;
    Animation _anim;

    public GameObject possessedBody;
    public WeaponBag weapons;
    public Transform weaponBagPos;

    public enum PossesState
    {
        NOT_POSSESSING,
        POSSESSING,
        DEAD
    }

    public PossesState currentPState;

    void Start ()
    {
        _trans = transform;
        _anim = GetComponentInChildren<Animation>();

        // Disable Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine("CheckStatus");
    }

    IEnumerator CheckStatus()
    {
        // while the player is not dead
        while (currentPState != PossesState.DEAD)
        {
            // Make sure player doesn't gain control even while the game is paused. (If you can find another way to script this part, it will be great.)
            if (!PauseMenu.pause)
            {
                if (weapons != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {                        
                        if (weapons.Attack())
                        {
                            _anim.Play(weapons.WeaponAttackAnimation);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        // Reload Firearm
                        if (weapons.Reload())
                        {
                            _anim.Play(weapons.WeaponReloadAnimation);
                        }
                    }

                    // Switch Weapons
                    if (Input.GetAxis("Mouse ScrollWheel") > 0)
                    {
                        weapons.SwitchWeapon(1);
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                    {
                        weapons.SwitchWeapon(-1);
                    }
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Possession();
                }

                // Dead
                if (currHP <= 0)
                {
                    currHP = 0;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }

            yield return null;
        }
    }

    void Possession()
    {
        RaycastHit hit;
        if (Physics.Raycast(_trans.position + Vector3.up * 0.5f, _trans.forward, out hit))
        {

            //Collider target = hit.collider; // What did I hit?
            //float distance = hit.distance; // How far out?
            //Vector3 location = hit.point; // Where did I make impact?

            // What's the GameObject?
            GameObject targetGameObject = hit.collider.gameObject;
            //Debug.Log(targetGameObject);
           
            if (!targetGameObject.CompareTag("AI"))
            {
                return;
            }
            else if (targetGameObject.CompareTag("AI"))
            {        
                AI _ai = targetGameObject.GetComponent<AI>();

                if (_ai != null)
                {
                    // Check if it is the current AI you're possessing
                    if (possessedBody != _ai.gameObject)
                    {
                        // Leave your last possessed body and enter a new body
                        if (possessedBody)
                        {                                                                                        
                            possessedBody.SetActive(true);
                            possessedBody.transform.SetParent(null);

                            // Give Back old weapons to previous body        
                            AI _previousAI = possessedBody.GetComponent<AI>();
                            weapons.transform.SetParent(_previousAI.weaponBagPos);
                            weapons.transform.position = _previousAI.weaponBagPos.position;
                            weapons.transform.rotation = weapons.transform.parent.rotation;
                            _previousAI.UnPossessAI();
                        }
                        possessedBody = _ai.gameObject;

                        // AI becomes possessed                
                        _ai.PossessAI();

                        // Player possesses AI
                        currentPState = PossesState.POSSESSING;

                        // Go to AI's position                        
                        _trans.position = possessedBody.transform.position;
                        _trans.rotation = possessedBody.transform.rotation;    
                        _ai.transform.SetParent(_trans);
                        _ai.gameObject.SetActive(false);                            

                        // Acquire all of the AI's weapons   
                        weapons = _ai.weapons;
                        weapons.transform.SetParent(weaponBagPos);
                        weapons.transform.position = weaponBagPos.position;
                        weapons.transform.rotation = weapons.transform.parent.rotation;

                        // Make sure that the AIs do not know you're the player
                        tag = "Untagged";
                    }
                }
            }                                     
        }
    }
}