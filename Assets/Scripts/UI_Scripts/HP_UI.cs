using UnityEngine;
using UnityEngine.UI;

public class HP_UI : MonoBehaviour
{
    [SerializeField] Image hpbarImage;
    [SerializeField] Stats player;

    void LateUpdate()
    {
        // Displays the user-hp on the HP-UI
        hpbarImage.fillAmount = player.currHP / player.maxHP;
    }
}
