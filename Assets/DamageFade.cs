using UnityEngine;

public class DamageTextFade : MonoBehaviour
{
    public float lifetime = 1f;
    public float floatSpeed = 20f;

    void Update()
    {
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
            Destroy(gameObject);
    }
}