using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static Vector3 lastCheckpointPosition;

    private Animator animator;

    private bool isActivated = false;

    void Start()
    {
        if (lastCheckpointPosition == Vector3.zero)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                lastCheckpointPosition = player.transform.position;
            }
        }

        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            lastCheckpointPosition = transform.position;
            Debug.Log("Чекпоинт!");

            if (animator != null)
            {
                animator.SetBool("IsActivated", true);
            }

            isActivated = true;
        }
    }

    public static Vector3 GetLastCheckpointPosition()
    {
        return lastCheckpointPosition;
    }
}