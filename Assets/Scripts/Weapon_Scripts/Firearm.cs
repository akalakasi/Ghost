﻿using UnityEngine;
using System.Collections;

// All Firearms will have this base-script
public class Firearm : Weapon
{
    [SerializeField] protected Transform firingPos;
    [SerializeField] AudioClip fireClip;
    [SerializeField] AudioClip reloadClip;

    public int Current_Ammo;
    public int MaxAmmo_perRound;
    public int TotalAmmo;
    public float ReloadingTime;
    public float FiringRange;

    public override void Reload()
    {
        if (Current_Ammo == MaxAmmo_perRound || TotalAmmo == 0)
        {
            // Don't need to Reload / Can't Reload
            return;
        }
        else
        {
            StartCoroutine(ReloadProcess(ReloadingTime));
        }
    }

    IEnumerator ReloadProcess(float _reloadTime)
    {
        // Reloading-SoundEffect
        ReloadAudio();

        // Reloading-Animation Delay
        yield return new WaitForSeconds(_reloadTime);

        // Minus Ammo to refill Current_Ammo
        int _ammoNeeded = MaxAmmo_perRound - Current_Ammo;

        if (_ammoNeeded <= TotalAmmo)
        {
            TotalAmmo -= _ammoNeeded;
        }
        else
        {
            _ammoNeeded = TotalAmmo;
        }
        
        Current_Ammo += _ammoNeeded;
    }

    protected void FireAudio()
    {
        _audio.PlayOneShot(fireClip);
    }

    protected void ReloadAudio()
    {
        _audio.PlayOneShot(reloadClip);
    }

    public override bool IsFireArm
    {
        get { return true; }
    }

    public override int WeaponCurrentAmmo()
    {
        return Current_Ammo;
    }

    public override int WeaponTotalAmmo()
    {
        return TotalAmmo;
    }
}
