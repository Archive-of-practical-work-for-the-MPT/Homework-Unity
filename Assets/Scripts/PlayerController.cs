using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float MoveSpeed;
    public float JumpForce;
    public bool IsGrounded;
    private Vector2 vect;
    private Animator animator;

    [SerializeField]
    private Joystick Joystick;

    public Animator check;
    private Vector2 lastCheckpointPosition;
    public float jumpThreshold = 0.7f;
    private AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip jumpSound;
    public AudioClip checkpointSound;
    public AudioClip winSound;
    private bool isCheckpointActivated = false;

    [SerializeField]
    private GameObject jumpIcon;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        lastCheckpointPosition = transform.position;

        if (jumpIcon != null)
        {
            EventTrigger trigger = jumpIcon.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = jumpIcon.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { OnJumpIconClicked(); });
            trigger.triggers.Add(entry);
        }
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && IsGrounded)
        {
            JumpPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.transform.position.y <= -1.5)
        {
            Death();
        }
        HorizontalMovePlayer();
        Rotate();
        setAnimation();
    }

    private void HorizontalMovePlayer()
    {
        float MoveInput = Input.GetAxis("Horizontal");
        vect = new Vector2(MoveInput, 0);
        rb2d.velocity = new Vector2(vect.x * MoveSpeed, rb2d.velocity.y);
        rb2d.velocity = new Vector2(Joystick.Horizontal * MoveSpeed, rb2d.velocity.y);
    }

    private void JumpPlayer()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(transform.up * JumpForce, ForceMode2D.Impulse);
        IsGrounded = false;
        if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
    }

    public void OnJumpIconClicked()
    {
        if (IsGrounded)
        {
            JumpPlayer();
        }
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.A)) { gameObject.transform.rotation = Quaternion.Euler(0, 180, 0); }
        if (Input.GetKey(KeyCode.D)) { gameObject.transform.rotation = Quaternion.Euler(0, 0, 0); }

        if (Joystick.Horizontal < -0.1f)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (Joystick.Horizontal > 0.1f)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 && collision.contacts[0].normal.y > 0.5f)
        {
            IsGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            IsGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Death();
        }
        if (collision.gameObject.layer == 8)
        {
            Win();
        }
    }

    private void Win()
    {
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
        if (winSound != null) audioSource.PlayOneShot(winSound);
        StartCoroutine(LoadMenuAfterDelay(2f));
    }

    private IEnumerator LoadMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Animator checkpointAnimator = collision.gameObject.GetComponent<Animator>();
            if (checkpointAnimator != null)
            {
                if (collision.gameObject.transform.position.x > lastCheckpointPosition.x)
                {
                    checkpointAnimator.SetTrigger("Check");
                    if (checkpointSound != null) audioSource.PlayOneShot(checkpointSound);
                    lastCheckpointPosition = collision.transform.position;
                }
            }
        }
    }

    private void Death()
    {
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
        if (deathSound != null) audioSource.PlayOneShot(deathSound);
        Invoke("Restart", 0.5f);
        animator.SetTrigger("Death");
    }

    private void Restart()
    {
        animator.SetTrigger("Respawn");
        transform.position = lastCheckpointPosition;
        rb2d.constraints = RigidbodyConstraints2D.None;
        rb2d.freezeRotation = true;
        rb2d.velocity = Vector2.zero;
        animator.SetInteger("Moving", 0);
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
}
