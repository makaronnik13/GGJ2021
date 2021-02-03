using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenPositioner : MonoBehaviour
{
    public Vector3 screenPos;

    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
    }
}
