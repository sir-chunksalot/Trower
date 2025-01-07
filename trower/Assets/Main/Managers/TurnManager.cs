using System;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] GameObject turnTrackerUI;

    public event EventHandler OnEnemyTurn;
    public event EventHandler OnEnemiesEvaluated;

    private bool playerTurn;
    private bool enemiesEvaluated;
    private bool trapsEvaluated;
    private bool trapTurn;

    TMP_Text turnTrackerText;

    private void Start()
    {
        turnTrackerText = turnTrackerUI.GetComponent<TMP_Text>();
        playerTurn = true;
    }

    private void PlayerTurn()
    {
        playerTurn = true;
        enemiesEvaluated = false;
        trapsEvaluated = false;
        turnTrackerText.text = "player turn";
    }

    private void EnemyTurn()
    {
        turnTrackerText.text = "enemy turn";
        playerTurn = false;
        OnEnemyTurn.Invoke(gameObject, EventArgs.Empty);
    }


    //cycle:
    //1. player makes traps, arms traps, and makes buildings. 
    //2. enemies move around
    //3. traps that were armed in step 1 fire
    public void ChangeTurn()
    {
        if (trapsEvaluated && enemiesEvaluated && !playerTurn)
        {
            PlayerTurn();
        }
        else if (!enemiesEvaluated && !playerTurn)
        {
            turnTrackerText.text = "wait up for enemies stinky boy";
        }
        else if (!trapsEvaluated && !playerTurn)
        {
            turnTrackerText.text = "wait up for enemies stinky boy";
        }
        else
        {
            EnemyTurn();
        }
    }

    public void EnemiesEvaluated() //called by heromanager script
    {
        enemiesEvaluated = true;
        OnEnemiesEvaluated.Invoke(gameObject, EventArgs.Empty);
    }

    public void TrapsEvaluated() //called my trap manager script
    {
        trapsEvaluated = true;
        ChangeTurn();
    }

    public bool CanArmTraps()
    {
        return playerTurn;
    }
}
