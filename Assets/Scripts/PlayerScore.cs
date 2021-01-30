using System;
using UnityEngine;

[System.Serializable]
public class PlayerScore
{
    
    public string username;
    public int score;

    public PlayerScore(string playerName, int score)
    {
        this.username = playerName;
        this.score = score;
    }
}