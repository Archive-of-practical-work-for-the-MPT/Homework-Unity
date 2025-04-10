using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    public float smooth;

    void Start()
    {
        Vector3 vector = player.transform.position;
        vector.z = -100f;
        transform.position = vector;
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset,
            smooth * Time.deltaTime);
    }
}
