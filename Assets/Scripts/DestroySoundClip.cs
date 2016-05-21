using UnityEngine;
using System.Collections;

public class DestroySoundClip : MonoBehaviour
{
    AudioSource audioSource;
	// Use this for initialization
	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        Invoke("DestroySelf", audioSource.clip.length + 0.02f);
	}

	void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
