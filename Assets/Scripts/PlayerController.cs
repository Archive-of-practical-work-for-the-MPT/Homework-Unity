using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;

    public float MoveSpeed;

    public float JumpForce;

    public bool IsGrounded;
    
    private Animator animator;

    private Vector2 vect;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && IsGrounded) { JumpPlayer(); }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameObject.transform.position.y <= -3)
        {
            Death();
        }
        HorizontalMovePlayer();
        Rotate();
        setAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Death();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && collision.contacts[0].normal.y > 0.5f) { IsGrounded = true; }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3) { IsGrounded = false; }
    }

    private void setAnimation()
    {
        if (rb2d.velocity == Vector2.zero)
        {
            animator.SetInteger("Moving", 0);
        }
        else if (rb2d.velocity.y == 0 && rb2d.velocity.x != 0)
        {
            animator.SetInteger("Moving", 1);
        }
        else if (rb2d.velocity.y > 0)
        {
            animator.SetInteger("Moving", 3);
        }
        else if (rb2d.velocity.y < 0.3)
        {
            animator.SetInteger("Moving", 4);
        }
    }

    private void Death()
    {
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
        Invoke("Restart", 0.5f);
        animator.SetTrigger("Death");
    }

    private void Restart()
    {
        Vector3 checkpointPosition = Checkpoint.GetLastCheckpointPosition();

        Destroy(gameObject);

        GameObject playerPrefab = (GameObject)Resources.Load("Player");
        if (playerPrefab != null)
        {
            Object newPlayerObject = Instantiate(playerPrefab, checkpointPosition, Quaternion.identity);
            GameObject newPlayer = newPlayerObject as GameObject;

            if (newPlayer != null)
            {
                newPlayer.tag = "Player";

                CameraController cameraController = FindObjectOfType<CameraController>();
                if (cameraController != null)
                {
                    cameraController.player = newPlayer;

                    Vector3 vector = newPlayer.transform.position;
                    vector.z = cameraController.transform.position.z;
                    cameraController.transform.position = vector;
                    cameraController.offset = cameraController.transform.position - newPlayer.transform.position;


                }
            }
        }
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.A)) { gameObject.transform.rotation = Quaternion.Euler(0, 180, 0); }
        if (Input.GetKey(KeyCode.D)) { gameObject.transform.rotation = Quaternion.Euler(0, 0, 0); }
    }

    private void JumpPlayer()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(transform.up * JumpForce, ForceMode2D.Impulse);
        IsGrounded = false;
    }

    private void HorizontalMovePlayer()
    {
        float MoveInput = Input.GetAxis("Horizontal");
        vect = new Vector2(MoveInput, 0);
        rb2d.velocity = new Vector2(vect.x * MoveSpeed, rb2d.velocity.y);
    }
}
