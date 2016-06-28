using UnityEngine;

public class NoiseSource : MonoBehaviour
{
    [SerializeField] LayerMask noiseAffectLayer;

    Transform _trans;

    void Awake()
    {
        _trans = transform;
    }

    // Noise can attract AIs' attention in the area
    public void Noise(float _noiseRadius)
    {
        Collider[] noiseSphere = Physics.OverlapSphere(_trans.position, _noiseRadius, noiseAffectLayer);

        if (noiseSphere.Length > 0)
        {
            foreach (Collider ai in noiseSphere)
            {
                ai.GetComponent<AI>().HeardNoise(_trans.gameObject);
            }
        }

        Invoke("DestroyObj", 1);
    }

    void DestroyObj()
    {
        Destroy(gameObject);
    }
}
