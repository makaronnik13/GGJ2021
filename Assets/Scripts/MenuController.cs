using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    public Animator TitleAnimator;

    [SerializeField]
    private CommicsController Commics;

    private bool started = false;

    // Update is called once per frame
    void Update()
    {
        if (!Commics.Started && Input.anyKeyDown)
        {
            TitleAnimator.SetTrigger("Started");
            Commics.Show();
        }
    }
}
