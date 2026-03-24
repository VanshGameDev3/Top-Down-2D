using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private CinemachineImpulseSource impulse;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        impulse = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float strength = 1f)
    {
        impulse.GenerateImpulse(strength);
    }
}