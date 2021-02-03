using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommicsController: MonoBehaviour
{
    public bool Started = false;

    public Animator Animator;

    public void Show()
    {
        StartCoroutine(DealyShow());
    }

    private IEnumerator DealyShow()
    {
        yield return new WaitForSeconds(1f);
        Animator.SetTrigger("next");
        yield return new WaitForSeconds(1f);

        FindObjectOfType<MenuController>().TitleAnimator.gameObject.SetActive(false);
       
        Started = true;
    }

    void Update()
    {
        if (Input.anyKeyDown && Started)
        {
            CommicsEnded();
        }
    }

    public void CommicsEnded()
    {
        SceneManager.LoadScene(1);
    }

    public void Sound()
    {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }
}