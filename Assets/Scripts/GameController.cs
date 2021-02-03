using com.armatur.common.flags;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [SerializeField]
    private AudioSource MonsterSoundSource, MonsterAppearSource, SockFound, SockPairFound, Torchlight;

    [SerializeField]
    private AnimationCurve MonsterSoundSourceVolume;

    [SerializeField]
    private GameObject FlyingSockPrefab;

    [SerializeField]
    private Animator TorchlightAnimator, PlayerAnimator;

    [SerializeField]
    private EndScreenController EndScreen;

    [SerializeField]
    private GameObject SocksPairPrefab, MonsterPrefab;

    [SerializeField]
    private Transform CollectedSocksHub;

    [SerializeField]
    private float minCameraDist = 1f;

    [SerializeField]
    private SockVisual LeftSockVisual, RightSockVisual;
    public float RoundTime = 100f;
    [Range(0f, 1f)]
    public float DangerPercent = 0.2f;

    [SerializeField]
    private List<AudioClip> FoundSockClips;


    public int MonsterTriggered = 0;
    private bool warningTriggered = false;

    private GenericFlag<SockInstance> LeftSock = new GenericFlag<SockInstance>("left", null);
    private GenericFlag<SockInstance> RightSock = new GenericFlag<SockInstance>("right", null);
    public GenericFlag<float> StayTime = new GenericFlag<float>("stayTime", 0);
    public GenericFlag<bool> GameStarted = new GenericFlag<bool>("gameStarted", false); 


    private List<ISockHolder> Holders = new List<ISockHolder>();
    private List<SockInstance> Socks = new List<SockInstance>();

    public int MaxScore
    {
        get
        {
            int v = 0;
            foreach (SockInstance si in Socks)
            {
                v += si.Data.Scores;
            }
            return v;
        }
    }

    public void AtackByMonster(ISockHolder sockHolder)
    {
        MonsterAppearSource.PlayOneShot(MonsterAppearSource.clip);
        GameObject ScaryMonster = Instantiate(MonsterPrefab);
        ScaryMonster.transform.localScale = Vector3.one;
        ScaryMonster.transform.position = Camera.main.transform.position+Vector3.forward;
        Destroy(ScaryMonster, 1.5f);

        sockHolder.MonsterInside = false;
        MonsterTriggered++;
        PlayerAnimator.SetTrigger("Scared");

        StartCoroutine(RemoveSocksFromLegs());


        ISockHolder holder = Holders.Where(h => h.CanPlaceMonster).OrderBy(s => Guid.NewGuid()).FirstOrDefault();

        if (holder != null)
        {
            holder.MonsterInside = true;
        }

        
        warningTriggered = false;
    }

    public IEnumerator RemoveSocksFromLegs()
    {
        yield return new WaitForSeconds(0.3f);

        if (LeftSock.Value != null)
        {
            PlaceSock(LeftSock.Value);
            Debug.Log("remove left sock");
            LeftSock.SetState(null);
        }
        if (RightSock.Value != null)
        {
            PlaceSock(RightSock.Value);
            Debug.Log("remove right sock");
            RightSock.SetState(null);
        }

        yield return new WaitForSeconds(1.5f);

        ReplicsPlayer.Instance.ShowReplic("That monster stole all my socks! Stinker!");
    }

    private void Start()
    {
        foreach (SockPosition sp in FindObjectsOfType<SockPosition>())
        {
            Holders.Add(sp);
        }

        foreach (SockHolder sp in FindObjectsOfType<SockHolder>().Where(h=>h.CanHoldSock))
        {
            Holders.Add(sp);
        }

        foreach (SockData sd in Resources.LoadAll<SockData>("ScriptableObjects/Socks").OrderBy(s=>Guid.NewGuid()).Take(9))
        {
            Socks.Add(new SockInstance(sd));
            Socks.Add(new SockInstance(sd));
        }

        Socks = Socks.OrderBy(s=>Guid.NewGuid()).ToList();
        Holders = Holders.OrderBy(s => Guid.NewGuid()).ToList();

        Holders.Where(h => h.CanPlaceMonster).OrderBy(s => Guid.NewGuid()).FirstOrDefault().MonsterInside = true;

        foreach (SockInstance si in Socks)
        {
            PlaceSock(si);
        }

        LeftSock.AddListener(LeftSockChanged);
        RightSock.AddListener(RightSockChanged);

        ReplicsPlayer.Instance.ShowReplic("Where are all my socks? Cant see anything. Oh, I have a flashlight!");
    }

    private void PlaceSock(SockInstance si, ISockHolder concreteHolder = null)
    {
        if (concreteHolder != null)
        {
            concreteHolder.Sock = si;
            return;
        }

        List<ISockHolder> avaliableHolders = new List<ISockHolder>();

        int holdersCount = Holders.Where(h=> Vector3.Distance(h.Position, Camera.main.transform.position) >= minCameraDist && h.CanPlaceSock && h.GetType()==typeof(SockHolder)).Count();


        if (holdersCount< Holders.Where(h=>h.GetType() == typeof(SockHolder)).Count()/2f)
        {
            ISockHolder holder = Holders.Where(h => Vector3.Distance(h.Position, Camera.main.transform.position) >= minCameraDist && h.CanPlaceSock && h.GetType() == typeof(SockPosition)).OrderBy(h => Guid.NewGuid()).FirstOrDefault();
            if (holder == null)
            {
                holder = Holders.Where(h=>h.CanPlaceSock).OrderBy(h=>Guid.NewGuid()).FirstOrDefault();
            }
            holder.Sock = si;
        }
        else
        {
            ISockHolder holder2 = Holders.Where(h => Vector3.Distance(h.Position, Camera.main.transform.position) >= minCameraDist && h.CanPlaceSock && h.GetType() == typeof(SockHolder)).OrderBy(h => Guid.NewGuid()).FirstOrDefault();
            if (holder2 == null)
            {
                holder2 = Holders.Where(h => h.CanPlaceSock).OrderBy(h => Guid.NewGuid()).FirstOrDefault();
            }
            holder2.Sock = si;
        }

    }

    private void RightSockChanged(SockInstance sock)
    {
            RightSockVisual.SetSock(sock);
    }

    private void LeftSockChanged(SockInstance sock)
    {
        LeftSockVisual.SetSock(sock);
    }

    public void StartGame()
    {
        Torchlight.PlayOneShot(Torchlight.clip);
        TorchlightAnimator.SetTrigger("On");
        GameStarted.SetState(true);
        StayTime.SetState(RoundTime);
    }

    public void FinishGame()
    {
        Debug.Log("Stop");
        GameStarted.SetState(false);
        EndScreen.Show();
    }

    private void Update()
    {
        if (GameStarted.Value)
        {
            StayTime.SetState(StayTime.Value-Time.deltaTime);
        }

        if (!GameStarted.Value && Input.anyKeyDown)
        {
            StartGame();
        }

        ISockHolder holder = Holders.FirstOrDefault(h => h.MonsterInside);
        if (holder!=null)
        {
            Vector3 monsterPos = Holders.FirstOrDefault(h => h.MonsterInside).Position;
            float dist = Vector2.Distance(new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y), new Vector2(monsterPos.x, monsterPos.y));
            MonsterSoundSource.volume = MonsterSoundSourceVolume.Evaluate(dist);
        }
        else
        {
            MonsterSoundSource.volume = 0;
        }

        if (warningTriggered == false && MonsterSoundSource.volume>0.3f)
        {
            ReplicsPlayer.Instance.ShowReplic("I have a bad feeling");
            warningTriggered = true;
        }
    }

    public void CollectSock(SockInstance sock, Vector3 position)
    {
        SockFound.PlayOneShot(FoundSockClips.OrderBy(s=>Guid.NewGuid()).FirstOrDefault());
        GameObject newSock = Instantiate(FlyingSockPrefab);
        newSock.transform.position = position;
        newSock.GetComponentInChildren<SockVisual>().SetSock(sock);

        Transform aim = RightSockVisual.transform;

        if (RightSock.Value != null)
        {
            aim = LeftSockVisual.transform;
        }

        if (Holders.FirstOrDefault(h=>h.MonsterInside)==null)
        {
            ISockHolder holder = Holders.Where(h => h.CanPlaceMonster).OrderBy(s => Guid.NewGuid()).FirstOrDefault();
            if (holder != null)
            {
                holder.MonsterInside = true;
            }
        }

        StartCoroutine(MoveSockTo(newSock, aim, 1f));
    }

    private IEnumerator MoveSockTo(GameObject sock, Transform aim, float v)
    {
        float t = v;

        Vector3 start = sock.transform.position;

        while (t>0)
        {
            sock.transform.position = Vector3.Lerp(aim.position,start, t/v);
            t -= Time.deltaTime;
            yield return null;
        }
        SockRecieved(sock.GetComponentInChildren<SockVisual>().Sock);
        Destroy(sock);
    }

    public void SockRecieved(SockInstance sock)
    {
        if (sock != null && RightSock.Value != null && RightSock.Value.Data == sock.Data)
        {
            RightSock.SetState(null);
            CollectSocksPair(sock);
            return;
        }

        if (sock != null && LeftSock.Value != null && LeftSock.Value.Data == sock.Data)
        {
            LeftSock.SetState(null);
            CollectSocksPair(sock);
            return;
        }

        if (RightSock.Value == null)
        {
            RightSock.SetState(sock);
        }
        else if (LeftSock.Value == null)
        {
            LeftSock.SetState(sock);
        }
        else
        {
            float r = UnityEngine.Random.value;
            if (r>0.7f)
            {
                ReplicsPlayer.Instance.ShowReplic("Damn, too many socks! I have just two legs.");
            }
            else if(r>0.2f)
            {
                ReplicsPlayer.Instance.ShowReplic("I need pairs!");
            }
            else 
            {
                ReplicsPlayer.Instance.ShowReplic("These socks are not alike!");
            }
           

            SockInstance removingSock = RightSock.Value;
            RightSock.SetState(LeftSock.Value);
            LeftSock.SetState(sock);
            PlaceSock(removingSock, sock.holder);
        }

        if (Holders.FirstOrDefault(s => s.Sock != null) == null)
        {
            Debug.Log("носки кончились");
            FinishGame();
        }
    }

    private void CollectSocksPair(SockInstance sock)
    {
        SockPairFound.PlayOneShot(SockPairFound.clip);
        GameObject newSockPair = Instantiate(SocksPairPrefab);
        newSockPair.transform.SetParent(CollectedSocksHub);
        newSockPair.transform.localScale = Vector3.one;
        foreach (SockImageView iv in newSockPair.GetComponentsInChildren<SockImageView>())
        {
            iv.SetSock(sock);
        }
    }

    [ContextMenu("Gameover")]
    private void FakeGameover()
    {
        MonsterTriggered = 3;
        CollectSocksPair(Socks[0]);
        CollectSocksPair(Socks[1]);
        CollectSocksPair(Socks[2]);
        CollectSocksPair(Socks[3]);
        CollectSock(Socks[4], Vector3.zero);
        CollectSock(Socks[5], Vector3.zero);
        FinishGame();
    }
}
