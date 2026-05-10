using System.Collections;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private GameObject hitParticlePrefab;

    private Renderer[] renderers;
    private Color[] originalColors;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;
    }

    public void PlayHitEffect(float blinkDuration = 0f)
    {
        StartCoroutine(ShakeCoroutine());
        StartCoroutine(BlinkCoroutine(blinkDuration));
        SpawnParticles();
    }

    public void PlayInvulnerabilityBlink(float duration)
    {
        StartCoroutine(BlinkCoroutine(duration));
    }

    IEnumerator BlinkCoroutine(float duration)
    {
        float elapsed = 0f;
        bool isRed = false;
        while (elapsed < duration)
        {
            isRed = !isRed;
            SetColor(isRed ? Color.red : Color.white);
            elapsed += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }
        RestoreColors();
    }

    IEnumerator ShakeCoroutine()
    {
        Transform cam = Camera.main.transform;
        Vector3 originalPos = cam.localPosition;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            cam.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.localPosition = originalPos;
    }

    void SpawnParticles()
    {
        if (hitParticlePrefab == null) return;
        GameObject p = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
        Destroy(p, 1f);
    }

    void SetColor(Color c)
    {
        foreach (var r in renderers)
            r.material.color = c;
    }

    void RestoreColors()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }
}