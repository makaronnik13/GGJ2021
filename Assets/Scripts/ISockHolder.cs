using UnityEngine;

public interface ISockHolder
{
    SockInstance Sock
    {
        get;
        set;
    }
    Vector3 Position { get;}

    bool MonsterInside
    {
        get;
        set;
    }

    bool CanPlaceMonster
    {
        get;
    }

    bool CanPlaceSock
    {
        get;
    }
}