using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [SerializeField] GameObject bolt;
    [SerializeField] float yOffset;
    [SerializeField] GameObject detectorContainer;
    [SerializeField] Animator anim;
    float roomSizeX;
    float roomSizeY;
    bool isClicked;
    List<GameObject> heroes;
    bool active;

    private void Awake()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        TowerBuilder towerBuilder = gameManager.GetComponent<TowerBuilder>();
        heroes = new List<GameObject>();
        roomSizeX = towerBuilder.GetRoomBounds().x;
        roomSizeY = towerBuilder.GetRoomBounds().y;
    }
    public float GetYOffset()
    {
        return yOffset;
    }

    public void DisableSensor()
    {
        active = false;
    }

    public void EnableSensor()
    {
        active = false;
    }

    public void Rotate()
    {
        detectorContainer.transform.localRotation = new Quaternion(0, 180, 0, 0);
        Debug.Log("fartfartafrt");
    }

    private void Activate()
    {
        GameObject newBolt = Instantiate(bolt, transform.position, Quaternion.identity, detectorContainer.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isClicked) return;
        if (collision.gameObject.layer == 6)
        {
            Activate();
            anim.SetBool("Clicked", true);
            isClicked = true;
            heroes.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            if (heroes.Contains(collision.gameObject))
            {
                heroes.Remove(collision.gameObject);
            }
            if (heroes.Count <= 0)
            {
                isClicked = false;
                anim.SetBool("Clicked", false);
            }

        }
    }
}
