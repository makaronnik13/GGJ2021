using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SockHolder : MonoBehaviour, ISockHolder
{
    public bool CanHoldSock = true;
    public Animator Animator;
    public string Description;
    private bool used = false;

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
        }
    }

    public Vector3 Position { get => transform.position; }
    private bool monsterInside = false;
    public bool MonsterInside
    {
        get => monsterInside; set
        {
            monsterInside = value;
        }
    }

    public bool CanPlaceMonster
    {
        get
        {
            return Sock == null && MonsterInside == false;
        }
    }

    public bool CanPlaceSock
    {
        get
        {
            return Sock == null && MonsterInside == false;
        }
    }

    public void Use()
    {
    

        if (MonsterInside)
        {
            monsterInside = false;
            GameController.Instance.AtackByMonster(this);
            return;
        }

        if (Sock!=null)
        {
            AudioSource source = GetComponent<AudioSource>();
            if (source!=null)
            {
                source.PlayOneShot(source.clip);
            }

            if (used)
            {
                return;
            }

            if (Animator!=null)
            {
                Animator?.SetTrigger("Use");
                used = true;
            }
            else
            {
                GameController.Instance.CollectSock(Sock, transform.position);
                Sock = null;
            }
            
        }
        else
        {
            Animator?.SetTrigger("Use");
            ReplicsPlayer.Instance.ShowReplic(Description);
        }
    }

    public void UseAfterAnim()
    {
        used = false;
        if (Sock!=null)
        {
            GameController.Instance.CollectSock(Sock, transform.position);
        }
        Sock = null;
    }
}
