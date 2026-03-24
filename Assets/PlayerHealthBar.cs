using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("Images")]
    public Image fillImage;  

    public void UpdateHealth(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
}