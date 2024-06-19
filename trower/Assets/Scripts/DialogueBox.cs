using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] GameObject message;
    [SerializeField] float offsetY;
    [SerializeField] float offsetX;
    [SerializeField] bool canClick;

    DialogueManager dialogueManager;
    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject dialogueManagerOBJ = gameManager.GetComponent<UIManager>().GetDialogueManager();
        dialogueManager = dialogueManagerOBJ.GetComponent<DialogueManager>();
        dialogueManager.AddDialogueBox(gameObject.GetComponentInParent<Transform>().parent.gameObject, message, offsetX, offsetY);

        Button button = gameObject.GetComponent<Button>();
        if(canClick)
        {
            button.onClick.AddListener(() => { dialogueManager.ReadMessage(gameObject.transform.parent.gameObject); });
        }
        
    }

    public void ManualReadMessage()
    {
        dialogueManager.ReadMessage(gameObject.transform.parent.gameObject);
    }




}
