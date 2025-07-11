using TMPro;
using UnityEngine;

public class DamagePopupAnimation : MonoBehaviour
{
    public AnimationCurve opacityCurve;
    public AnimationCurve sizeCurve;
    public AnimationCurve heightCurve;
    private TextMeshProUGUI tmp;
    private float time = 0;
    private Vector3 origin;

    private void Awake()
    {

        tmp = GetComponent<TextMeshProUGUI>(); // Same object, no child needed
        Debug.Log("Initialized");
        origin = transform.position;
    }

    void Update()
    {
        if (tmp != null)
        {
            tmp.color = new Color(194f / 255f, 24f / 255f, 7f / 255f, opacityCurve.Evaluate(time));
            transform.localScale = Vector3.one * sizeCurve.Evaluate(time);
            transform.position = origin + new Vector3(0, 1 + heightCurve.Evaluate(time), 0);
            time += Time.deltaTime;
        }
    } 
}