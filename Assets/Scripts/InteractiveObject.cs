using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
    public UnityEvent OnClick;

    [SerializeField]
    private SpriteRenderer Renderer;

    private Material material;

    private void Start()
    {
        material = Renderer.material;
    }

    private void OnMouseEnter()
    {
        material.SetFloat("_IsOutlineEnabled", 1);
        MenuClick.Instance.Click();
    }

    private void OnMouseExit()
    {
        material.SetFloat("_IsOutlineEnabled", 0);
    }

    private void OnMouseDown()
    {
        OnClick.Invoke();
        material.SetFloat("_IsOutlineEnabled", 0);
    }
}
