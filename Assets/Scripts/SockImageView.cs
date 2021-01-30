using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SockImageView : MonoBehaviour
{

    [SerializeField]
    private Image Color, Texture;

    public SockInstance sock;

    public void SetSock(SockInstance sockData)
    {
        sock = sockData;
        if (sockData!=null)
        {
            Color.color = sockData.Data.Color;
            Texture.sprite = sockData.Data.Texture;
        }
    }
}
