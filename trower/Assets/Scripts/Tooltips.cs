using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    DialogueManager dialogueManager;
    private void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject dialogueManagerOBJ = gameManager.GetComponent<UIManager>().GetDialogueManager();
        dialogueManager = dialogueManagerOBJ.GetComponent<DialogueManager>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        dialogueManager.DiscoverCursorSwitch(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        dialogueManager.DiscoverCursorSwitch(false);
        dialogueManager.EndMe();
    }
}
