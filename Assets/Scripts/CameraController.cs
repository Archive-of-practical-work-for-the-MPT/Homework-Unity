using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;
    public float smooth;

    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            Vector3 vector = player.transform.position;
            vector.z = -100f;
            transform.position = vector;
            offset = transform.position - player.transform.position;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, smooth * Time.deltaTime);
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                offset = transform.position - player.transform.position;
            }
        }
    }
}
