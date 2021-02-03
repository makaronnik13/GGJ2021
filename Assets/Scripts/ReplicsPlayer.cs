using RedBlueGames.Tools.TextTyper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReplicsPlayer : Singleton<ReplicsPlayer>
{
    [SerializeField]
    private TextTyper Typer;

    [SerializeField]
    private GameObject PlayerBuble;

    [SerializeField]
    private AudioSource MumbleSource;

    [SerializeField]
    private List<AudioClip> mumbles = new List<AudioClip>();

    Coroutine typewriterCoroutine;

    private void Start()
    {
        PlayerBuble.SetActive(false);
    }

    public void ShowReplic(string s)
    {
        PlayerBuble.SetActive(true);
        Typer.PrintCompleted.RemoveAllListeners();
        MumbleSource.Stop();
        MumbleSource.PlayOneShot(mumbles.OrderBy(c=>Guid.NewGuid()).FirstOrDefault());
        if (typewriterCoroutine!=null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
        
        Typer.TypeText(s);
        Typer.PrintCompleted.AddListener(PrintCompleted);
    }

    private void PrintCompleted()
    {
        typewriterCoroutine = StartCoroutine(HideBuble());
    }

    private IEnumerator HideBuble()
    {
        yield return new WaitForSeconds(2f);
        PlayerBuble.SetActive(false);
        typewriterCoroutine = null;
    }
}
