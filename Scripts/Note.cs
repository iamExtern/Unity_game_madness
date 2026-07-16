using TMPro;
using UnityEngine;

public class Note : Key
{
    public int id;
    public TMP_Text noteText;
    public bool useMobileText = false;

    [TextArea(5, 10)]
    public string textRU;

    [TextArea(5, 10)]
    public string textEN;

    [TextArea(5, 10)]
    public string mobileTextRU;

    [TextArea(5, 10)]
    public string mobileTextEN;

    private bool taked = false;

    protected override void Start()
    {
        base.Start();
    }

    private void Init()
    {
        if (!useMobileText)
        {
            if (MultiLanguage.lang == MultiLanguage.Language.Ru)
                noteText.text = textRU;
            else
                noteText.text = textEN;
        }
        else
        {
            if (Options.pc)
            {
                if (MultiLanguage.lang == MultiLanguage.Language.Ru)
                    noteText.text = textRU;
                else
                    noteText.text = textEN;
            }
            else
            {
                if (MultiLanguage.lang == MultiLanguage.Language.Ru)
                    noteText.text = mobileTextRU;
                else
                    noteText.text = mobileTextEN;
            }
        }

        if (Door.lockCodeDoors.TryGetValue(id, out Door _door))
            noteText.text += _door.code;
    }

    public override void OnHoverEnter()
    {
        if (!taked)
            base.OnHoverEnter();
    }

    public override void OnHoverExit()
    {
        if (!taked)
            base.OnHoverExit();
    }

    public override void Interactive()
    {
        Player.instance.notes.Add(gameObject);
        Player.instance.DisplayNotesCount();
        NoteViewer.instance.OnTakeNote();
        SoundManager.instance.PlaySound(SoundManager.Sound.NoteTake);
        GuiManager.instance.SetAimType(GuiManager.AimType.Point);
        outline.enabled = false;
        taked = true;
        hover = false;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        NoteCodeProvider.onNoteInit += Init;
    }

    private void OnDisable()
    {
        NoteCodeProvider.onNoteInit -= Init;
    }
}
