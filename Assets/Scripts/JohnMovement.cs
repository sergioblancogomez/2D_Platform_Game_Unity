using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JohnMovement : MonoBehaviour
{
    public AudioClip SoundJump;
    public AudioClip SoundHit;
    public GameObject BulletPrefab;
    public float Speed;
    public float JumpForce;
    private Rigidbody2D Rigidbody2D;
    private float Horizontal;
    private bool Grounded;
    private Animator Animator;
    private float LastShoot;
    public float maxHealth;
    public float currentHealth;
    private Animation anim;
    public Image image;
    
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = currentHealth / maxHealth;
        Horizontal = Input.GetAxis("Horizontal");

        if (Horizontal < 0.0f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else if (Horizontal > 0.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        Animator.SetBool("Running", Horizontal != 0.0f);

        //Debug.DrawRay(transform.position, Vector3.down * 0.1f, Color.red);

        if (Physics2D.Raycast(transform.position, Vector3.down, 0.14f))
        {
            Grounded = true;
        }

        else Grounded = false;

        if (Input.GetKeyDown(KeyCode.W) && Grounded)
        {
            Camera.main.GetComponent<AudioSource>().PlayOneShot(SoundJump);
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > LastShoot + 0.25f)
        {
            Shoot();
            LastShoot = Time.time;
        }
    }

    private void Jump ()
    {
        
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private void FixedUpdate()
    {
        Rigidbody2D.velocity = new Vector2(Horizontal, Rigidbody2D.velocity.y * Speed);

    }

    private void Shoot ()
    {
        Vector3 direction;

        if (transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;

        GameObject bullet = Instantiate(BulletPrefab, transform.position + direction * 0.1f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);
    }

    public void Hit()
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(SoundHit);
        currentHealth = currentHealth - 1;
        if(currentHealth == 0) 
        {
            Animator.SetBool("IsDead", true);
            Rigidbody2D.velocity = Vector2.zero;
            StartCoroutine(Die());
        } 
    }

    IEnumerator Die(){
        yield return new WaitForSeconds(1); //waits 1 seconds
        Destroy(gameObject); 
    }
}
