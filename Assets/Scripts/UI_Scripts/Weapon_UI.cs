using UnityEngine;
using UnityEngine.UI;

public class Weapon_UI : MonoBehaviour
{
    [SerializeField] PlayerScript player;
    [SerializeField] CanvasGroup weaponCanvasGroup;
    [SerializeField] CanvasGroup ammoCanvasGroup;
    [SerializeField] Image weaponImage;    
    [SerializeField] Text currAmmoText;
    [SerializeField] Text totalAmmoText;

	void LateUpdate ()
    {        
        if (player != null)
        {
            if (player.weapons != null)
            {
                // Show Weapon_UI
                weaponCanvasGroup.alpha = 1;

                if (player.weapons.currentWeapon.IsFireArm)
                {
                    // Display weapon-ammo UI 
                    // if it is a non-melee weapon
                    ammoCanvasGroup.alpha = 1;

                    // Display the weapon-ammo text UI 
                    currAmmoText.text = player.weapons.currentWeapon.WeaponCurrentAmmo().ToString();
                    totalAmmoText.text = player.weapons.currentWeapon.WeaponTotalAmmo().ToString();
                }
                else if (!player.weapons.currentWeapon.IsFireArm)
                {
                    // Hide the ammo UI
                    // if it is a melee weapon
                    ammoCanvasGroup.alpha = 0;

                    currAmmoText.text = System.String.Empty;
                    totalAmmoText.text = System.String.Empty;
                }
            }
            else
            {
                // Hide Weapon_UI
                weaponCanvasGroup.alpha = 0;
            }
        }
	}

    void NewWeaponUI(Sprite new_WeaponSprite)
    {
        // Update the current weapon in the UI
        weaponImage.sprite = new_WeaponSprite;
    }
}
