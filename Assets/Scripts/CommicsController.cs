using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommicsController: MonoBehaviour
{
    public bool Started = false;

    public Animator Animator;

    public void Show()
    {
        Started = true;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Animator.SetTrigger("next");
        }
    }

    public void CommicsEnded()
    {
        SceneManager.LoadScene(1);
    }
}