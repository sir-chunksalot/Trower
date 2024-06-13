using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] GameObject message;
    [SerializeField] float offsetY;
    [SerializeField] float offsetX;
    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject dialogueManagerOBJ = gameManager.GetComponent<UIManager>().GetDialogueManager();
        DialogueManager dialogueManager = dialogueManagerOBJ.GetComponent<DialogueManager>();
        dialogueManager.AddDialogueBox(gameObject.GetComponentInParent<Transform>().parent.gameObject, message, offsetX, offsetY);

        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(() => { dialogueManager.ReadMessage(gameObject.transform.parent.gameObject); });
    }




}
