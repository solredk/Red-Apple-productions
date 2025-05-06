using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private int playerCount = 0;
    
    [SerializeField] private List<GameObject> ActivePlayers;
    [SerializeField] private GameObject[] playerPrefabs;

    private void Awake()
    {
        playerInputManager.playerPrefab = playerPrefabs[playerCount];
    }
    public void OnJoined()
    {
        playerInputManager.playerPrefab = playerPrefabs[playerCount];
        playerCount += 1;
        ActivePlayers.Add(playerInputManager.playerPrefab);

    }

    public void OnLeft()
    {
        playerCount -= 1;
        ActivePlayers.Remove(playerInputManager.playerPrefab);
    }
}
