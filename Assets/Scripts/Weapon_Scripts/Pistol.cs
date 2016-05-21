using UnityEngine;

public class Pistol : Firearm
{
    public float bulletSpeed;
    public float spreadMagnitude;

    void Start()
    {
        _trans = transform;
        _audio = GetComponent<AudioSource>();
    }

    public override void Fire()
    {
        // Make sure there's ammo
        if (Current_Ammo != 0)
        {
            Current_Ammo--;
            FireAudio();

            RaycastHit hit;
            if (Physics.Raycast(firingPos.transform.position, firingPos.forward * FiringRange, out hit))
            {
                //Collider target = hit.collider; // What did I hit?
                //float distance = hit.distance; // How far out?
                //Vector3 location = hit.point; // Where did I make impact?
                GameObject targetGameObject = hit.collider.gameObject; // What's the GameObject?
                Stats hitObject = targetGameObject.GetComponent<Stats>();
                
                if (hitObject != null)
                {
                    hitObject.ReceiveDamage(weaponDamage);
                }
            }
        }
    }
}
