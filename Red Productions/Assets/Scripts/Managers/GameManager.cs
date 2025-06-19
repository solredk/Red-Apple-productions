using UnityEngine;
using UnityEngine.InputSystem;

public enum GameMode
{
    SinglePlayer,
    CoOp,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameMode gameMode;

    [Header("single player en co-op settings")]
    [SerializeField] private WaveSpawner waveSpawner;

    [Header("co-op settings")]
    [SerializeField] private GameObject SecondPlayer;
    [SerializeField] private PlayerInputManager playerInputManager;

    private bool waveStarted = false;
    private void Awake()
    {
        Instance = this;

        if (playerInputManager == null)
        {
            StartCoroutine(waveSpawner.SpawnLoop());
        }
    }
    private void Update()
    {
        if (playerInputManager == null)
            return;

        if (playerInputManager.playerCount == 1)
            playerInputManager.playerPrefab = SecondPlayer;

        if (playerInputManager.playerCount == 2 && waveStarted == false)
        {
            waveStarted = true;
            StartCoroutine(waveSpawner.SpawnLoop());
        }
    }
    public void DoJoinLobby()
    {
        //if there are 2 players you can spawn waves
        if (playerInputManager.playerCount == 2)
        {
            StartCoroutine(waveSpawner.SpawnLoop());
        }
            //if the first player is already in the game, then we can spawn the second player
            playerInputManager.playerPrefab = SecondPlayer;
    }
}
