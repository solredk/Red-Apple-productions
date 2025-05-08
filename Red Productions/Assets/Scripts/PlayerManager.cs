using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private int playerCount = 0;
    
    [SerializeField] private List<GameObject> ActivePlayers;
    [SerializeField] private GameObject[] playerPrefabs;

    [SerializeField] private GameObject[] playerGameObjects;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (playerInputManager.playerCount == 1)
        {
            playerGameObjects[0].SetActive(true); // Corrected method name to 'SetActive'  
        }
        else if (playerInputManager.playerCount == 2)
        {
            playerGameObjects[1].SetActive(true); 
        }
        //playerInputManager.playerPrefab = playerPrefabs[playerCount];  
    }
    public void OnJoined()
    {
        //playerInputManager.playerPrefab = playerPrefabs[playerCount];
        //playerCount += 1;
       // ActivePlayers.Add(playerInputManager.playerPrefab);

    }

    public void OnLeft()
    {
        //playerCount -= 1;
       // ActivePlayers.Remove(playerInputManager.playerPrefab);
    }
}
