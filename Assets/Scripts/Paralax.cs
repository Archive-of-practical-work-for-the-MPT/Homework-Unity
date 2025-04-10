using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = Camera.main.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Scroll();
    }

    void Scroll()
    {
        float delta = 0.001f * moveSpeed * Camera.main.velocity.x;
        transform.position += new Vector3(delta, 0, 0);
        transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, 0);
    }
}