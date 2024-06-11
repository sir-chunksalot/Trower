using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //text effect 
    [SerializeField] GameObject fakeMouseOBJ;
    [SerializeField] GameObject message;
    [SerializeField] GameObject messageAnim;
    private List<GameObject> dialogueBoxes;
    private List<string> dialogueMessages;
    private List<Vector2> messageOffset;

    private List<GameObject> activeDialogues;

    private void Awake()
    {
        dialogueBoxes = new List<GameObject>();
        activeDialogues = new List<GameObject>();
        dialogueMessages = new List<string>();
        messageOffset = new List<Vector2>();
    }
    public void ReadMessage(GameObject box) //instantiates text
    {
        if(activeDialogues.Count >= 1)
        {
            return;
        }
        int index = -1;
        try
        {
            index = dialogueBoxes.IndexOf(box);
        }
        catch
        {
            Debug.Log("Error! Selected Dialogue Box does not contain a valid reference.");
        }
        if (index == -1) return;
        RectTransform UIElement = dialogueBoxes[index].GetComponent<RectTransform>();
        Transform messageAnimTransform = Instantiate(messageAnim, Vector3.zero, Quaternion.identity, gameObject.transform).transform;
        RectTransform canvas = UIElement.parent.GetComponent<RectTransform>();
        Debug.Log(canvas.gameObject + "TOOcoLD");

        Vector3 pos = Camera.main.ScreenToWorldPoint(UIElement.position);
        float yOffset = messageOffset[index].y;
        float xOffset = messageOffset[index].x;
        pos = new Vector3(pos.x + xOffset, pos.y + yOffset, -10);
        messageAnimTransform.position = pos;

        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        Vector3 screenPos = Camera.main.WorldToScreenPoint(messageAnimTransform.position);


        StartCoroutine(AnimWait(screenPos, index, messageAnimTransform.gameObject));

        

        string message = dialogueMessages[index];
        Debug.Log("LATEATNIGHT  " + message);
    }
    private IEnumerator AnimWait(Vector3 pos, int index, GameObject anim)
    {
        yield return new WaitForSeconds(.1f);
        Destroy(anim);
        RectTransform messageTransform = Instantiate(message, Vector3.zero, Quaternion.identity, dialogueBoxes[index].transform).GetComponent<RectTransform>();

        
        Vector3 uiPos = new Vector3(pos.x, pos.y, pos.z);

        messageTransform.position = uiPos;

        messageTransform.SetAsFirstSibling();

        activeDialogues.Add(messageTransform.gameObject);
    }




    public void EndMe() //called by instantiated dialogue box when cursor leaves it
    {
        Debug.Log("fart");
        foreach (GameObject box in activeDialogues)
        {
            Destroy(box);
        }
        activeDialogues.Clear();
    }

    public void AddDialogueBox(GameObject box, string message, float offsetX, float offsetY) //called by new dialogue boxes to add them to the list
    {
        Debug.Log("MARCELIEN" + box + dialogueBoxes);
        dialogueBoxes.Add(box); //dialogue box contains the location and size of the box (using collider and transform)
        dialogueMessages.Add(message);
        messageOffset.Add(new Vector2(offsetX, offsetY));

    }

    public void DiscoverCursorSwitch(bool isDiscovering) //called by the dialogue boxes whenever you hover over the box
    {
        FakeMouse fakeMouse = fakeMouseOBJ.GetComponent<FakeMouse>();
        if(isDiscovering)
        {
            fakeMouse.SwitchCursor("discover");
        }
        else
        {
            fakeMouse.SwitchCursor("base");
        }
        
    }

}
