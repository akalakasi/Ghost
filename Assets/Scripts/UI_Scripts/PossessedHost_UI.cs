using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PossessedHost_UI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] PlayerScript player;
    [SerializeField] Text possessedHostName;

	void Start ()
    {
        if (player != null)
        {
            StartCoroutine(PossessedHost());
        }
	}

    IEnumerator PossessedHost()
    {
        while (player.currentPState == PlayerScript.PossesState.POSSESSING)
        {
            // Display UI & possessed host's name
            canvasGroup.alpha = 1;
            possessedHostName.text = player.possessedBody.name;

            yield return null;
        }
        
        while (player.currentPState != PlayerScript.PossesState.POSSESSING)
        {
            // Hide UI & possessed host's name
            canvasGroup.alpha = 0;
            possessedHostName.text = System.String.Empty;

            yield return null;
        }

        StartCoroutine(PossessedHost());    
    }
}
