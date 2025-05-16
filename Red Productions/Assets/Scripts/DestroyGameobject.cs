using UnityEngine;

public class DestroyGameobject : MonoBehaviour
{
    [SerializeField] private float destroyTime = 3f;
    [SerializeField] private GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player object by its tag
        Destroy(gameObject, destroyTime); // Destroy this game object after the specified time
    }
    private void Update()
    {
        transform.LookAt(player.transform); // Make this object look at the player
    }
}
