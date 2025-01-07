using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour, IDamageable, IBurnable
{
    [SerializeField] int maxStamina;
    [SerializeField] int speed;
    [SerializeField] float zOffset;
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
    float onFireDuration;
    float fireDamageInterval;
    float fireDamage;
    float currentHealth;
    int currentStamina;
    int distFromGrid;
    int dir;
    public bool moving;
    private bool walking;
    private bool climbing;

    GridSpace currentCell;

    Vector2 targetSpace;
    private void Awake()
    {
        currentHealth = maxHealth;
        heroFeet = gameObject.GetComponentInChildren<HeroFeet>();
        anim = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset);
        targetSpace = transform.position;
        currentStamina = maxStamina;
    }

    public void FlipRight()
    {
        spriteRenderer.flipX = true;
        dir = 1;
    }
    public void FlipLeft()
    {
        spriteRenderer.flipX = false;
        dir = -1;
    }

    public void Move(Vector2 targetMove)
    {
        if (currentCell != null && targetMove.y != currentCell.GetPos().y)
        {
            climbing = true;
        }
        else
        {
            walking = true;
        }
        moving = true;
        targetSpace = targetMove;
    }

    private void Update()
    {
        if (!moving) { return; }
        Vector2 currentPos = gameObject.transform.position;

        if (targetSpace.x < currentPos.x) { FlipLeft(); }
        else { FlipRight(); }

        if (walking)
        {
            anim.SetBool("Moving", true);
            transform.Translate(Vector2.right * speed * dir * Time.deltaTime);
        }
        if (climbing)
        {
            anim.SetBool("Climbing", true);
            transform.Translate(Vector2.up * speed * dir * Time.deltaTime);
        }


        if (Mathf.Abs((currentPos - targetSpace).magnitude) <= .2f)
        {
            gameObject.transform.position = targetSpace;
            anim.SetBool("Moving", false);
            anim.SetBool("Climbing", false);
            moving = false;
            walking = false;
            climbing = false;
            currentStamina--;
        }
    }

    public bool GetIsMoving()
    {
        return moving;
    }

    public int GetCurrentStamina()
    {
        return currentStamina;
    }

    public void Restore()
    {
        currentStamina = maxStamina;
    }

    public int GetDistanceFromGrid()
    {
        return distFromGrid;
    }

    public void GetCloserToGrid()
    {
        if (distFromGrid > 0) { distFromGrid -= 1; }
        if (distFromGrid < 0) { distFromGrid += 1; }
    }

    public void SetDistFromGrid(int dist)
    {
        distFromGrid = dist;
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
    private IEnumerator RandomDelay()
    {
        float seconds = UnityEngine.Random.Range(1, 101);
        seconds = seconds / 100;
        if (seconds > .5f)
        {
            seconds -= .5f;
        }
        yield return new WaitForSeconds(seconds);
    }

    public bool GetFallingStatus()
    {
        return false;
    }


    public void SetCurrentCell(GridSpace gridSpace)
    {
        currentCell = gridSpace;
        //set grid enemy here
    }

    public GridSpace GetCurrentCell()
    {
        return currentCell;
    }

    public void KillMe()
    {
        Debug.Log("killed me");
        paralyzed = true;
        heroManager.KillHero(gameObject);
    }
    public void Damage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            KillMe();
        }
        if (amount > 0)
        {
            flash.Flash();
        }
        Debug.Log("IM BURNING AHHH" + fireDamage + " | " + fireDamageInterval + " | " + onFireDuration + " | Remaining Health" + currentHealth);
    }

    public void Burn(float length, float damage, float damageInterval)
    {
        if (!canBurn) return;
        onFireDuration = length;
        fireDamageInterval = damageInterval;
        fireDamage = damage;
    }
}
