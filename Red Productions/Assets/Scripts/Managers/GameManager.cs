using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject SecondPlayer;

    [SerializeField] private PlayerInputManager playerInputManager;

    [SerializeField] private WaveSpawner waveSpawner;

    public void DoJoinLobby()
    {

        if (playerInputManager.playerCount == 2)
        {
            StartCoroutine(waveSpawner.SpawnLoop());
        }

        playerInputManager.playerPrefab = SecondPlayer;
    }
}
