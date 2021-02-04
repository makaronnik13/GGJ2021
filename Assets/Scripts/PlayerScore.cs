using System;
using UnityEngine;

[System.Serializable]
public class PlayerScore
{
    
    public string username;
    public string score;

    public PlayerScore(string playerName, string score)
    {
        this.username = playerName;
        this.score = score;
    }
}