using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fly : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject camera;
    Rigidbody2D rb2d;
    public GameObject panel;
    public Text txt;
    SpriteRenderer sprite;
    public EnergyBar energyBar;
    public ScriptHandler scriptHandler;

    [Header("Energia")]
    public float maxEnergy;
    public float currentEnergy;
    public float energyTaken;
    public float energyRestoren;
    public float loseEnergyByTime;

    [Header("Points")]
    public int points;
    public int total;
    public bool end;

    [Header("Movement")]
    public float speed;
    float horizontalInput;

    [Header("Jump")]
    public float jumpForce;
    bool grounded;
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;
    public float jumpTimeDecrease;

    [Header("Jump buffer")]
    public float jumpBufferTimer;
    private float jumpBufferTimerCounter;

    [Header("Fly")]
    public float flyForce;
    public float gravityFlying;
    public float gravityMove;






    //Start
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        currentEnergy = maxEnergy;
        energyBar.MaxEnergy(maxEnergy);
        energyBar.SetMaxEnergy();
        energyBar.setEnergyTimer(loseEnergyByTime);
        points = 0;
        total = 37;
        end = false;
        scriptHandler.CheckpointHit(transform.position);

    }

    //Update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage();
        }

        Debug.Log(grounded);
        //Camera
        //======================================================================================
        camera.transform.position = new Vector3(transform.position.x, transform.position.y + 2, -10);
        if (camera.transform.position.x <= 1)
        {
            camera.transform.position = new Vector3(1, camera.transform.position.y, -10);
        }
        else if (camera.transform.position.x >= 420)
        {
            camera.transform.position = new Vector3(420, camera.transform.position.y, -10);
        }
        if (camera.transform.position.y <= -4)
        {
            camera.transform.position = new Vector3(camera.transform.position.x, -4, -10);
        }
        //======================================================================================

        //Movimento
        //======================================================================================
        if (!end)
        {
            panel.SetActive(false);
            horizontalInput = Input.GetAxis("Horizontal");
            rb2d.velocity = new Vector2(horizontalInput * speed, rb2d.velocity.y);
            if (rb2d.velocity.x < 0)
            {
                sprite.flipX = true;
            }
            else if (rb2d.velocity.x > 0)
            {
                sprite.flipX = false;
            }

            if (Input.GetKeyDown(KeyCode.Space) && grounded == false)
            {
                isJumping = false;
                rb2d.gravityScale = gravityFlying;
                Jump();
            }


            if (Input.GetKeyDown(KeyCode.Space) && grounded == true)
            {
                isJumping = true;
                Jump();
                jumpTimeCounter = jumpTime;

            }

            //Executado se a tecla de pulo continuar apertada
            if (Input.GetKey(KeyCode.Space) && isJumping == true)
            {
                //Quando o JumpCounter acabar o pulo termina
                if (jumpTimeCounter > 0)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce / jumpTimeDecrease);
                    jumpTimeCounter -= Time.deltaTime;
                }
                else { isJumping = false; }
            }

            if (isJumping == false)
            {
                isGrounded();
            }

            //Se o bot�o for soltado o pulo termina
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
            }
        }
        else
        {
            panel.SetActive(true);
        }
        //======================================================================================





    }
    //Jump
    //======================================================================================
    void Jump()
    {
        if (isJumping)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            grounded = false;
            jumpBufferTimerCounter = 0;
        }
        else
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, flyForce);
        }
    }
    //======================================================================================

    //Colis�o
    //======================================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "item")
        {
            Destroy(collision.gameObject);
            energyBar.GainEnergy(energyRestoren);
            points++;
        }
        if (collision.gameObject.tag == "end")
        {
            end = true;
            txt.text = "Coletou " + points + "/" + total + " latinhas.";
        }
        if (collision.gameObject.tag == "checkpoint")
        {
            
            scriptHandler.CheckpointHit(transform.position);
            Heal();
        }

    }
    //======================================================================================


    //Panel
    //======================================================================================
    public void Restart(string scene)
    {

        SceneManager.LoadScene(scene);
        end = false;
        points = 0;
    }



    public void Close()
    {
        Application.Quit();
    }
    //======================================================================================

    public void isGrounded()
    {
        Debug.DrawRay(transform.position, Vector2.down * 2f, Color.red);

        RaycastHit2D raycastHit = Physics2D.Raycast(this.transform.position, new Vector3(0, -1, 0), 0.2f, LayerMask.GetMask("Ground"));

        if (Physics2D.Raycast(this.transform.position, new Vector3(0, -1, 0), 2f, LayerMask.GetMask("Ground")))
        {
            grounded = true;
            jumpBufferTimerCounter = jumpBufferTimer;
            rb2d.gravityScale = gravityMove;
        }
        else
        {


            if (jumpBufferTimerCounter <= 0)
            {
                grounded = false;
            }

            else
            {
                jumpBufferTimerCounter -= Time.deltaTime;
                grounded = true;
            }
        }
    }

    public void TakeDamage()
    {
        energyBar.LoseEnergy(energyTaken);
    }

    public void Heal()
    {
        energyBar.SetMaxEnergy();
    }

}
