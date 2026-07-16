using UnityEngine;

public class NoteViewer : MonoBehaviour
{
    public static NoteViewer instance = null;
    public bool lockOpen = false;

    public bool open = false;
    private int currentNoteID = -1;
    private GameObject currentNote = null;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        Opener();
    }

    private void Opener()
    {
        if (lockOpen || !Options.pc)
            return;

        if (Input.GetKeyDown(Options.noteViewerKey) && !PausePanel.instance.pause && !StateProvider.codeLockOpen)
        {
            if (open)
                Close();
            else
                Open();
        }
    }

    public void OpenFromButton()
    {
        if (Options.pc || Player.instance.monster.playerKilling || StateProvider.playerInExitZone || PausePanel.instance.pause && !StateProvider.codeLockOpen)
            return;

        if (!open)
        {
            Open();
        }
    }

    public void Open()
    {
        if (Player.instance.monster.playerKilling || StateProvider.playerInExitZone)
            return;

        open = true;

        PausePanel.instance.FreezeGame(true);

        Player.instance.controller.SetControllerMode(Controller.ControllerMode.CursorControll);
        Player.instance.interactable = false;

        GuiManager.instance.noteGamePaused.gameObject.SetActive(true);
        GuiManager.instance.LockHideAim(true);
        GuiManager.instance.noteNew.gameObject.SetActive(false);
        GuiManager.instance.HideAlert();
        GuiManager.instance.noteViewingCont.SetActive(true);

        Player.instance.SetVisibleItems(false);

        if (Player.instance.notes.Count > 0)
        {
            RefreshCounterText();
            GuiManager.instance.noteEmptyText.gameObject.SetActive(false);

            DisplayNote(currentNoteID);
        }
        else
        {
            GuiManager.instance.noteViewingCounterText.text = "0/0";
            GuiManager.instance.noteEmptyText.gameObject.SetActive(true);
        }

        if (!Options.pc)
            GuiManager.instance.SetActiveControllUI(false);

        SoundManager.instance.PlaySound(SoundManager.Sound.NoteViewerOpen);
    }

    public void Close()
    {
        open = false;

        PausePanel.instance.FreezeGame(false);

        Player.instance.SetVisibleItems(true);

        Player.instance.controller.SetControllerMode(Controller.ControllerMode.Moving);
        Player.instance.interactable = true;

        GuiManager.instance.noteGamePaused.gameObject.SetActive(false);
        GuiManager.instance.LockHideAim(false);
        GuiManager.instance.noteViewingCont.SetActive(false);

        HideCurrentNote();

        if (!Options.pc)
            GuiManager.instance.SetActiveControllUI(true);

        SoundManager.instance.PlaySound(SoundManager.Sound.NoteViewerOpen);
    }

    public void Back()
    {
        if (currentNoteID > 0)
        {
            SoundManager.instance.PlaySound(SoundManager.Sound.NoteViewerTurning);

            HideCurrentNote();
            currentNoteID--;
            DisplayNote(currentNoteID);

            RefreshCounterText();
        }
    }

    public void Next()
    {
        if (currentNoteID < Player.instance.notes.Count - 1)
        {
            SoundManager.instance.PlaySound(SoundManager.Sound.NoteViewerTurning);

            HideCurrentNote();
            currentNoteID++;
            DisplayNote(currentNoteID);

            RefreshCounterText();
        }
    }

    public void OnTakeNote()
    {
        if (Options.pc)
            GuiManager.instance.Alert(MultiLanguage.GetText(4));
        else
            GuiManager.instance.Alert(MultiLanguage.GetText(29));

        GuiManager.instance.noteNew.gameObject.SetActive(true);
        currentNoteID = Player.instance.notes.Count - 1;
    }

    private void DisplayNote(int noteID)
    {
        Player.instance.notes[noteID].SetActive(true);
        Player.instance.notes[noteID].transform.position = Player.instance.notePoint.position;
        Player.instance.notes[noteID].transform.SetParent(Player.instance.controller.cam);
        Player.instance.notes[noteID].transform.localEulerAngles = Player.instance.notePoint.localEulerAngles;
        currentNote =  Player.instance.notes[noteID];
    }

    private void HideCurrentNote()
    {
        if (currentNote != null)
            currentNote.SetActive(false);
    }

    private void RefreshCounterText()
    {
        GuiManager.instance.noteViewingCounterText.text = (currentNoteID + 1) + "/" + Player.instance.notes.Count;
    }
}
