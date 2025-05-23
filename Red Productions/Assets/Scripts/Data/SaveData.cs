using System.Collections.Generic;
using UnityEngine;
public enum GameMode
{
    SinglePlayer,
    CoOp,
}

[System.Serializable]
public class SaveData
{
    public GameMode gameMode;

    public int singlePlayerScore;
    public int singlePlayerLastScore;
    public int singlePlayerHighscore;

    public List<int> multiPlayerPlayerScore;



}
