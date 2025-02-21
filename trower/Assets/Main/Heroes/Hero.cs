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
    HeroManager heroManager;
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
    int canMove;
    public bool moving;
    private bool walking;
    private bool climbing;

    GridSpace currentCell;
    Trap currentTrap;

    Vector2 targetSpace;
    Vector2 velocity;
    private void Awake()
    {
        currentHealth = maxHealth;
        heroFeet = gameObject.GetComponentInChildren<HeroFeet>();
        anim = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset);
        targetSpace = transform.position;
        currentStamina = maxStamina;
        canMove = 1;
    }

    public void Birth(HeroManager heroManager) //called by heromanager when it spawns a new hero
    {
        this.heroManager = heroManager; 
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
            velocity = (Vector2.right * speed * dir * canMove * Time.deltaTime);
            transform.Translate(velocity);
        }
        if (climbing)
        {
            anim.SetBool("Climbing", true);
            velocity = (Vector2.up * speed * dir * canMove * Time.deltaTime);
            transform.Translate(velocity);
        }
        if(canMove != 1)
        {
            if(currentTrap == null || currentTrap.TrapEffects(this))
            {
                Debug.Log("fartfart");
                canMove = 1;
            }
        }


        if (Mathf.Abs((currentPos - targetSpace).magnitude) <= .2f) //finish moving
        {
            gameObject.transform.position = targetSpace;
            anim.SetBool("Moving", false);
            anim.SetBool("Climbing", false);
            moving = false;
            walking = false;
            climbing = false;
            currentStamina--;
        }

        TryChangeCurrentCell(heroManager.GetClosestGridSpaceToHero(this));  
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


    private void TryChangeCurrentCell(GridSpace gridSpace) //gridspace is the closest cell to the hero
    {
        if(currentCell == gridSpace) { return; }

        string entryType = "";
        string exitType = "";

        //sets direction
        if(velocity.y > 0) { entryType = "enter_bottom"; exitType = "exit_top"; }
        else if(velocity.y < 0) { entryType = "enter_top"; exitType = "exit_bottom"; }
        else if(velocity.x > 0) { entryType = "enter_left"; exitType = "exit_right"; }
        else if(velocity.x < 0) { entryType = "enter_right"; exitType = "exit_left"; }

        foreach (TrapTrigger sensor in gridSpace.GetSensor()) //new cell (entering)
        {
            if (sensor.TriggerSensor(entryType))
            {
                currentTrap = sensor.GetTrapDaddy();
                canMove = 0;
            }
        }

        if(currentCell != null)
        {
            foreach (TrapTrigger sensor in currentCell.GetSensor()) //original cell (exiting)
            {
                if(sensor.TriggerSensor(exitType))
                {
                    currentTrap = sensor.GetTrapDaddy();
                    canMove = 0;
                }
            }
        }

        currentCell = gridSpace; //sets new cell
        currentCell.ad
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
