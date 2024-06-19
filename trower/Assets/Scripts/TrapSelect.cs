using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapSelect : MonoBehaviour
{
    [SerializeField] GameObject[] potentialTraps;
    [SerializeField] GameObject[] trapRotations;
    [SerializeField] Color selectedColor;

    List<string> buildNames;
    GameObject currentBuild;
    private TowerBuilder towerBuilder;
    private TrapBuilder trapBuilder;
    private int selectedTrap;
    private bool rotateTrap;
    bool canDoShit;

    private void Start()
    {
        Debug.Log("jakar +" + potentialTraps[0].name);
        canDoShit = true;
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        trapBuilder = gameObject.GetComponent<TrapBuilder>();
        towerBuilder.onTowerPlace += DeselectAll;
        trapBuilder.onTrapPlace += DeselectAll;
        buildNames = new List<string>();


        foreach (GameObject builds in potentialTraps)
        {
            buildNames.Add(builds.name);
            Debug.Log(builds.name);
        }

    }
    public void DeselectAll(object sender, EventArgs e)
    {
        if (!canDoShit) return;
        StartCoroutine(DontDoShit());
    }

    public void DeselectOne(string name)
    {
        int count = 0;
        foreach (GameObject trap in potentialTraps)
        {
            count++;
            if (trap.name + "(Clone)" + "(Clone)" == name) //scuffed but it works
            {
                trap.GetComponent<CardHolsterGraphics>().SetActiveStatus(false);
            }
        }
    }

    public void OnItemClicked(string buttonName)
    {
        if (!canDoShit) return;
        //trapSprites[selectedTrap].image.color = Color.white;
        selectedTrap = buildNames.FindIndex(x => x.Equals(buttonName));
        Debug.Log(selectedTrap + buttonName + "zorogori");
        currentBuild = potentialTraps[selectedTrap];
        if (currentBuild.name.StartsWith("F"))
        {
            Debug.Log("Grabbed Floor");
        }
        else if (Coins.GetCoins() < currentBuild.GetComponent<CardHolsterGraphics>().GetCost() || currentBuild.GetComponent<CardHolsterGraphics>().GetActiveStatus() == false) return; //play a sound that signifies that you cant place that tower
        //trapSprites[selectedTrap].image.color = selectedColor;
        Debug.Log(buttonName + "BUTTON WAS CLICKED");

        GetCurrentTrap();
    }

    public void Rotate(InputAction.CallbackContext context) //this doesnt actually rotate the trap, it just sends a different prerendered game object to the tower builder script to be used
    {
        if (context.performed)
        {
            if (currentBuild.name[0] == 'T')
            {
                rotateTrap = !rotateTrap;
                Debug.Log(rotateTrap + " sankji");
                GetCurrentTrap();
            }
            string trapName = currentBuild.name;
            char endOfString = trapName[trapName.Length - 1];
            int rotNum = -1;
            int.TryParse(endOfString.ToString(), out rotNum);
            Debug.Log(rotNum + "rotnum" + endOfString + "endstring");

            if (rotNum != -1)
            {
                trapName = currentBuild.name.Substring(0, trapName.Length - 1) + (rotNum + 1);
                foreach (GameObject rotations in trapRotations)
                {
                    Debug.Log(rotations.name + "USOPP" + trapName);
                    if (rotations.name == trapName)
                    {
                        Debug.Log("NAMI");
                        currentBuild = rotations;
                        GetCurrentTrap();
                        return;
                    }
                }
                trapName = currentBuild.name.Substring(0, trapName.Length - 1) + 0;
                foreach (GameObject rotations in trapRotations)
                {
                    Debug.Log(rotations.name + "USOPP" + trapName);
                    if (rotations.name == trapName)
                    {
                        Debug.Log("NAMI");
                        currentBuild = rotations;
                        GetCurrentTrap();
                    }
                }
            }
        }
    }

    public void GetCurrentTrap()
    {
        if (!canDoShit) return;
        if (selectedTrap < potentialTraps.Length)
        {
            Debug.Log(currentBuild.name[0] + "SANJA");
            if (currentBuild.name[0] == 'F') //checks to see if its a floor by the F indicator
            {
                Debug.Log(currentBuild.name + "(Clone)");
                towerBuilder.CurrentFloor(currentBuild.name + "(Clone)");
                trapBuilder.EndPlacement();
            }
            else //if its just a regular trap, tell the towerbuilder script
            {
                trapBuilder.EndPlacement();
                trapBuilder.CurrentTrap(currentBuild.name, rotateTrap); 
                towerBuilder.EndPlacement();
            }

        }
    }

    private IEnumerator DontDoShit() //this method, aptly named "Dont Do Shit", prevents this script from doing shit. i was having problems with the tower getting all fucky when people placed too fast, so here we go. perfect solution 
    {
        canDoShit = false;
        yield return new WaitForSeconds(.2f);
        canDoShit = true;

    }
}
