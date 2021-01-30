using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchlightController : MonoBehaviour
{
    [Range(0, 3)]
    public float Size;

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * Size;
    }
}
