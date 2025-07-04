using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI promptText;

    void Start()
    {
        if (promptText == null)
        {
            Debug.LogWarning("PlayerUI: promptText is not assigned in the inspector!");
        }
    }

    public void UpdateText(string promptMessage)
    {
        if (promptText != null)
        {
            promptText.text = promptMessage;
        }
    }
}