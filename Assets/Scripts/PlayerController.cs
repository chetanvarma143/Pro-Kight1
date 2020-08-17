using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    

    
    

    private enum State { idle, running, jumping, falling,hurt}
    private State state = State.idle;
    
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int gems = 0;
    [SerializeField] private TextMeshProUGUI gemsText;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private AudioSource gem;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private int health;
    [SerializeField] private Text healthAmount;
    [SerializeField] public Joystick joystick;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        healthAmount.text = health.ToString();
    }
    private void Update()
    {
        if (state != State.hurt)
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state);

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable" )
        {
            gem.Play();
            Destroy(collision.gameObject);
            gems += 1;
            gemsText.text = gems.ToString();
        }
        if(collision.tag == "Powerup")
        {
            Destroy(collision.gameObject);
            jumpForce = 25f;
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (state == State.falling)
            {
                enemy.JumpedOn();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                state = State.jumping;
            }
            else
            {
                state = State.hurt;
                HandleHealth();
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }

    private void HandleHealth()
    {
        health -= 1;
        healthAmount.text = health.ToString();
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Movement()
    {
        float hDirection = joystick.Horizontal;

        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }

        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }

    }
    public void Jump()
    {
        

        if (coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = State.jumping;
            
        }
    }

    private void AnimationState()
    {
        if (state == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }

        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
    }

    private void Footstep()
    {
        footstep.Play();
    }

    private IEnumerator ResetPower()
    {
        yield return new WaitForSeconds(10);
        jumpForce = 19;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

}
 