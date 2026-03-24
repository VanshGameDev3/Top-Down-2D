using UnityEngine;
using TMPro;

public class TextDamagePop : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifeTime = 1f;

    private TextMeshPro tmp;

    void Start()
    {
        tmp = GetComponent<TextMeshPro>();
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    public void SetText(string text)
    {
        if (tmp == null) tmp = GetComponent<TextMeshPro>();
        tmp.text = text;
    }
}