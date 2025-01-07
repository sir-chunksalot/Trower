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
        Debug.Log("jakarr +" + potentialTraps[0].name);
        canDoShit = true;
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        trapBuilder = gameObject.GetComponent<TrapBuilder>();
        buildNames = new List<string>();


        foreach (GameObject builds in potentialTraps)
        {
            buildNames.Add(builds.name);
        }

    }

    public bool GetPlacing()
    {
        Debug.Log("DOOFEN GET PLACING");
        if (towerBuilder.GetIsPlacingFloor() || trapBuilder.GetPlacingTrap())
        {
            return true;
        }
        return false;
    }

    public void OnItemClicked(string buttonName)
    {
        if (!canDoShit) return;
        selectedTrap = buildNames.FindIndex(x => x.Equals(buttonName));
        currentBuild = potentialTraps[selectedTrap];

        GetCurrentTrap();
    }

    public void Rotate(InputAction.CallbackContext context) //this doesnt actually rotate the trap, it just sends a different prerendered game object to the tower builder script to be used
    {
        if (currentBuild == null) { return; }
        if (context.performed)
        {
            if (currentBuild.name[0] == 'T')
            {
                rotateTrap = !rotateTrap;
                Debug.Log(rotateTrap + " sankji");
                GetCurrentTrap();
                return;
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
                        return;
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
            else if (currentBuild.name[0] == 'T')//if its just a regular trap, tell the trappbuilder script
            {
                trapBuilder.EndPlacement();
                trapBuilder.CurrentTrap(currentBuild.name, rotateTrap);
                towerBuilder.EndPlacement();
            }

        }
    }

}
