using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SockPosition : MonoBehaviour, ISockHolder
{
    [SerializeField]
    private SockVisual Visual;

    private SockInstance sock;
    public SockInstance Sock
    {
        get
        {
            return sock;
        }
        set
        {
            sock = value;
            if (sock!=null)
            {
                Visual.gameObject.SetActive(true);
                Visual.SetSock(sock);
                sock.holder = this;
            }
            else
            {
                Visual.gameObject.SetActive(false);
            }
        }
    }

    public bool CanPlaceMonster
    {
        get
        {
            return false;
        }
    }

    public bool CanPlaceSock
    {
        get
        {
            return Sock == null && MonsterInside == false;
        }
    }

    private bool monsterInside = false;
    public bool MonsterInside
    {
        get => monsterInside; set
        {
            monsterInside = value;
        }
    }

    public Vector3 Position { get => transform.position; }

    void Awake()
    {
        Visual.gameObject.SetActive(false);
    }

    public void Collect()
    {
        if (MonsterInside)
        {
            GameController.Instance.AtackByMonster(this);
            return;
        }

        GameController.Instance.CollectSock(Sock, transform.position);
        Sock = null;

    }
}
