using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HP_UI : MonoBehaviour
{
    [SerializeField] Image hpbarImage;
    [SerializeField] PlayerScript player;

    void LateUpdate()
    {
        // Displays the user-hp on the HP-UI
        hpbarImage.fillAmount = player.currHP / player.HP;
    }
}
