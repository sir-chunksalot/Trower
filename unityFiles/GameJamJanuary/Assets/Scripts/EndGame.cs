using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private Sprite bgWin;
    [SerializeField] private Sprite bgLoss;
    [SerializeField] private GameObject scoreTextBox;

    private TMP_Text scoreText;

    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        scoreText = scoreTextBox.GetComponent<TMP_Text>();
        DidYouWin(GameEnd.GetWinStatus());
    }

    public void DidYouWin(bool yes)
    {
        if (yes)
        {
            spriteRenderer.sprite = bgWin;
            scoreText.text = GameEnd.GetScore().ToString();
            scoreTextBox.SetActive(true);
        }
        else
        {
            spriteRenderer.sprite = bgLoss;
            scoreTextBox.SetActive(false);
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(2);
        SceneManager.UnloadSceneAsync(3);
    }
}
