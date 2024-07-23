using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] GameObject message;
    [SerializeField] float offsetY;
    [SerializeField] float offsetX;
    [SerializeField] bool canClick;
    private bool hasBeenRead;
    DialogueManager dialogueManager;
    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject dialogueManagerOBJ = gameManager.GetComponent<UIManager>().GetDialogueManager();
        if(dialogueManagerOBJ != null)
        {
            dialogueManager = dialogueManagerOBJ.GetComponent<DialogueManager>();
            dialogueManager.AddDialogueBox(gameObject.GetComponentInParent<Transform>().parent.gameObject, message, offsetX, offsetY);

            Button button = gameObject.GetComponent<Button>();
            if (canClick)
            {
                button.onClick.AddListener(() => { dialogueManager.ReadMessage(gameObject.transform.parent.gameObject); });
            }
        }
        else
        {
            Debug.Log("ERROR! 'DialogueBox' requires valid reference to 'DialogueManagerOBJ'.");
        }

        
    }

    public void ManualReadMessage()
    {
        if(dialogueManager != null)
        {
            dialogueManager.ReadMessage(gameObject.transform.parent.gameObject);
            hasBeenRead = true;
        }
        else
        {
            Debug.Log("ERROR! 'DialogueBox' requires valid reference to 'DialogueManager'.");
        }

    }

    public bool GetHasBeenRead()
    {
        return hasBeenRead;
    }




}
