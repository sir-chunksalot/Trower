using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject enemy2;
    [SerializeField] GameObject door;
    [SerializeField] GameObject door2;
    [SerializeField] GameObject fakeMouse;
    [SerializeField] Sprite openDoorSprite;
    [SerializeField] GameObject spears;
    [SerializeField] GameObject spears2;
    [SerializeField] GameObject fireSpitter;
    [SerializeField] GameObject scoreTextBox;
    [SerializeField] GameObject bloodTextBox;

    [SerializeField] GameObject hover;
    [SerializeField] GameObject trapsCost;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject pressA;
    [SerializeField] GameObject activate;
    [SerializeField] GameObject resist;
    [SerializeField] GameObject pressA2;
    [SerializeField] GameObject pressA3;
    [SerializeField] GameObject pressD2;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject enter;

    [SerializeField] AudioSource audio1;
    [SerializeField] AudioSource audio2;
    [SerializeField] AudioSource audio3;

    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 secondPos;
    TMP_Text scoreText;
    TMP_Text bloodText;
    CameraController cam;

    int count;
    bool killable;
    bool killable2;
    bool touching;

    private void Start()
    {
        gameObject.transform.position = startPos;
        bloodText = bloodTextBox.GetComponent<TMP_Text>();
        scoreText = scoreTextBox.GetComponent<TMP_Text>();
        cam = Camera.main.GetComponent<CameraController>();
        enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        killable = false;
        //cam.SetCameraPos(new Vector3(0, 11, -10));
        hover.SetActive(true);
    }

    public void Activate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (count == 0 && touching && context.control.name.Equals("d"))
            {
                Debug.Log("start");
                door.GetComponent<SpriteRenderer>().sprite = openDoorSprite;
                audio1.Play();

                enemy.GetComponent<Rigidbody2D>().velocity = Vector2.up * -18;
                bloodText.text = "40";
                hover.SetActive(false);
                trapsCost.SetActive(true);
                arrow.SetActive(true);
                pressA.SetActive(true);
                count++;
            }
            else if (count == 1 && touching && context.control.name.Equals("a"))
            {
                door2.GetComponent<SpriteRenderer>().sprite = openDoorSprite;
                audio1.Play();
                bloodText.text = "30";
                trapsCost.SetActive(false);
                arrow.SetActive(false);
                pressA.SetActive(false);

                activate.SetActive(true);

                enemy2.GetComponent<Rigidbody2D>().velocity = Vector2.up * -18;
                gameObject.transform.position = secondPos;
                count++;
            }
            else if (count == 2 && touching)
            {
                if (context.control.name.Equals("a"))
                {
                    audio2.Play();
                    spears2.SetActive(true);
                    pressD2.SetActive(true);
                    killable2 = true;
                }

                if (context.control.name.Equals("d"))
                {
                    audio2.Play();
                    spears.SetActive(true);
                    pressA2.SetActive(true);
                    killable = true;
                }

                if (killable && killable2)
                {
                    activate.SetActive(false);
                    pressD2.SetActive(false);
                    pressA2.SetActive(false);
                    resist.SetActive(true);
                    pressA3.SetActive(true);
                    count++;
                }

            }
            else if (count == 3 && touching)
            {
                resist.SetActive(false);
                pressA3.SetActive(false);
                fire.SetActive(true);
                bloodText.text = "20";

                fireSpitter.SetActive(true);
                enter.SetActive(true);
                StartCoroutine(BurnTime());
            }
        }
    }

    public void ChangePhase(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(2);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Mouse")
        {
            touching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Mouse")
        {
            touching = false;
        }
    }

    private IEnumerator BurnTime()
    {
        yield return new WaitForSeconds(1);
        Destroy(enemy2);
        audio3.Play();
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        fakeMouse.transform.position = new Vector3(mousePos.x, mousePos.y, 10);
        if (killable && enemy != null)
        {
            if (enemy.transform.position.y < -1.7)
            {
                audio3.Play();
                Destroy(enemy);
                //cam.AddShake(.6f);
                scoreText.text = (float.Parse(scoreText.text) + 1).ToString();
            }

        }

    }

}
