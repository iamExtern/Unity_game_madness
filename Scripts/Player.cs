using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Monster monster;
    public static Player instance = null;

    public float interactiveDistance = 0.8f;
    public float itemGrabbingDistance = 0.4f;
    public LayerMask interactableLayerMask;
    public Controller controller;
    public Transform notePoint;
    public Transform empPoint;
    public Transform exitKeyPoint;
    public bool exitKeyIsHave;

    public List<GameObject> notes = new List<GameObject>();

    private int _key = 0;
    public int key
    {
        get => _key;
        set
        {
            _key = value;
            GuiManager.instance.keysText.text = value.ToString();
        }
    }

    private int _battery = 0;
    public int battery
    {
        get => _battery;
        set
        {
            _battery = value;
            GuiManager.instance.batteryText.text = value.ToString();
        }
    }

    private InteractableObject currentInteractableObject = null;

    public bool interactable = true;
    public bool onlyInteractiveKey = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        Interactable();
        MonsterEyeChecker();
    }

    private void MonsterEyeChecker()
    {
        if (monster.gameObject.activeSelf && !monster.playerKilling && !NoteViewer.instance.open && !StateProvider.codeLockOpen)
        {
            RaycastHit hit;
            if (Physics.Raycast(controller.cam.position, controller.cam.forward, out hit, 100f))
            {
                if (hit.collider.gameObject.CompareTag("monsterHead"))
                {
                    monster.OnPlayerEyeLook();
                }
            }
        }
    }

    public void Interactive()
    {
        if (!interactable && !onlyInteractiveKey)
            return;

        if (currentInteractableObject != null)
            currentInteractableObject.Interactive();
    }

    private void Interactable()
    {
        if (!interactable && !onlyInteractiveKey)
            return;

        if (!onlyInteractiveKey)
        {
            RaycastHit hit;

            if (Physics.Raycast(controller.cam.position, controller.cam.forward, out hit, interactiveDistance, interactableLayerMask))
            {
                if (hit.collider.gameObject.CompareTag("interactive"))
                {
                    HoverEnter(hit);
                }
                else if (hit.collider.gameObject.CompareTag("item"))
                {
                    if (Vector3.Distance(controller.cam.position, hit.point) < itemGrabbingDistance)
                    {
                        HoverEnter(hit);
                    }
                    else if (currentInteractableObject != null)
                        HoverExit();
                }
                else if (currentInteractableObject != null)
                {
                    HoverExit();
                }
            }
            else if (currentInteractableObject != null)
            {
                HoverExit();
            }
        }

        if (!Options.pc)
            return;

        if (Input.GetKeyDown(Options.interactiveKey) && currentInteractableObject != null)
        {
            currentInteractableObject.Interactive();
        }
    }

    public void SetVisibleItems(bool visible)
    {
        EMP.instance.SetVisible(visible);

        if (exitKeyIsHave)
            ExitKey.instance.gameObject.SetActive(visible);
    }

    private void HoverEnter(RaycastHit hit)
    {
        if (currentInteractableObject != null && currentInteractableObject != this)
            currentInteractableObject.OnHoverExit();

        InteractableObject io = hit.collider.gameObject.GetComponent<InteractableObject>();
        currentInteractableObject = io;
        io.OnHoverEnter();
    }

    private void HoverExit()
    {
        currentInteractableObject.OnHoverExit();
        currentInteractableObject = null;
    }

    public void DisplayNotesCount()
    {
        GuiManager.instance.noteText.text = notes.Count.ToString();
    }
}
