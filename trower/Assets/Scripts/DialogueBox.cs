using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] string message;
    [SerializeField] GameObject dialogueManagerOBJ;
    [SerializeField] float offsetY;
    [SerializeField] float offsetX;
    void Start()
    {
        DialogueManager dialogueManager = dialogueManagerOBJ.GetComponent<DialogueManager>();
        dialogueManager.AddDialogueBox(gameObject.GetComponentInParent<Transform>().parent.gameObject, message, offsetX, offsetY);
    }
}
