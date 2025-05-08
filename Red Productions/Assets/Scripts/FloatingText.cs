using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 3f; // Time in seconds before the text is destroyed
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroyTime); // Destroy the text after the specified time
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
