using UnityEngine;

// ALL weapons will have this base-script
public class Weapon : MonoBehaviour
{   
    protected Transform _trans;
    protected AudioSource _audio;

    [Header("Weapon Stats", order = 0)]
    [Space(5, order = 1)]
    [SerializeField] protected float weaponDamage;
    [SerializeField] protected float noiseLevel;

    [Space(10, order = 0)]
    [Header("Weapon Animation", order = 1)]
    [Space(5, order = 2)]
    public string attackName;
    public string reloadName;

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
