using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject camera;
    Rigidbody2D rb2d;
    public GameObject panel;
    public Text txt;
    SpriteRenderer sprite;
    public EnergyBar energyBar;
    public ScriptHandler scriptHandler;
    public SpriteRenderer arrowLeft;
    public SpriteRenderer arrowRight;
    public SpriteRenderer buttonE;
    private GameObject enemy;
    private Animator anim;
    public Transform missileGoPos;
    public GameObject missilePrefab;
    private AudioSource audioSource;
    public AudioClip damagedClip;
    public AudioClip colisionClip;
    public AudioClip colectClip;

    [Header("Energia")]
    public float maxEnergy;
    private float currentEnergy;
    public float energyTaken;
    public float energyRestoren;
    public float loseEnergyByTime;

    [Header("Points")]
    public int points;
    public int total;
    public bool end;

    [Header("Movement Florest")]
    public float speed;
    float horizontalInput;
    float verticalInput;
    private int playerDirection;
    private Vector2 oldVelocity;

    [Header("Movement Water")]
    public float speedWater, fspeed;
    private bool direction;

    [Header("Movement City")]
    public float flyForce;
    public float gravityFlying;
    public float gravityMove;
    public bool isFlying;
    public float energyTakenFly;

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

    [Header("Others")]
    private float spriteChange = 0.1f;
    public float timerHit;
    bool hit = false;
    float timer = 0.1f;
    private bool isCatch = false;
    public int raiaBreakFree = 10;
    private int raiaBreakFreeCurrent;
    private int playerState;
    private bool canEnterLevel = false;
    private int levelSelected;
    public bool isAttacking = false;
    public float missileCooldownMax = 2f;
    private float missileCoooldownTimer = 0f;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        currentEnergy = maxEnergy;
        if (energyBar != null)
        {
            energyBar.MaxEnergy(maxEnergy);
            energyBar.SetMaxEnergy();
            energyBar.setEnergyTimer(loseEnergyByTime);
        }
        points = 0;
        total = 37;
        end = false;
        scriptHandler.CheckpointHit(transform.position);
        raiaBreakFreeCurrent = raiaBreakFree;
    }

    void Update()
    {
        //Detecta fase e muda o playerState para a movimentação correta
        playerState = scriptHandler.GetGameState();
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    TakeDamage();
        //}

        //Decide qual movimentação usar dependendo da fase
        switch (playerState)
        {
            case 0:
                Menu();
                break;

            case 1:
                Floresta();
                break;

            case 2:
                Oceano();
                break;

            case 3:
                Cidade();
                break;
        }
    }

    void FixedUpdate()
    {
        oldVelocity = rb2d.velocity;
    }

    void Menu()
    {
        camera.transform.position = new Vector3(transform.position.x, 0, -10);
        if (camera.transform.position.x <= -1.25f)
        {
            camera.transform.position = new Vector3(-1.25f, 0, -10);
        }
        else if (camera.transform.position.x >= 1.9f)
        {
            camera.transform.position = new Vector3(1.9f, 0, -10);
        }

        horizontalInput = Input.GetAxis("Horizontal");
        rb2d.velocity = new Vector2(horizontalInput * speed, rb2d.velocity.y);
        if (rb2d.velocity.x < 0)
        {
            anim.Play("robot_walk");
            sprite.flipX = true;
            playerDirection = 1;
        }
        else if (rb2d.velocity.x > 0)
        {
            anim.Play("robot_walk");
            sprite.flipX = false;
            playerDirection = -1;
        }

        else
        {
            anim.Play("robot_idle");
        }

        if (Input.GetKeyDown(KeyCode.E) && canEnterLevel)
        {
            SceneManager.LoadScene(levelSelected);
        }

        if (Input.GetKeyDown(KeyCode.Space) && grounded == true)
        {
            isJumping = true;
            Jump();
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        if (isJumping == false)
        {
            isGrounded();
        }
    }

    void Floresta()
    {
        //Camera
        //======================================================================================
        camera.transform.position = new Vector3(transform.position.x, transform.position.y - 2, -10);
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
            if (rb2d.velocity.x < 0 && isAttacking == false)
            {
                anim.Play("robot_walk");
                sprite.flipX = true;
                playerDirection = 1;
            }
            else if (rb2d.velocity.x > 0 && isAttacking == false)
            {
                anim.Play("robot_walk");
                sprite.flipX = false;
                playerDirection = -1;
            }

            else if(isAttacking == false)
            {
                anim.Play("robot_idle");
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
            
            if(hit)
            {
                Ivencibility();
            }

            if(Input.GetKeyUp(KeyCode.E))
            {
                isAttacking = true;
                anim.Play("robot_kick");
                //raycasts do ataque
                if(playerDirection == 1)
                {
                    Debug.DrawRay(transform.position - new Vector3(0,1.3f,0), Vector3.left * 3f, Color.green);
                    RaycastHit2D raycastHit = Physics2D.Raycast(this.transform.position - new Vector3(0,1.3f,0), Vector3.left, 4f, LayerMask.GetMask("Weakness"));
                    if (raycastHit)
                    {
                        raycastHit.collider.transform.parent.gameObject.GetComponent<Enemy>().GotStuned();             
                    }    
                }
                else
                {
                    Debug.DrawRay(transform.position - new Vector3(0,1.3f,0), Vector3.right * 3f, Color.green);
                    RaycastHit2D raycastHit = Physics2D.Raycast(this.transform.position - new Vector3(0,1.3f,0), Vector3.right, 4f, LayerMask.GetMask("Weakness"));
                    if (raycastHit)
                    {
                        raycastHit.collider.transform.parent.gameObject.GetComponent<Enemy>().GotStuned();             
                    }    
                } 
            }
        }
        else
        {
            panel.SetActive(true);
        }
        //======================================================================================
    }

    void Oceano()
    {
        if (Input.GetButtonDown("Fire1") && scriptHandler.isGamePaused == false && missileCoooldownTimer <= 0)
        {
            missileCoooldownTimer = missileCooldownMax;
            Vector3 mousePos = Input.mousePosition;
            GameObject missile = Instantiate(missilePrefab, this.transform.position, Quaternion.identity);
            missile.GetComponent<Missile>().UpdatePos(missileGoPos);
            missile.GetComponent<Missile>().mousePosition = mousePos;
        }
        missileCoooldownTimer = missileCoooldownTimer - Time.deltaTime;

        anim.Play("robot_ocean");
        //caso tenha sido pega pela raia
        if (isCatch)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");

            if (direction && horizontalInput > 0)
            {
                direction = false;
                raiaBreakFreeCurrent = raiaBreakFreeCurrent - 1;
                arrowLeft.enabled = true;
                arrowRight.enabled = false;
                transform.position = new Vector2(transform.position.x + 0.5f, transform.position.y);
            }
            else if (direction == false && horizontalInput < 0)
            {
                direction = true;
                raiaBreakFreeCurrent = raiaBreakFreeCurrent - 1;
                arrowLeft.enabled = false;
                arrowRight.enabled = true;
                transform.position = new Vector2(transform.position.x - 0.5f, transform.position.y);
            }

            if (raiaBreakFreeCurrent <= 0)
            {
                isCatch = false;
                enemy.GetComponent<EPlaticRay>().Uncatch();
                raiaBreakFreeCurrent = raiaBreakFree;
                arrowLeft.enabled = false;
                arrowRight.enabled = false;
                direction = true;
            }
        }
        else
        {  
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            rb2d.AddForce(new Vector2(horizontalInput, verticalInput).normalized * speedWater * Time.deltaTime * 100);

            //Movimento normal
            if (hit == false)
            {
                transform.position = new Vector2(transform.position.x + fspeed * Time.deltaTime, transform.position.y);
                camera.transform.position = new Vector3(camera.transform.position.x + fspeed * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
            }
            //Hit
            else
            {
                //Codigo para camera voltar a andar sem precisar esperar o tempo completo da invencibilidade
                if (timer < 1.5f)
                {
                    transform.position = new Vector2(transform.position.x - (fspeed / timer) / 3 * Time.deltaTime, transform.position.y);
                    camera.transform.position = new Vector3(camera.transform.position.x - (fspeed / timer) / 3 * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
                }
                else
                {
                    transform.position = new Vector2(transform.position.x + fspeed * Time.deltaTime, transform.position.y);
                    camera.transform.position = new Vector3(camera.transform.position.x + fspeed * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
                }
                Ivencibility();
            }
            //=============================================================================================
        }
    }

    void Cidade()
    {
        
        //Camera
        //======================================================================================
        camera.transform.position = new Vector3(transform.position.x, transform.position.y + 2, -10);
        if (camera.transform.position.x <= 1)
        {
            camera.transform.position = new Vector3(1, camera.transform.position.y, -10);
        }
        else if (camera.transform.position.x >= 667)
        {
            camera.transform.position = new Vector3(667, camera.transform.position.y, -10);
        }
        if (camera.transform.position.y <= -4)
        {
            camera.transform.position = new Vector3(camera.transform.position.x, -4, -10);
        }
        else if (camera.transform.position.y >= 30)
        {
            camera.transform.position = new Vector3(camera.transform.position.x, 30, -10);
        }
        //======================================================================================

        //Movimento
        //======================================================================================
        if (!end)
        {
            panel.SetActive(false);
            horizontalInput = Input.GetAxis("Horizontal");
            rb2d.velocity = new Vector2(horizontalInput * speed, rb2d.velocity.y);
            if (rb2d.velocity.x < 0 && isAttacking == false && isFlying == false)
            {
                anim.Play("robot_walk");
                sprite.flipX = true;
                playerDirection = 1;
            }
            else if (rb2d.velocity.x > 0 && isAttacking == false && isFlying == false)
            {
                anim.Play("robot_walk");
                sprite.flipX = false;
                playerDirection = -1;
            }

            else if(isAttacking == false && isFlying == false)
            {
                anim.Play("robot_idle");
            }

            else if (isFlying)
            {
                anim.Play("robot_fly");
            }

            if (Input.GetKeyDown(KeyCode.Space) && grounded == false)
            {
                isFlying = true;
                JumpCity();
            }

            else if (Input.GetKeyDown(KeyCode.Space) && grounded == true)
            {
                isJumping = true;
                JumpCity();
                jumpTimeCounter = jumpTime;

            }
            if(transform.position.y <= camera.transform.position.y - 15)
            {
                energyBar.LoseEnergy(100);
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
        if (hit)
        {
            Ivencibility();
        }
    }

    void Jump()
    {
        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
        grounded = false;
        jumpBufferTimerCounter = 0;
    }

    void JumpCity()
    {
        if (isFlying)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, flyForce);
            rb2d.gravityScale = gravityFlying;
            energyBar.LoseEnergy(energyTakenFly);
        }
        else if (isJumping)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
            grounded = false;
            jumpBufferTimerCounter = 0;
        }  
    }
    //Colis�o
    //======================================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "inimigo":
                TakeDamage();
                break;

            case "item":
                rb2d.velocity = oldVelocity;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "item":
                //Codigo está no tilemap de itens
                //gotItem();
                break;

            case "end":
                end = true;
                txt.text = "Coletou " + points + "/" + total + " latinhas.";
                break;

            case "checkpoint":
                scriptHandler.CheckpointHit(transform.position);
                Heal();
                break;

            case "obstaculo":
                audioSource.PlayOneShot(colisionClip);
                TakeDamage();
                break;

            case "raia":
                if (hit == false && isCatch == false)
                {
                    enemy = collision.gameObject;
                    enemy.GetComponent<EPlaticRay>().Catch();
                    isCatch = true;
                    arrowLeft.enabled = true;
                }
                break;

            case "inimigo":
                //if (playerState == 1)
                //{
                //    Vector3 hitVector = new Vector3(transform.position.x - collision.transform.position.x, 1, 0).normalized;
                //    rb2d.AddForce(hitVector * 1000);
                //}
                audioSource.PlayOneShot(damagedClip);
                TakeDamage();
                break;

            case "projectile":
                audioSource.PlayOneShot(damagedClip);
                TakeDamage();
                break;

            case "fimFase":
                SceneManager.LoadScene(0);
                break;

            case "EntrarFase":
                switch(collision.gameObject.name)
                {
                    case "EntrarFloresta":
                        levelSelected = 1;
                        break;

                    case "EntrarOceano":
                        levelSelected = 2;
                        break;

                    case "EntrarCidade":
                        levelSelected = 3;
                        break;
                }
                buttonE.enabled = true;
                canEnterLevel = true;
                break;
        }
    }
    //======================================================================================

    void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "EntrarFase":
                buttonE.enabled = false;
                canEnterLevel = false;
                break;
        }
    }

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
            isFlying = false;
            jumpBufferTimerCounter = jumpBufferTimer;
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
        if (hit == false)
        {
            energyBar.LoseEnergy(energyTaken);
            hit = true;
        }
    }

    public void Heal()
    {
        energyBar.SetMaxEnergy();
    }

    private void Ivencibility()
    {
        timer = timer + 1 * Time.deltaTime;
        spriteChange = spriteChange - 1 * Time.deltaTime;

        if (spriteChange <= 0)
        {
            spriteChange = 0.1f;
            if (sprite.enabled == true)
            {
                sprite.enabled = false;
            }
            else
            {
                sprite.enabled = true;
            }
        }

        if (timer > timerHit)
        {
            hit = false;
            timer = 0.1f;
            sprite.enabled = true;
        }
    }

    public void gotItem()
    {
        audioSource.PlayOneShot(colectClip);
        energyBar.GainEnergy(energyRestoren);
        points++;
    }
}
