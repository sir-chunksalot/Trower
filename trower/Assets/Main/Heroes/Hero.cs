using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour, IDamageable, IBurnable
{
    [SerializeField] float speed;
    [SerializeField] float zOffset;
    [SerializeField] float offsetLength;
    [SerializeField] bool faceRight;
    [SerializeField] bool paralyzed;
    [SerializeField] bool canFall;
    [SerializeField] bool canBurn;
    [SerializeField] float maxHealth;

    FlashEffect flash;
    WaveManager waveManager;
    HeroManager heroManager;
    Rigidbody2D rb;
    HeroFeet heroFeet;

    Animator anim;
    SpriteRenderer spriteRenderer;

    Vector3 doorPos;

    List<GameObject> touchedLadders;
    float onFireDuration;
    float fireDamageInterval;
    float fireDamage;
    float timePassed;
    bool attackPhase;
    bool facingLeft;
    bool isSuperFalling;
    float superFallDir;
    bool isTouchingGround;
    bool isFalling;
    int fallCount;
    bool isClimbing;
    bool canClimb;
    bool smashBros;
    bool gameOver;
    float currentSpeed;
    float rotation;
    float currentHealth;
    private void Awake()
    {
        rotation = 15;
        currentHealth = maxHealth;
        touchedLadders = new List<GameObject>();
        heroFeet = gameObject.GetComponentInChildren<HeroFeet>();
        anim = gameObject.GetComponent<Animator>();
        facingLeft = true;
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        heroManager = gameManager.GetComponent<HeroManager>();
        waveManager = gameManager.GetComponent<WaveManager>();
        waveManager.OnAttackPhaseStart += AttackPhase;
        waveManager.OnDefensePhaseStart += DefensePhase;
        waveManager.OnFinalWave += Surrender;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        flash = gameObject.GetComponent<FlashEffect>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        currentSpeed = speed;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset);
        StartCoroutine(OffsetLength());
        if (faceRight)
        {
            FlipRight();
        }


    }

    private void Start()
    {
        Debug.Log("gameManagerrrr" + heroManager);
        heroManager.AddHeroToList(gameObject);
    }

    private IEnumerator OffsetLength()
    {
        canClimb = false;
        yield return new WaitForSeconds(offsetLength);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - zOffset);
        canClimb = true;
    }
    private void FixedUpdate()
    {
        if(onFireDuration >= 0)
        {
            timePassed += Time.deltaTime;
            onFireDuration -= Time.deltaTime;
            if(timePassed >= fireDamageInterval)
            {
                timePassed = 0;
                Damage(fireDamage);
            }
        }
        if(gameOver)
        {  
            RotateMe();
            rb.AddForce((doorPos - gameObject.transform.position).normalized * 10);
        }
        else if (smashBros)
        {
            rb.velocity = Vector2.zero;
            RotateMe(); 
            gameObject.transform.localScale += new Vector3(.04f, .04f, 00);
        }
        else if (isSuperFalling)
        {
            if (rb.transform.position.y >= heroManager.TopOfMap() + 5)
            {
                superFallDir = -1;
                rb.velocity = rb.velocity * superFallDir;
            }
            if (transform.position.y <= 0)
            {
                isSuperFalling = false;
                rb.velocity = Vector2.zero;
                UpdateMove();
            }
        }
        else if (GetIsTouchingGround())
        {
            if (isFalling)
            {
                isFalling = false;
                UpdateMove();
            }
        }
        else
        {
            if (!isFalling && canFall)
            {
                fallCount++;
                if (fallCount < 5) return;
                fallCount = 0;
                isFalling = true;
                UpdateMove();
            }

        }
    }

    public void RotateMe()
    {
        gameObject.transform.Rotate(0, 0, Mathf.Abs(rotation));
    }

    public void FlipRight()
    {
        facingLeft = false;
        gameObject.GetComponent<SpriteRenderer>().flipX = true;
    }
    public void FlipLeft()
    {
        facingLeft = true;
        gameObject.GetComponent<SpriteRenderer>().flipX = false;
    }

    private void UpdateMove()
    {

        Debug.Log("UpdateMove");
        if (paralyzed)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (isSuperFalling)
        {
            SuperFall();
            anim.SetBool("Falling", true);
            return;
        }
        else { anim.SetBool("Falling", false); }

        if (isClimbing)
        {
            Climb();
            anim.SetBool("Climbing", true);
            return;
        }
        else { anim.SetBool("Climbing", false); }

        if (isFalling)
        {
            Fall();
            anim.SetBool("Falling", true);
            return;
        }
        else { anim.SetBool("Falling", false); }

        if (attackPhase)
        {
            Walk();
            anim.SetBool("AttackPhase", true);
            return;
        }
        else { Idle(); }
    }
    private void Walk()
    {
        float goLeft = 1;
        if (facingLeft)
        {
            goLeft = -1;
        }

        rb.velocity = new Vector2(1, 0) * currentSpeed * goLeft;
    }
    private void Idle()
    {
        rb.velocity = Vector2.zero;
        anim.SetBool("Falling", false);
        anim.SetBool("Climbing", false);
        anim.SetBool("AttackPhase", false);
    }
    private void Climb()
    {
        rb.velocity = new Vector2(0, 1) * currentSpeed;
    }

    private void Fall()
    {
        rb.velocity = new Vector3(0, -1) * 10;
    }

    private void SuperFall()
    {
        rb.velocity = new Vector2(0, 1) * 10 * superFallDir;
    }

    private bool GetIsTouchingGround()
    {
        if (heroFeet != null)
        {
            return heroFeet.GetIsTouchingGround();
        }
        else
        {
            return false;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameOver) return;
        Debug.Log("FLUMBERG hero hit " + collision.gameObject);
        if (collision.gameObject.tag == "EnemyWall")
        {
            if (facingLeft)
            {
                FlipRight();
            }
            else
            {
                FlipLeft();
            }

            UpdateMove();

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameOver) return;
        if (collision.gameObject.tag == "Ladder" && collision.gameObject.transform.position.y > transform.position.y && canClimb)
        {
            if (!touchedLadders.Contains(collision.gameObject))
            {
                touchedLadders.Add(collision.gameObject);
            }

            isClimbing = true;
            UpdateMove();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameOver) return;
        if (collision.gameObject.tag == "Ladder")
        {
            touchedLadders.Remove(collision.gameObject);
            if (touchedLadders.Count == 0)
            {
                isClimbing = false;
                UpdateMove();
            }
        }
    }




    public void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void AttackPhase()
    {
        attackPhase = true;
        currentSpeed = speed;
        StartCoroutine(RandomDelay());
    }
    public void DefensePhase()
    {
        attackPhase = false;
        UpdateMove();
        currentSpeed = 0;
        rb.velocity = Vector2.zero;
    }
    private void AttackPhase(object sender, EventArgs e)
    {
        AttackPhase();
    }

    private void DefensePhase(object sender, EventArgs e)
    {
        DefensePhase();
    }

    private IEnumerator RandomDelay()
    {
        float seconds = UnityEngine.Random.Range(1, 101);
        seconds = seconds / 100;
        if (seconds > .5f)
        {
            seconds -= .5f;
        }
        yield return new WaitForSeconds(seconds);
        UpdateMove();
        currentSpeed = speed;
    }
    public void StartSuperFall(float dir, bool smashBros)
    {
        isSuperFalling = true;
        this.superFallDir = dir;
        if (smashBros)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            rb.velocity = Vector2.zero;
            this.smashBros = smashBros;
            StartCoroutine(SmashBros());
        }
        UpdateMove();
    }

    private IEnumerator SmashBros()
    {
        //anim.SetBool("Falling", y);
        yield return new WaitForSeconds(1f);

        KillMe();
    }

    public void Stun(float stunTime)
    {
        currentSpeed = 0;
        StartCoroutine(StunTime(stunTime));
    }

    private IEnumerator StunTime(float time)
    {
        yield return new WaitForSeconds(time);
        currentSpeed = speed;
    }
    public bool GetFallingStatus()
    {
        return isSuperFalling;
    }

    public void Surrender(object sender, EventArgs e)
    {
        rb.velocity = Vector2.zero;
        doorPos = heroManager.GetDoorPos();
        gameOver = true;
        heroFeet.gameObject.SetActive(false);
        rb.isKinematic = false;
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    //private IEnumerator Test()
    //{
    //    yield return new WaitForSeconds(.2f);
        
    //    StartCoroutine(Test());
    //}


    public void KillMe()
    {
        Debug.Log("killed me");
        paralyzed = true;
        heroManager.KillHero(gameObject);
    }
    private void OnDestroy()
    {
        waveManager.OnAttackPhaseStart -= AttackPhase;
        waveManager.OnDefensePhaseStart -= DefensePhase;
        waveManager.OnFinalWave -= Surrender;
    }

    public void Damage(float amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            KillMe();
        }
        if(amount > 0)
        {
            flash.Flash();
        }
        Debug.Log("IM BURNING AHHH" + fireDamage + " | " + fireDamageInterval + " | "+ onFireDuration + " | Remaining Health" + currentHealth);
    }

    public void Burn(float length, float damage, float damageInterval)
    {
        if (!canBurn) return;
        onFireDuration = length;
        fireDamageInterval = damageInterval;
        fireDamage = damage;
    }





















    //[SerializeField] float speed;
    //[SerializeField] float difficulty;
    //[SerializeField] float spawnWeight;
    //[SerializeField] float skullForce;
    //[SerializeField] bool hasArmor;
    //[SerializeField] GameObject skull;
    //[SerializeField] float healthNumber;
    //[SerializeField] Material regularMat;
    //[SerializeField] Material whiteMat;
    //[SerializeField] bool canWin;
    //[SerializeField] Vector3 pos;
    //[SerializeField] bool canAttack;
    //SpriteRenderer sprite;
    //Rigidbody2D rb;
    //Vector2 force;
    //int[] randomDir = new int[2] { -1, 1 };
    //bool falling;
    //bool climbingLadder;
    //private float health;

    //List<GameObject> exitLadders;
    //List<GameObject> blackListedLadders;

    //Vector3 rememberDir;

    //HeroSpawner heroSpawner;
    //GenerateViableFloors generateViableFloors;

    //void Start()
    //{
    //    generateViableFloors = gameObject.GetComponentInParent<GenerateViableFloors>();
    //    exitLadders = new List<GameObject>();
    //    blackListedLadders = new List<GameObject>();

    //    transform.position = new Vector3(transform.position.x + pos.x, transform.position.y + pos.y, transform.position.z + pos.z);
    //    health = healthNumber;
    //    falling = false;
    //    climbingLadder = false;

    //    sprite = gameObject.GetComponent<SpriteRenderer>();
    //    sprite.material = regularMat;
    //    heroSpawner = gameObject.GetComponentInParent<HeroSpawner>();
    //    falling = false;
    //    Random.InitState(System.DateTime.Now.Millisecond);

    //    rb = gameObject.GetComponent<Rigidbody2D>();

    //    force = Vector2.right * speed * (Mathf.Sign(transform.position.x) * -1);
    //    rb.velocity = force;

    //    //StartCoroutine(WorkAroundForWallCollissionOnSpawn());
    //}

    //private void Update()
    //{
    //    if (rb.velocity.normalized.x == 1)
    //    {
    //        sprite.flipX = true;
    //    }
    //    else if (rb.velocity.normalized.x == -1)
    //    {
    //        sprite.flipX = false;
    //    }
    //    if (GameEnd.GetWinStatus())
    //    {
    //        force = Vector2.zero;
    //        speed = 0;
    //        rb.velocity = Vector2.zero;
    //    }
    //    Debug.Log(rb.velocity);

    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "EnemyWall")
    //    {
    //        force = force * -1;
    //        rb.velocity = force;
    //        Debug.Log("wall moment");
    //    }

    //    if (collision.gameObject.tag == "EnemyFloor")
    //    {
    //        if (falling)
    //        {
    //            rb.velocity = force;
    //            falling = false;
    //            Debug.Log("floor moment");
    //        }
    //    }

    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    //if (!falling)
    //    //{
    //    //    Debug.Log(collision.tag + "BOOGER");
    //    //    if (collision.tag == "Ladder" && !blackListedLadders.Contains(collision.gameObject))
    //    //    {
    //    //        climbingLadder = true;
    //    //        rb.velocity = Vector2.up * speed;
    //    //    }
    //    //    if (collision.tag == "TopOfLadder")
    //    //    {
    //    //        if (!generateViableFloors.GetViableLadders().Contains(collision.gameObject)) //checks to see if this ladder is viable
    //    //        {
    //    //            if (!exitLadders.Contains(collision.gameObject)) //first time, do nothing
    //    //            {
    //    //                exitLadders.Add(collision.gameObject);
    //    //                rememberDir = force;
    //    //            }
    //    //            else//second time, jump down
    //    //            {
    //    //                Fall();
    //    //                climbingLadder = false;
    //    //                exitLadders.Clear();

    //    //                force = rememberDir;

    //    //                blackListedLadders.Add(collision.gameObject.GetComponentInParent<LadderEnable>().gameObject);
    //    //                Debug.Log(collision.gameObject.GetComponentInParent<LadderEnable>().gameObject + "sleep");
    //    //            }

    //    //        }
    //    //    }

    //    //    if (collision.tag == "Web")
    //    //    {
    //    //        rb.velocity = Vector2.zero;
    //    //    }

    //    //    if (collision.tag == "Entrance")
    //    //    {
    //    //        if (canWin)
    //    //        {
    //    //            Debug.Log("Hero escaped!");
    //    //            GameEnd.SetWinStatus(false);

    //    //            SceneManager.LoadScene(3);
    //    //        }

    //    //    }

    //    //    if (collision.tag == "TrapRoom" && canAttack)
    //    //    {
    //    //        Debug.Log("TRAPPY!!");
    //    //        gameObject.GetComponent<Animator>().SetBool("FoundTarget", true);
    //    //    }

    //    //}
    //}

    //private void OnDestroy()
    //{
    //    if (gameObject.scene.isLoaded) //Was killed
    //    {
    //        Debug.Log("ded");
    //        SpawnSkull();
    //        heroSpawner.HeroDied(gameObject);
    //    }
    //    else //Was Cleaned Up on Scene Closure
    //    {
    //        Debug.Log("goodbye sweetums");
    //    }
    //}

    //private void SpawnSkull()
    //{
    //    Quaternion rot = Quaternion.Euler(0, 0, Random.Range(1, 360));
    //    GameObject newSkull = Instantiate(skull, gameObject.transform.position, rot);
    //    newSkull.GetComponent<Rigidbody2D>().AddForce(skullForce * (Vector2.up) + (Random.Range(-1 * skullForce, skullForce) * Vector2.right));
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (!falling)
    //    {
    //        if (collision.tag == "Ladder" || collision.tag == "R_Ladder" || collision.tag == "L_Ladder")
    //        {
    //            if (climbingLadder)
    //            {
    //                Debug.Log("no more ladder");
    //                climbingLadder = false;
    //                rb.velocity = Vector2.right * speed * randomDir[Random.Range(0, 2)];
    //                force = rb.velocity;
    //            }
    //        }

    //        if (collision.tag == "Web")
    //        {
    //            rb.velocity = force;
    //        }

    //        if (collision.tag == "TrapRoom" && canAttack)
    //        {
    //            Debug.Log("TRAPPY!!");
    //            gameObject.GetComponent<Animator>().SetBool("FoundTarget", false);
    //        }
    //    }
    //}

    //public void Fall() //called by smack when door opens
    //{
    //    rb.velocity = Vector2.down * speed * 5;
    //    falling = true;
    //}


    //public bool GetFallingStatus()
    //{
    //    return falling;
    //}

    //public float GetDifficulty()
    //{
    //    return difficulty;
    //}

    //public float GetSpawnWeight()
    //{
    //    return spawnWeight;
    //}

    //public bool GetLadderStatus()
    //{
    //    return climbingLadder;
    //}

    //public float GetHealth()
    //{
    //    return health;
    //}
    //public void ChangeHealth(float value, bool isFire)
    //{
    //    if (hasArmor && !isFire) return;
    //    Debug.Log(health);
    //    health = health + value;
    //    if (sprite.material != whiteMat)
    //    {
    //        StartCoroutine(Flash());
    //    }

    //    if (health <= 0)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //private IEnumerator Flash()
    //{
    //    Debug.Log("flashing");
    //    sprite.material = whiteMat;
    //    yield return new WaitForSeconds(.4f);
    //    sprite.material = regularMat;
    //    Debug.Log("doneFlashing");

    //}

}
