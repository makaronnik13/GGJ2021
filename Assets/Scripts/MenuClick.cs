using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuClick : Singleton<MenuClick>
{
    public void Click()
    {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
    }
}
