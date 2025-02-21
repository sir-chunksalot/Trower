using Array2DEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("TrapVariables")]
    [SerializeField] Array2DGameObject trapComponents;

    [System.Serializable]
    public class TrapDetails
    {
        [SerializeField] public Array2DBool range; //center is trap location
        [SerializeField] public float damageAmount;
        [SerializeField] public bool stoppedByDoors;

        [SerializeField] public GameObject projectile;
        [SerializeField] public Vector3 projectileSpawnPos;

        [SerializeField] public Vector3 offset; //trap builder uses this
        [SerializeField] public bool isWallTrap;
    }
    [SerializeField] private TrapDetails trapDetails;




    [System.Serializable]
    public class TrapData
    {
        [SerializeField] public Sprite pressSpace;
        [SerializeField] public Sprite pressedSpace;
        [SerializeField] public GameObject armedSprite;
        [SerializeField] public GameObject trapObj;
        [SerializeField] public GameObject trapSprite;
        [SerializeField] public Animator anim;
    }
    [SerializeField] private TrapData trapData;

    StatusEffect statusEffect;
    GridManager gridManager;
    Dictionary<Vector2Int, TrapTrigger> trapSpawns = new Dictionary<Vector2Int, TrapTrigger>(); //anything in here is considered apart of the trap (trip wire hooks, pressure plates, etc)
    List<GridSpace> range = new List<GridSpace>(); //anything in here is within the range for the trap to attack
    int objectID;
    bool attacking;


    private void Awake()
    {
        statusEffect = gameObject.GetComponent<StatusEffect>();
        gridManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GridManager>();
        objectID = gameObject.GetInstanceID();
        Debug.Log("gameObject parent " + gameObject + "id" + objectID);

        int xBounds = trapComponents.GridSize.x;
        int yBounds = trapComponents.GridSize.y;


        ////sets trap size as governed by trap components. this is used by trap builder script to decide how much room this trap needs
        for (int y = 0; y < yBounds; y++)
        {
            for (int x = 0; x < xBounds; x++)
            {
                Vector2Int indexPos = new Vector2Int(x - (xBounds/2), (y - (xBounds / 2)) * -1); //centers the crossbow

                GameObject spawn = trapComponents.GetCell(x, y);
                if (spawn == null) { trapSpawns.Add(indexPos, null); }
                else  { trapSpawns.Add(indexPos, trapComponents.GetCell(x, y).GetComponent<TrapTrigger>()); }
            }
        }


        xBounds = trapDetails.range.GridSize.x;
        yBounds = trapDetails.range.GridSize.y;


        //sets the range of the trap
        for (int y = 0; y < yBounds; y++)
        {
            for (int x = 0; x < xBounds; x++)
            {
                Vector2Int relativeIndex = new Vector2Int(x - (xBounds / 2), (y - (xBounds / 2)) * -1); //centers the crossbow

                bool withinRange = trapDetails.range.GetCell(x, y);
                GridSpace currentGrid = gridManager.GetClosestGridSpace(gameObject.transform.position, false);
                Vector2Int index = new Vector2Int(currentGrid.GetIndex().x + relativeIndex.x, currentGrid.GetIndex().y + relativeIndex.y);
                if(withinRange) { range.Add(gridManager.GetGridSpace(index.x, index.y)); }
            }
        }
    }

    public Vector2Int[] GetTrapSize()
    {
        List<Vector2Int> takenCells = new List<Vector2Int>();
        foreach(KeyValuePair<Vector2Int, TrapTrigger> spawn in trapSpawns)
        {
            if(spawn.Value == null) { continue; }
            Vector2Int pos = spawn.Key;
            takenCells.Add(pos);
        }
        return takenCells.ToArray();
    }

    public void UseTrap()
    {
        attacking = true;
        Debug.Log("used trap!");
        trapData.anim.SetBool("TrapActivated", true);

        GameObject newProjectile = Instantiate(trapDetails.projectile, transform.position + trapDetails.projectileSpawnPos, Quaternion.identity, this.transform);
        BasicProjectile projectile = newProjectile.GetComponent<BasicProjectile>();
        projectile.SetTargetCell(range[range.Count - 1]);

        GridSpace currentProjectileCell = range[0];
        while(projectile.GetIsMoving())
        {
            GridSpace newClosestCell = gridManager.GetClosestGridSpace(projectile.transform.position, false);
            EvaluateCell(newClosestCell);
            if (newClosestCell != currentProjectileCell)
            {
                currentProjectileCell = newClosestCell;
                EvaluateCell(currentProjectileCell);
            }
        }
        attacking = false;
    }

    private void EvaluateCell(GridSpace cell)
    {
        Trap trap = cell.GetCurrentTrap();
        Hero[] heroes = cell.GetHeroes();
        TryDamage(trap.gameObject);
        foreach (Hero hero in heroes)
        {
            TryDamage(hero.gameObject);
        }
    }

    public void TryDamage(GameObject target)
    {
        IDamageable damage = target.GetComponent<IDamageable>();
        if (damage != null)
        {
            if (statusEffect != null)
            {
                statusEffect.ApplyStatusEffect(target);
            }
            damage.Damage(trapDetails.damageAmount);
        }
    }
    public Vector3 GetOffset()
    {
        return trapDetails.offset;
    }

    public bool TrapEffects(Hero hero)
    {
        return attacking;
    }
    public void DisableTrap() //for when placement happens
    {
        Debug.Log("danny");
    }
    public void EnableTrap()//for when placement happens
    {
        Debug.Log("enabled trap");
        EnableSensors();
    }

    private void EnableSensors()
    {
        foreach (KeyValuePair<Vector2Int, TrapTrigger> spawn in trapSpawns)
        {
            if (spawn.Value == null) { continue; }
            spawn.Value.gameObject.SetActive(true);
            Debug.Log("enabled sensor" + spawn.Value.gameObject);
        }
    }

    public Dictionary<Vector2Int, TrapTrigger> GetTrapContents()
    {
        return trapSpawns;
    }

    public bool GetIsFacingLeft()
    {
        return trapData.trapSprite.transform.rotation.y == 1;
    }

    public void Rotate(bool left)
    {
        foreach (KeyValuePair<Vector2Int, TrapTrigger> spawn in trapSpawns)
        {
            if(spawn.Value == null) { continue; }
            Vector3 localPos = spawn.Value.gameObject.transform.localPosition;
            spawn.Value.gameObject.transform.localPosition = new Vector3(localPos.x * -1, localPos.y, localPos.z);
            spawn.Value.RotateSensor();
        }

        if (left)
        {
            trapData.trapObj.transform.localRotation = new Quaternion(0, 180, 0, 0);
            trapData.trapSprite.transform.localRotation = new Quaternion(0, 180, 0, 0);

        }
        else
        {
            trapData.trapObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            trapData.trapSprite.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        Debug.Log("fartfartafrt");
    }

    public void Arm()
    {
        trapData.armedSprite.SetActive(true);
    }
    public void Disarm()
    {
        trapData.armedSprite.SetActive(false);
    }

    public Vector3 GetRotation()
    {
        return trapData.trapObj.transform.localEulerAngles;
    }
    public bool GetIsWallTrap()
    {
        return trapDetails.isWallTrap;
    }
}
