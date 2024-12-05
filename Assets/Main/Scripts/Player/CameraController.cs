using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject target;
    private Vector3 offset;
    private Vector3 HeightPos = new Vector3((float)0, (float)3, (float)0);

    void Start()
    {
        offset = transform.position;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.transform.position + offset + HeightPos;
            transform.rotation = target.transform.rotation;
        }
    }
}
