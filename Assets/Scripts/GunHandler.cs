using UnityEngine;
using System.Collections;

public class GunHandler : MonoBehaviour {

    public GameObject GunPos;
    public GameObject GunSoundPrefab;
    public AudioSource GunAudio;
    public AudioClip GunShotSound;
    public float bulletSpeed;
    public float gunDamage;
    public float spreadMagnitude;
    public bool hasGun;
	// Use this for initialization
	void Start () {
	
	}
    public void Fire()
    {
        if (hasGun)
        {
            AudioHandler();
            RaycastHit hit;
            if (Physics.Raycast(GunPos.transform.position, transform.forward, out hit))
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
                        case PlayerScript.PossesState.Neutral:
                            if (_hitBasicAI != null)
                            {
                                _hitBasicAI.currHitPoints -= gunDamage;
                            }
                            break;
                        case PlayerScript.PossesState.Enemy:
                            if (_hitBasicAI != null)
                            {
                                _hitBasicAI.currHitPoints -= gunDamage;
                            }
                            break;
                        case PlayerScript.PossesState.Player:
                            _hitPlayerScript.currHP -= gunDamage;
                            break;
                        case PlayerScript.PossesState.Possessed:
                            if (_hitBasicAI != null)
                            {
                                _hitBasicAI.currHitPoints -= gunDamage;
                            }
                            break;
                    }
                }
                //Debug.Log(targetGameObject);
            }
        }
    }
	void AudioHandler()
    {
        Instantiate(GunSoundPrefab, GunPos.transform.position, Quaternion.identity);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
