using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("single player en co-op settings")]
    [SerializeField] private WaveSpawner waveSpawner;

    [Header("co-op settings")]
    [SerializeField] private GameObject SecondPlayer;
    [SerializeField] private PlayerInputManager playerInputManager;

    private void Awake()
    {
        if (playerInputManager == null)
            StartCoroutine(waveSpawner.SpawnLoop());
    }

    public void DoJoinLobby()
    {
        //if there are 2 players you can spawn waves
        if (playerInputManager.playerCount == 2)
            StartCoroutine(waveSpawner.SpawnLoop());

        //if the first player is already in the game, then we can spawn the second player
        playerInputManager.playerPrefab = SecondPlayer;
    }
}
