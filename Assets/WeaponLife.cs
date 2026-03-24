using UnityEngine;

public class WeaponLife : MonoBehaviour
{
    private Vector3 _origin;
    private float _maxDistance;
    private float _lifeTime;
    private float _timer;

    public void Initialize(Vector3 origin, float maxDistance, float lifeTime)
    {
        _origin = origin;
        _maxDistance = maxDistance;
        _lifeTime = lifeTime;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, _origin);

        if (distance > _maxDistance || _timer > _lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}