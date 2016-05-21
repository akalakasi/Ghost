using UnityEngine;

// ALL DAMAGEABLE OBJECTS will have this base-script
public class Stats : MonoBehaviour
{
    public float maxHP;
    public float currHP;

    void Awake()
    {
        currHP = maxHP;
    }

    public void Heal(float _heal)
    {
        currHP += _heal;
        if (currHP > maxHP)
        {
            currHP = maxHP;
        }
    }

	public void ReceiveDamage(float _dmg)
    {
        if (currHP > 0)
        {
            currHP -= _dmg;
        }
    }
}
