using PubNubAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoresController : MonoBehaviour
{
    public string user;
    public int testScore;

    PubNub pubnub;

    public List<PlayerScore> Scores = new List<PlayerScore>();

    public Action<List<PlayerScore>> onScoreRecieved = (v) => { };

    private void Start()
    {
        PNConfiguration pnConfiguration = new PNConfiguration();
        pnConfiguration.SubscribeKey = "sub-c-bf960f48-661d-11eb-bda1-0e29543bebe5";
        pnConfiguration.PublishKey = "pub-c-4ac9fc10-afa1-45da-9894-31376c2469af";
        pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
        pnConfiguration.UUID = System.Guid.NewGuid().ToString();


        pubnub = new PubNub(pnConfiguration);

        pubnub.SubscribeCallback += (sender, e) => 
        {
            SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

            Debug.Log(mea.Status);

            if (mea.Status != null)
            {
            }

            Debug.Log(mea.MessageResult);

            if (mea.MessageResult != null)
            {
                Debug.Log("recieved");

                Dictionary<string, object> msg = mea.MessageResult.Payload as Dictionary<string, object>;

                List<string> users = new List<string>();
                List<int> scores = new List<int>();

                Scores.Clear();

                foreach (KeyValuePair<string, object> pairs in msg)
                {
                    Debug.Log("____");
                    string s = "";

                    if (pairs.Key == "username")
                    {
                        Debug.Log(pairs.Value);

                        users = (pairs.Value as string[]).ToList();
                    }
                    if (pairs.Key == "score")
                    {
                        Debug.Log(pairs.Value);
                        Debug.Log(pairs.Value as int[]);

                        scores = (pairs.Value as int[]).ToList();

                    }
                }

                Debug.Log(scores.Count+"/"+users.Count);

                for (int i = 0; i < scores.Count; i++)
                {
                    Debug.Log(users[i]+"/"+scores[i]);

                    Scores.Add(new PlayerScore(users[i], scores[i]));
                }

                if (Scores.Count<10)
                {
                    Scores.Add(new PlayerScore("Username", 5));
                    Scores.Add(new PlayerScore("Viktor", 12));
                    Scores.Add(new PlayerScore("Kas", 32));
                    Scores.Add(new PlayerScore("Makar", 21));
                    Scores.Add(new PlayerScore("JohnDoe", 42));
                    Scores.Add(new PlayerScore("Nagibator777", 70));
                }

                onScoreRecieved(Scores);
            }
            if (mea.PresenceEventResult != null)
            {
                Debug.Log("In Example, SusbcribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event);
            }
        };

        pubnub.Subscribe()
            .Channels(new List<string>() {
                "my_chanel2"
            })
            .WithPresence()
            .Execute();

    }

    [ContextMenu("Test")]
    public void Test()
    {
        AddScore(user, testScore);
    }

    public void AddScore(string playerName, int score)
    {
        PlayerScore scoreObj = new PlayerScore(playerName, score);
        string msg = JsonUtility.ToJson(scoreObj);

        Debug.Log(msg);

        Debug.Log("Add "+playerName+"/"+score);
        pubnub.Fire().Channel("my_chanel").Message(msg).Async((result, status)=>
        {
            if (status.Error)
            {
                Debug.Log("error");
            }
            else
            {
                Debug.Log("sucsess "+result.Timetoken);
            }
        });
    }
}
