using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    DialogueManager dialogueManager;
    private void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject dialogueManagerOBJ = gameManager.GetComponent<UIManager>().GetDialogueManager();
        if(dialogueManagerOBJ != null)
        {
            dialogueManager = dialogueManagerOBJ.GetComponent<DialogueManager>();
        }
        else { Debug.Log("ERROR! 'Tooltip' requires valid reference to 'DialogueManagerOBJ"); }

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dialogueManager == null) { return; }
        dialogueManager.DiscoverCursorSwitch(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (dialogueManager == null) { return; }
        dialogueManager.DiscoverCursorSwitch(false);
        dialogueManager.EndMe(gameObject.transform.parent.GetComponentsInChildren<Transform>()[1].gameObject);
    }
}
