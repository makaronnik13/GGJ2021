using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SockData", fileName = "Sock")]
public class SockData : ScriptableObject
{
    public string SockName;
    public Sprite Texture;
    public Color Color;
    public int Scores;
}
