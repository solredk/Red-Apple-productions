using UnityEngine;

public class ScreenRumble : MonoBehaviour
{
    [SerializeField] private float duration = 0.3f;  
    [SerializeField] private float magnitude = 0.2f; // the magnitude of the shake

    private Vector3 originalPos;
    private float shakeTimer = 0f;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * magnitude;

            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0f)
            {
                transform.localPosition = originalPos;
            }
        }
    }

    public void TriggerShake(float shakeDuration, float shakeMagnitude)
    {
        duration = shakeDuration;
        magnitude = shakeMagnitude;
        shakeTimer = duration;
    }

    public void TriggerShake()
    {
        shakeTimer = duration;
    }
}
