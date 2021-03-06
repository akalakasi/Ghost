﻿using UnityEngine;
using System.Collections.Generic;

public class WeaponBag : MonoBehaviour
{
    public List<Weapon> WeaponsList = new List<Weapon>();
    public Weapon currentWeapon;

    int _selectedWeapon;

    void Start ()
    {
        // Weapons available in the player's weapon bag
        Weapon[] weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon w in weapons)
        {
            WeaponsList.Add(w);
        }

        // Select the first weapon if there is one
        if (WeaponsList.Count > 0)
        {
            currentWeapon = WeaponsList[0];
        }
    }

    public void SwitchWeapon(int _change)
    {
        if (WeaponsList.Count > 0)
        {
            // Turn off current weapon
            currentWeapon.gameObject.SetActive(false);

            if (_change > 0 && _selectedWeapon < WeaponsList.Count)
            {
                // Switch to next weapon
                _selectedWeapon++;
            }
            else if (_change < 0 && _selectedWeapon > 0)
            {
                // Switch to previous weapon
                _selectedWeapon--;
            }

            currentWeapon = WeaponsList[_selectedWeapon];

            // Turn on newly-selected weapon
            currentWeapon.gameObject.SetActive(true);
        }
    }

    public bool Attack()
    {
        if (currentWeapon != null)
        {
            if (currentWeapon.IsFireArm)
            {
                // Shoot
                currentWeapon.Fire();
            }
            else
            {
                // Melee
                currentWeapon.Attack();
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Reload()
    {
        if (currentWeapon != null)
        {
            if (currentWeapon.IsFireArm)
            {
                currentWeapon.Reload();

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    // Returns the animation name of the weapon-attack
    public string WeaponAttackAnimation
    {
        get
        {
            return currentWeapon.attackName;
        }
    }

    // Returns the animation name of the weapon-reload
    public string WeaponReloadAnimation
    {
        get
        {
            return currentWeapon.reloadName;
        }
    }
}
