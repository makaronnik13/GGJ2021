using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    [Range(0f,1f)]
    private float HorizontalEdgeSize = 0.15f;
    [SerializeField]
    [Range(0f, 1f)]
    private float VerticalalEdgeSize = 0.15f;

    [SerializeField]
    private Vector2 MinPos, MaxPos;

    [SerializeField]
    private float CameraSpeed = 1f;

    void Update()
    {
        Vector3 p = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (p.x<HorizontalEdgeSize)
        { 
            transform.Translate(Vector3.left*CameraSpeed*Time.deltaTime);
        }
        if (p.x > 1f-HorizontalEdgeSize)
        {
            transform.Translate(Vector3.right * CameraSpeed * Time.deltaTime );
        }

        if (p.y < VerticalalEdgeSize)
        {
            transform.Translate(Vector3.down * CameraSpeed * Time.deltaTime);
        }
        if (p.y > 1f - VerticalalEdgeSize)
        {
            transform.Translate(Vector3.up * CameraSpeed * Time.deltaTime );
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, MinPos.x, MaxPos.x), Mathf.Clamp(transform.position.y, MinPos.y, MaxPos.y), transform.position.z);
    }
}
