using PubNubAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoresController : MonoBehaviour
{
    PubNub pubnub;

    public List<PlayerScore> Scores = new List<PlayerScore>(); 

    private void Start()
    {
        Scores.Add(new PlayerScore("Username", 50));
        Scores.Add(new PlayerScore("Your mom", 500));
        Scores.Add(new PlayerScore("Viktor", 111));
        Scores.Add(new PlayerScore("Kas", 69));
        Scores.Add(new PlayerScore("Makar", 21));
        Scores.Add(new PlayerScore("JohnDoe", 42));
        Scores.Add(new PlayerScore("Nagibator777", 120));


        PNConfiguration pnConfiguration = new PNConfiguration();
        pnConfiguration.SubscribeKey = "sub-c-e54607e0-62e7-11eb-8d92-92d9cc8397ac";
        pnConfiguration.PublishKey = "pub-c-260f9afb-b214-47d8-a219-8c890dbc160d";
        pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
        pnConfiguration.UUID = System.Guid.NewGuid().ToString();


        pubnub = new PubNub(pnConfiguration);

        pubnub.SubscribeCallback += (sender, e) => 
        {
            Scores.Clear();

            SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;


            if (mea.Status != null)
            {
            }
            if (mea.MessageResult != null)
            {
                Dictionary<string, object> msg = mea.MessageResult.Payload as Dictionary<string, object>;

                string[] strArr = msg["username"] as string[];
                string[] strScores = msg["score"] as string[];

                for (int i = 0; i < strArr.Length; i++)
                {
                    Scores.Add(new PlayerScore(strArr[i], int.Parse(strScores[i])));
                }

                Debug.Log(Scores.Count);
            }
            if (mea.PresenceEventResult != null)
            {
                Debug.Log("In Example, SusbcribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event);
            }
        };
        pubnub.Subscribe()
            .Channels(new List<string>() {
                "my_channel"
            })
            .WithPresence()
            .Execute();

    }

    [ContextMenu("Test")]
    public void Test()
    {
        AddScore("TestUser_"+Random.Range(0, 5), Random.Range(1, 50));
    }

    public void AddScore(string playerName, int score)
    {
        PlayerScore scoreObj = new PlayerScore(playerName, score);
        string msg = JsonUtility.ToJson(scoreObj);

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
