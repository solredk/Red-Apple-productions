using UnityEngine;

public class UIBillboarding : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            // Make the UI face the camera
            transform.LookAt(Camera.main.transform);
            // Optionally, keep the UI upright (not upside down)
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}