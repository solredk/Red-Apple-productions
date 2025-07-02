using NUnit.Framework;
using System.Collections.Generic;
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


    [SerializeField] private InputManager inputManager;
    [Header("single player en co-op settings")]
    [SerializeField] private WaveSpawner waveSpawner;

    [Header("co-op settings")]
    public List<GameObject> players;
    public PlayerInputManager playerInputManager;

    [Header("LoadScene Component")]
    public Loadscene loadscene;

    private int aliveCount;

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

        if (playerInputManager.playerCount == 2 && players.Count != 2)
        {
            for (int i = 0; i < playerInputManager.playerCount; i++)
            {
                if (players.Count <= i)
                {
                    GameObject[] playerPrefab = GameObject.FindGameObjectsWithTag("Player");
                    players.Add(playerPrefab[i]);
                }
            }
        }


        if (playerInputManager.playerCount == 2 && waveStarted == false)
        {
            waveStarted = true;
            StartCoroutine(waveSpawner.SpawnLoop());
        }
    }

    private void LateUpdate()
    {

    }

    public void PlayerDied()
    {
        aliveCount--;
    }

    public void PlayerSpawned()
    {
        aliveCount++;
    }

    public void DeathScene()
    {
        loadscene.LoadScene();
    }

    public void DoJoinLobby()
    {
        //if there are 2 players you can spawn waves
        if (playerInputManager.playerCount == 2)
        {
            StartCoroutine(waveSpawner.SpawnLoop());
        }
    }
}
