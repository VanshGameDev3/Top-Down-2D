using UnityEngine;
using TMPro;
using System.Collections;

public class ArenaUIController : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private CanvasGroup waveGroup;

    [SerializeField] private TextMeshProUGUI arenaCompleteText;
    [SerializeField] private CanvasGroup arenaCompleteGroup;

    [SerializeField] private TextMeshProUGUI playerDiedText;
    [SerializeField] private CanvasGroup playerDiedGroup;

    [Header("Dust Effect")]
    [SerializeField] private ParticleSystem dustParticles;

    [Header("Timings")]
    [SerializeField] private float fadeIn = 0.5f;
    [SerializeField] private float stay = 1.5f;
    [SerializeField] private float fadeOut = 0.5f;

    public void ShowWaveText(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(waveText, waveGroup, message));
    }

    public void ShowArenaCompleted()
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(arenaCompleteText, arenaCompleteGroup, "CURSE LIFTED"));
    }

    public void ShowPlayerDied()
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(playerDiedText, playerDiedGroup, "Disappointed"));
    }

    private IEnumerator ShowRoutine(TextMeshProUGUI text, CanvasGroup group, string message)
    {
        text.text = message;
        text.gameObject.SetActive(true);

        yield return Fade(group, 0f, 1f, fadeIn);
        yield return new WaitForSeconds(stay);

        SpawnDust(text);

        yield return Fade(group, 1f, 0f, fadeOut);
        text.gameObject.SetActive(false);
    }

    private IEnumerator Fade(CanvasGroup group, float from, float to, float duration)
    {
        float t = 0f;
        group.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        group.alpha = to;
    }
    private void SpawnDust(TextMeshProUGUI text)
    {
        if (dustParticles == null)
            return;

        Camera cam = Camera.main;

        if (cam == null || !cam.enabled)
            return;

        if (!cam.gameObject.activeInHierarchy)
            return;

        Vector3 screenPos = text.rectTransform.position;

        screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.height);

        float safeZ = cam.nearClipPlane + 1f;

        Vector3 worldPos = cam.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, safeZ)
        );

        dustParticles.transform.position = worldPos;
        dustParticles.Play();
    }
}