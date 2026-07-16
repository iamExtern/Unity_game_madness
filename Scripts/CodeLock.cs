using UnityEngine;

public class CodeLock : MonoBehaviour
{
    public Transform camPoint;
    public Door doorRef;
    public CodeLockWheel[] wheels;

    public bool zoomMode = false;
    private Vector3 lastCamRot;
    private Vector3 lastCamParentRot;

    private bool wheelRotation = false;
    private CodeLockWheel currentWheel = null;

    private void Update()
    {
        WheelInteraction();
    }

    private void WheelInteraction()
    {
        if (!zoomMode)
            return;

        if (wheelRotation)
        {
            currentWheel.Rotation();
        }
    }

    public void OnWheelDown(int wheelID)
    {
        wheelRotation = true;
        currentWheel = wheels[wheelID];
        currentWheel.Select();
    }

    public void OnWheelUp(int wheelID)
    {
        wheelRotation = false;
        currentWheel.ToNearestNumber();
    }

    public void Interactive()
    {
        zoomMode = !zoomMode;
        StateProvider.codeLockOpen = zoomMode;

        Player.instance.SetVisibleItems(!zoomMode);

        if (zoomMode)
        {
            PausePanel.instance.FreezeGame(true);

            if (!Options.pc)
                GuiManager.instance.SetActiveControllUI(false);

            GuiManager.instance.noteGamePaused.gameObject.SetActive(true);

            NoteViewer.instance.lockOpen = true;
            GuiManager.instance.wheelZonesCont.SetActive(true);
            GuiManager.instance.LockHideAim(true);
            GuiManager.instance.HideAlert();
            Player.instance.controller.CancelForces();
            Player.instance.controller.SetControllerMode(Controller.ControllerMode.CursorControll);
            Player.instance.interactable = false;
            Player.instance.onlyInteractiveKey = true;

            lastCamRot = Player.instance.controller.playerMain.eulerAngles;
            lastCamParentRot = Player.instance.controller.cam.localEulerAngles;
            Player.instance.controller.cam.localEulerAngles = Vector3.zero;
            Player.instance.controller.playerMain.position = camPoint.position;
            Player.instance.controller.playerMain.eulerAngles = camPoint.eulerAngles;
            Player.instance.SetVisibleItems(false);

            WheelZone.onDown += OnWheelDown;
            WheelZone.onUp += OnWheelUp;
            CodeLockPanel.onExit += Interactive;
            CodeLockPanel.onOpen += TryOpen;
        }
        else
        {
            PausePanel.instance.FreezeGame(false);

            if (!Options.pc)
                GuiManager.instance.SetActiveControllUI(true);

            GuiManager.instance.noteGamePaused.gameObject.SetActive(false);

            NoteViewer.instance.lockOpen = false;
            GuiManager.instance.wheelZonesCont.SetActive(false);
            GuiManager.instance.LockHideAim(false);
            Player.instance.controller.SetControllerMode(Controller.ControllerMode.Moving);
            Player.instance.interactable = true;
            Player.instance.onlyInteractiveKey = false;

            Player.instance.controller.playerMain.localPosition = Vector3.zero;
            Player.instance.controller.playerMain.eulerAngles = lastCamRot;
            Player.instance.controller.cam.localEulerAngles = lastCamParentRot;
            Player.instance.SetVisibleItems(true);

            WheelZone.onDown -= OnWheelDown;
            WheelZone.onUp -= OnWheelUp;
            CodeLockPanel.onExit -= Interactive;
            CodeLockPanel.onOpen -= TryOpen;
        }
    }

    private void TryOpen()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i].number != (doorRef.code[i] - '0'))
            {
                GuiManager.instance.Alert(MultiLanguage.GetText(10));
                return;
            }
        }

        doorRef.Open(true);
        Interactive();
        Destroy(gameObject);
    }
}
