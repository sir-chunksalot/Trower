using UnityEngine;

public class DevTools : MonoBehaviour
{

    [SerializeField] GameObject meatSack;
    bool activeMeatsack;
    Vector3 mousePos;
    GameObject currentMeat;


    public void ShowViableFloors()
    {
        GenerateViableFloors genViableFloors = gameObject.GetComponent<GenerateViableFloors>();
        Debug.Log("GENVIABLE FLOORS");
        genViableFloors.Test();
    }

    public void GainCoin(float coin)
    {
        Coins.ChangeCoins(coin);
    }

    public void SpawnMeatSack()
    {
        if (!activeMeatsack)
        {
            currentMeat = Instantiate(meatSack, Vector2.zero, Quaternion.identity);
            activeMeatsack = true;
        }
        else
        {
            activeMeatsack = false;
            Destroy(currentMeat);
        }
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (activeMeatsack && currentMeat != null)
        {
            currentMeat.transform.position = new Vector3(mousePos.x, mousePos.y, 1.35f);
        }
    }

}
