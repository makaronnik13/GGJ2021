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
        pnConfiguration.SubscribeKey = "sub-c-7510f5ce-66df-11eb-941a-1292edd3dfab";
        pnConfiguration.PublishKey = "pub-c-489ed4c0-0e28-4b6a-a3b5-b91f6bdba9f8";
        pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
        pnConfiguration.UUID = System.Guid.NewGuid().ToString();


        pubnub = new PubNub(pnConfiguration);

        PlayerScore myFireObject = new PlayerScore("testUser", 0.ToString());
        string fireobject = JsonUtility.ToJson(myFireObject);
        pubnub.Fire()
          .Channel("my_channel")
          .Message(fireobject)
          .Async((result, status) => {
              if (status.Error)
              {
                  Debug.Log(status.Error);
                  Debug.Log(status.ErrorData.Info);
              }
              else
              {
                  Debug.Log(string.Format("Fire Timetoken: {0}", result.Timetoken));
              }
          });


        pubnub.SubscribeCallback += (sender, e) => 
        {
            SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

            Debug.Log(mea.Status);

            if (mea.Status != null)
            {
            }


            if (mea.MessageResult != null)
            {
                Debug.Log("recieved");


                Debug.Log(mea.MessageResult.MessageType);
                Debug.Log(mea.MessageResult.Payload);

                Dictionary<string, object> msg = mea.MessageResult.Payload as Dictionary<string, object>;

                    List<string> users = new List<string>();
                    List<string> scores = new List<string>();

                    Scores.Clear();

                    foreach (KeyValuePair<string, object> pairs in msg)
                    {
                        string s = "";

                        if (pairs.Key == "username")
                        {
                        if ((pairs.Value as object[]).Length>0)
                        {
                            users = (pairs.Value as string[]).ToList();
                        }
                            
                        }
                        if (pairs.Key == "score")
                        {
                        if ((pairs.Value as object[]).Length > 0)
                        {
                            scores = (pairs.Value as string[]).ToList();
                        }
                        }
                    }


                    for (int i = 0; i < scores.Count; i++)
                    {
                        Scores.Add(new PlayerScore(users[i], scores[i]));
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
                "my_channel2"
            })
            .WithPresence()
            .Execute();

    }

    [ContextMenu("Test")]
    public void Test()
    {
        AddScore(user, testScore.ToString());
    }

    public void AddScore(string playerName, string score)
    {
        PlayerScore scoreObj = new PlayerScore(playerName, score);
        string msg = JsonUtility.ToJson(scoreObj);

        pubnub.Publish()
      .Channel("my_channel")
      .Message(msg)
      .Async((result, status) => {
          if (!status.Error)
          {
              Debug.Log(string.Format("Publish Timetoken: {0}", result.Timetoken));
          }
          else
          {
              Debug.Log(status.Error);
              Debug.Log(status.ErrorData.Info);
          }
      });

      
    }
}
