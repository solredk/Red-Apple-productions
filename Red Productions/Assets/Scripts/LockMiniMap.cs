using UnityEngine;

public class LockMiniMap : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, 40, player.transform.position.z);
    }
}
