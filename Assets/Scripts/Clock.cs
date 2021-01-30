using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    [SerializeField]
    private Transform Arrow;

    [SerializeField]
    private GameObject View;

    [SerializeField]
    private Animator ClockAnimator;

    [SerializeField]
    private Image Fill;

    private bool danger = false;

    private void Awake()
    {
        View.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.GameStarted.AddListener(GameStartedChanged);
        GameController.Instance.StayTime.AddListener(StayTimeChanged);
        Fill.fillAmount = GameController.Instance.DangerPercent;
    }

    private void StayTimeChanged(float v)
    {
        if (!GameController.Instance.GameStarted.Value)
        {
            return;
        }

        float val = v / GameController.Instance.RoundTime;

        if (val<= GameController.Instance.DangerPercent && !danger)
        {
            ClockAnimator.SetTrigger("danger");
            danger = true;
        }

        if (val <= 0)
        {
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
            ClockAnimator.SetTrigger("end");
            danger = true;
            GameController.Instance.FinishGame();
        }
        Arrow.localRotation = Quaternion.Euler(new Vector3(0,0,360f*v/GameController.Instance.RoundTime));
    }

    private void GameStartedChanged(bool v)
    {
        if (v)
        {
            View.SetActive(true);
        }

    }

 
}
