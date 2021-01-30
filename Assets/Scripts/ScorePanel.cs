using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePanel : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI Name, Score;

    public void Init(string name, int score, bool selected)
    {
        if (selected)
        {
            Name.color = Color.yellow;
            Score.color = Color.yellow;
        }
        Name.text = name;
        Score.text = score.ToString();
    }
}
