using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject
{
    public static List<Door> doors = new List<Door>();
    public static Dictionary<int, Door> lockCodeDoors = new Dictionary<int, Door>();

    public Animator anim;
    public Transform soundPoint;
    public bool narrowly = false;
    public bool doorIsFreeConst = false;
    public LockMode lockMode = LockMode.None;

    public CodeLock codeLock;
    public int codeLockID;
    public bool codeLockInputFront;
    public string code; //4 צטפנû

    private bool open = false;
    private bool frontLook = false;
    private float[] handleWigglingDelay = new float[] { 1f, 2.5f };
    private Coroutine currentHandleWiggler = null;

    private Lerper hingeLerper = null;
    private float startRotZ;
    private float targetRotZ;

    private void Awake()
    {
        if (lockMode != LockMode.CodeLock && !doorIsFreeConst)
            doors.Add(this);
        else if (lockMode == LockMode.CodeLock)
        {
            lockCodeDoors.Add(codeLockID, this);

            int codeInt = Random.Range(0, 10000);
            string codeStr = codeInt.ToString();
            int lengthDelta = 4 - codeStr.Length;

            for (int i = 0; i < lengthDelta; i++)
            {
                codeStr.Insert(0, "0");
            }

            code = codeStr;
        }
    }

    private void Init(GameObject codeLockPrefab)
    {
        if (lockMode == LockMode.CodeLock)
        {
            Transform codeLockClone = Instantiate(codeLockPrefab, transform).transform;
            if (codeLockInputFront)
            {
                codeLockClone.localPosition = new Vector3(0.006523f, 0.000692f, -0.001833f);
                codeLockClone.localEulerAngles = new Vector3(0f, 90f, -90f);
            }
            else
            {
                codeLockClone.localPosition = new Vector3(0.006523f, -0.000692f, -0.001833f);
                codeLockClone.localEulerAngles = new Vector3(0f, -90f, 90f);
            }

            codeLock = codeLockClone.GetComponent<CodeLock>();
            codeLock.doorRef = this;
        }
    }

    public void OnHoverEnter(bool front)
    {
        if (!hover && !open)
        {
            frontLook = front;
            hover = true;

            GuiManager.instance.SetAimType(GuiManager.AimType.Hand);

            if (lockMode == LockMode.LockKey)
            {
                if (Player.instance.key > 0)
                {
                    if (Options.pc)
                        GuiManager.instance.SetAimHand(MultiLanguage.GetText(0), true);
                    else
                        GuiManager.instance.SetAimHand(MultiLanguage.GetText(22), true);
                }
                else
                {
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(1), true);
                }
            }
            else if (lockMode == LockMode.LockExit)
            {
                if (Player.instance.exitKeyIsHave)
                {
                    if (Options.pc)
                        GuiManager.instance.SetAimHand(MultiLanguage.GetText(0), true);
                    else
                        GuiManager.instance.SetAimHand(MultiLanguage.GetText(22), true);
                }
                else
                {
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(1), true);
                }
            }
            else if (lockMode == LockMode.CodeLock)
            {
                if (Options.pc)
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(9), true);
                else
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(30), true);
            }
            else
            {
                if (Options.pc)
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(0), false);
                else
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(22), false);
            }
        }
    }

    public void OnHoverExit(bool front)
    {
        if (front == frontLook && hover && !open)
        {
            hover = false;
            GuiManager.instance.SetAimType(GuiManager.AimType.Point);
        }
    }

    public override void Interactive()
    {
        if (!open && !anim.enabled)
        {
            if (lockMode == LockMode.LockKey)
            {
                if (Player.instance.key > 0)
                {
                    Player.instance.key--;
                    Open(true);
                }
                else
                {
                    LockDoor();
                }
            }
            else if (lockMode == LockMode.LockExit)
            {
                if (Player.instance.exitKeyIsHave)
                {
                    Open(true);
                }
                else
                {
                    LockDoor();
                }
            }
            else if (lockMode == LockMode.CodeLock)
            {
                codeLock.Interactive();
            }
            else
            {
                Open(false);
            }
        }
    }

    public void Open(bool unlockOpen)
    {
        hover = false;
        anim.enabled = true;
        open = true;

        StopHandleWiggling();

        if (!narrowly)
        {
            if (frontLook)
                anim.Play("doorOpenFront");
            else
                anim.Play("doorOpenBack");
        }
        else
        {
            if (frontLook)
                anim.Play("doorOpenFrontSmall");
            else
                anim.Play("doorOpenBackSmall");
        }

        SoundManager.instance.PlaySound3D(SoundManager.Sound.DoorOpen, soundPoint.position, soundPoint);
        GuiManager.instance.SetAimType(GuiManager.AimType.Point);
    }

    public bool IsOpen() => open;

    public void OnOpenAnimEnd()
    {
        anim.enabled = false;
    }

    public void StartHandleWiggling()
    {
        if (!open && lockMode != LockMode.CodeLock)
        {
            float newDelay = Random.Range(handleWigglingDelay[0], handleWigglingDelay[1]);
            currentHandleWiggler = StartCoroutine(HandleWiggling(newDelay));
            MonsterEventer.doorHandlesWigglingStop += StopHandleWiggling;
        }
    }

    public void StopHandleWiggling()
    {
        if (currentHandleWiggler != null)
            StopCoroutine(currentHandleWiggler);

        MonsterEventer.doorHandlesWigglingStop -= StopHandleWiggling;
    }

    private IEnumerator HandleWiggling(float delay)
    {
        yield return new WaitForSeconds(delay);

        LockDoor();
        float newDelay = Random.Range(handleWigglingDelay[0], handleWigglingDelay[1]);
        currentHandleWiggler = StartCoroutine(HandleWiggling(newDelay));
    }

    public void StartHingeWiggling()
    {
        if (anim.enabled || !open)
            return;

        float duration = Random.Range(5f, 7f);

        startRotZ = transform.localEulerAngles.y;
        if (frontLook)
            targetRotZ = startRotZ + 25f;
        else
            targetRotZ = startRotZ - 25f;

        if (hingeLerper == null)
            hingeLerper = new Lerper(1f / duration, HingeLerperAction);
        hingeLerper.StartLerp(true);

        MonsterEventer.doorHingeWigglingIteration += HingeLerperLerping;
        MonsterEventer.doorHingeWigglingStop += StopHingeWiggling;
    }

    private void StopHingeWiggling()
    {
        hingeLerper.StopLerp();
        hingeLerper.cof = 0f;
        MonsterEventer.doorHingeWigglingIteration -= HingeLerperLerping;
        MonsterEventer.doorHingeWigglingStop -= StopHingeWiggling;
    }

    private void HingeLerperAction(float cof)
    {
        transform.localEulerAngles = new Vector3(-90f, 0f, MathExtra.SmoothSinLerp(startRotZ, targetRotZ, cof));
    }

    private void HingeLerperLerping()
    {
        hingeLerper.Lerping();
    }

    private void LockDoor()
    {
        SoundManager.instance.PlaySound3D(SoundManager.Sound.LockDoor, soundPoint.position, soundPoint);
        anim.enabled = true;
        anim.Play("lock");
    }

    public enum LockMode
    {
        None,
        LockKey,
        CodeLock,
        LockExit
    }

    private void OnEnable()
    {
        NoteCodeProvider.onDoorInit += Init;
    }

    private void OnDisable()
    {
        NoteCodeProvider.onDoorInit -= Init;
    }
}
