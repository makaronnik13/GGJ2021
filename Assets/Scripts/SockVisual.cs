using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SockVisual : MonoBehaviour
{
    [SerializeField]
    private GameObject View;

    [SerializeField]
    private SpriteRenderer Color, Texture;

    public SockInstance Sock;

    private void Awake()
    {
        SetSock(null);
    }

    public void SetSock(SockInstance sock)
    {
        this.Sock = sock;

        if (sock == null)
        {
            View.SetActive(false);
        }
        else
        {
            View.SetActive(true);
            Color.color = sock.Data.Color;
            Texture.sprite = sock.Data.Texture;
        }
    }
}
