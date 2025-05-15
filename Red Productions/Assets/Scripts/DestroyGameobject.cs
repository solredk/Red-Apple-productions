using UnityEngine;

public class DestroyGameobject : MonoBehaviour
{
    [SerializeField] private float destroyTime = 3f;

    private void Start()
    {
        Destroy(gameObject, destroyTime); // Destroy this game object after the specified time
    }
}
