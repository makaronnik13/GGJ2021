using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour
{
    [SerializeField]
    private AudioSource SocksSource, CoinSource;

    [SerializeField]
    private Transform scoresHub;

    [SerializeField]
    private ScoresController ScoresController;

    [SerializeField]
    private GameObject ScoreLinePrefab;

    [SerializeField]
    private TMPro.TMP_InputField PlayerName;

    [SerializeField]
    private Button ApplyScoreBtn;

    [SerializeField]
    private GameObject View, SocksView, MonsterView, PointsView, ScoresView;

    [SerializeField]
    private Animator SockAnimator, MonsterAnimator;

    [SerializeField]
    private Transform CollectedSocks;

    [SerializeField]
    private TMPro.TextMeshProUGUI SockTitle, ExtraScore, GlobalScore;

    [SerializeField]
    private Slider ScoreSlider;

    private int maxScore;

    private int scoreSum = 0;
    private int scoreBonus = 0;

    private bool sockFlying = false;

    private List<PlayerScore> localScores = new List<PlayerScore>();
    private PlayerScore currentScore;

    private Coroutine showScoresCoroutine;

    private void Start()
    {
        PlayerName.onValueChanged.AddListener(PlayerNameInputChanged);
        ScoresController.onScoreRecieved += ScoresRecieved;
    }

    private void ScoresRecieved(List<PlayerScore> scores)
    {
        if (showScoresCoroutine!=null)
        {
            StopCoroutine(showScoresCoroutine);
            showScoresCoroutine = null;
        }
        List<PlayerScore> allScores = new List<PlayerScore>(scores);
        foreach (PlayerScore ps in localScores)
        {
            allScores.Add(ps);
        }

        allScores = allScores.OrderByDescending(s=>s.score).ToList();

        foreach (Transform go in scoresHub)
        {
            Destroy(go.gameObject);
        }

        int i = 0;
        foreach (PlayerScore ps in allScores)
        {
            GameObject newLine = Instantiate(ScoreLinePrefab);
            newLine.transform.SetParent(scoresHub);
            newLine.GetComponent<ScorePanel>().Init(ps.username, ps.score, ps.username == currentScore.username && ps.score == currentScore.score);
            newLine.transform.localScale = Vector3.one;
        }

    }

    private void PlayerNameInputChanged(string v)
    {
        ApplyScoreBtn.interactable = v != string.Empty;
    }

    [ContextMenu("test")]
    public void Test()
    {
        View.SetActive(true);
        SocksView.SetActive(false);
        ScoresView.SetActive(true);
        PlayerName.text = "test";
        scoreSum = 10;
        ApplyScore();
    }

    public void ApplyScore()
    {
        PointsView.SetActive(false);
        ScoresView.SetActive(true);

        currentScore = new PlayerScore(PlayerName.text, scoreSum);

        localScores.Add(currentScore);

        ScoresController.AddScore(currentScore.username, currentScore.score);

        showScoresCoroutine = StartCoroutine(ShowScores());
    }

    private IEnumerator ShowScores()
    {
        yield return new WaitForSeconds(1f);
        ScoresRecieved(new List<PlayerScore>()
        {
            new PlayerScore("Kas", 35),
            new PlayerScore("Viktor", 25),
            new PlayerScore("Makar", 21),
            new PlayerScore("Nagibator3000", 100),
            new PlayerScore("JohnDoe", 12)
        });
    }

    public void Show()
    {
        View.SetActive(true);

        StartCoroutine(CountSocks());
    }

    private IEnumerator CountSocks()
    {
        GlobalScore.text = scoreSum.ToString();
        ScoreSlider.value = 0;
        ExtraScore.text = "0";

        maxScore = GameController.Instance.MaxScore;
        SocksView.SetActive(true);

        while (CollectedSocks.childCount != 0)
        {
            if (sockFlying)
            {
                yield return null;
            }
            else
            {
                StartCoroutine(MoveSockToPanel(CollectedSocks.transform.GetChild(0)));
            }
        }

        yield return new WaitForSeconds(1f);

        SocksView.SetActive(false);

        if (GameController.Instance.MonsterTriggered>0)
        {
            MonsterView.SetActive(true);
            MonsterAnimator.SetTrigger("Show");
            StartCoroutine(RemovePointsFromMonster(GameController.Instance.MonsterTriggered));
        }
        else
        {
            PlayerName.interactable = true;
        }
    }

    private IEnumerator RemovePointsFromMonster(int monsterTriggered)
    {
        int scoreForTrigger = 5;
        int removingScores = scoreForTrigger * monsterTriggered;
        scoreBonus = 0;

        while (removingScores > 0)
        {
            removingScores--;
            scoreBonus++;
            scoreSum--;
            ExtraScore.text = "-" + scoreBonus;
            GlobalScore.text = scoreSum.ToString();
            ScoreSlider.value = scoreSum / (maxScore + 0.0f);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
        //MonsterAnimator.SetTrigger("Hide");

        PlayerName.interactable = true;


    }

    private IEnumerator MoveSockToPanel(Transform tr)
    {
        sockFlying = true;

        /*
        tr.SetParent(SocksView.transform);
        float t = 1;
        Vector3 startPos = tr.position;
        while (t>0)
        {
            t -= Time.deltaTime;
            tr.position = Vector3.Lerp(SocksView.transform.position,startPos, t);
            yield return null;
        }
        */

        SockInstance sock = tr.GetComponentInChildren<SockImageView>().sock;
       

        SockAnimator.SetTrigger("Show");
        SocksSource.PlayOneShot(SocksSource.clip);
        SockTitle.text = sock.Data.SockName;


        foreach (SockImageView siv in SocksView.GetComponentsInChildren<SockImageView>())
        {
            siv.SetSock(sock);
        }

        tr.gameObject.SetActive(false);

        yield return StartCoroutine(CollectSockPoints(sock, 2));

        Destroy(tr.gameObject);
    }

    private IEnumerator CollectSockPoints(SockInstance sock, int count)
    {
        int points = sock.Data.Scores*count;

        scoreBonus = 0;

        while (points>0)
        {
            if (UnityEngine.Random.value<0.1f)
            {
                CoinSource.PlayOneShot(CoinSource.clip);
            }
            points--;
            scoreBonus++;
            scoreSum++;
            ExtraScore.text = "+" + scoreBonus;
            GlobalScore.text = scoreSum.ToString();
            ScoreSlider.value = scoreSum / (maxScore + 0.0f);
            yield return new WaitForSeconds(0.1f);
        }

        SockAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(1f);
        sockFlying = false;
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
