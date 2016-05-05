using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Weapon_UI : MonoBehaviour
{
    [SerializeField] Image weaponImage;
    [SerializeField] CanvasGroup ammoUI;
    [SerializeField] Text currAmmoText;
    [SerializeField] Text maxAmmoText;

    int _currAmmo;
    int _maxAmmo;

	void LateUpdate ()
    {
        // Display the weapon-ammo text UI 
        currAmmoText.text = _currAmmo.ToString();
        maxAmmoText.text = _maxAmmo.ToString();
	}

    void NewWeaponUI(Sprite new_WeaponSprite, bool _isMelee, int _newCurrAmmo, int _newMaxAmmo)
    {
        // Update the latest weapon-in-use in the UI
        weaponImage.sprite = new_WeaponSprite;

        // Check if it is a melee-weapon
        if (!_isMelee)
        {
            // Display weapon-ammo UI 
            // if it is a non-melee weapon
            _currAmmo = _newCurrAmmo;
            _maxAmmo = _newMaxAmmo;

            ammoUI.alpha = 1;
        }
        else
        {
            // Hide the ammo UI
            // if it is a melee weapon
            ammoUI.alpha = 0;

            _currAmmo = 0;
            _maxAmmo = 0;
        }
    }
}
