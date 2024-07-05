using System;
using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] float speed;
    WaveManager waveManager;
    HeroManager heroManager;
    Rigidbody2D rb;

    bool attackPhase;
    bool facingLeft;
    bool isFalling;
    float currentSpeed;
    private void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        heroManager = gameManager.GetComponent<HeroManager>();
        heroManager.AddHeroToList(gameObject);
        waveManager = gameManager.GetComponent<WaveManager>();
        waveManager.OnAttackPhaseStart += AttackPhase;
        waveManager.OnDefensePhaseStart += DefensePhase;
        waveManager.OnFinalWave += Surrender;
        rb = gameObject.GetComponent<Rigidbody2D>();
        currentSpeed = speed;
        if (gameObject.transform.position.x > 0)
        {
            facingLeft = true;
        }
        else
        {
            facingLeft = false;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.gameObject.layer == 11) //trap layer
    //    {
    //        heroManager.KillHero(gameObject);
    //    }
    //}

    private void Update()
    {
        if (attackPhase)
        {
            Vector2 dir = new Vector2(1, 0) * currentSpeed;
            if (facingLeft)
            {
                dir *= -1;
            }

            rb.velocity = dir;

        }
    }

    public void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void AttackPhase()
    {
        attackPhase = true;
        StartCoroutine(RandomDelay());
    }
    public void DefensePhase()
    {
        attackPhase = false;
        currentSpeed = 0;
        rb.velocity = Vector2.zero;
        gameObject.GetComponent<Animator>().SetBool("AttackPhase", false);
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
        currentSpeed = speed;
        gameObject.GetComponent<Animator>().SetBool("AttackPhase", true);
    }

    public bool GetFallingStatus()
    {
        return isFalling;
    }

    public void Surrender(object sender, EventArgs e)
    {
        //add something cooler here later
        KillMe();
    }

    public void KillMe()
    {
        Debug.Log("killed me");
        heroManager.KillHero(gameObject);
    }
    private void OnDestroy()
    {
        waveManager.OnAttackPhaseStart -= AttackPhase;
        waveManager.OnDefensePhaseStart -= DefensePhase;
        waveManager.OnFinalWave -= Surrender;
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
