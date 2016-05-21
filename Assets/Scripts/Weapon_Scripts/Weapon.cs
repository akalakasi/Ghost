using UnityEngine;

// ALL weapons will have this base-script
public class Weapon : MonoBehaviour
{   
    protected Transform _trans;
    protected AudioSource _audio;

    public float weaponDamage;

    public virtual void Attack() { }

    public virtual void Fire() { }

    public virtual void Reload() { }

    public virtual bool IsFireArm
    {
        get { return false; }
    }

    public virtual int WeaponCurrentAmmo()
    {
        return 0;
    }
    public virtual int WeaponTotalAmmo()
    {
        return 0;
    }
}
