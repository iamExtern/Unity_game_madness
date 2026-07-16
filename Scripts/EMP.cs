using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EMP : Key
{
    public static EMP instance = null;

    public MeshRenderer mesh;
    public GameObject canvas;
    public float batteryTime = 30f; //40f
    public Color signalEnabledColor;
    public Color signalDisabledColor;
    public Image signal;
    public Animator energyTextAnim;
    public TMP_Text energyText;
    public TMP_Text offText;
    public Bulb[] bulb;

    private bool isEnabled = false;
    public bool inHand = false;
    private bool energyDrain = false;
    private float energy;

    private float energySubstract;

    private const int animEndId = 1;
    public int currentLevel { get; private set; } = 1;
    private bool errorState;

    private void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        base.Start();

        energySubstract = 100f / batteryTime;
        Disable();
        offText.text = MultiLanguage.GetText(8);
    }

    void Update()
    {
        Switcher();
        EnergyDrain();
        Charger();
    }

    private void Switcher()
    {
        if (!inHand || !Options.pc)
            return;

        if (Input.GetKeyDown(Options.empKey))
        {
            EmpSwitchActive();
        }
    }

    public void EmpSwitchActive()
    {
        if (!StateProvider.codeLockOpen && !NoteViewer.instance.open && !PausePanel.instance.pause)
        {
            SoundManager.instance.PlaySound(SoundManager.Sound.EmpEnabled);
            if (isEnabled)
                Disable();
            else
                Enable();
        }
    }

    public void SetVisible(bool visible)
    {
        if (!inHand)
            return;

        mesh.enabled = visible;
        canvas.SetActive(visible);

        foreach (Bulb b in bulb)
            b.gameObject.SetActive(visible);
    }

    public void EmpCharge()
    {
        if (!StateProvider.codeLockOpen && !NoteViewer.instance.open && !PausePanel.instance.pause)
        {
            if (Player.instance.battery > 0)
            {
                if (energy < 10f)
                {
                    Player.instance.battery--;
                    energy = 100f;
                    SoundManager.instance.PlaySound(SoundManager.Sound.EmpCharge);
                    DisplayEnergy();
                }
                else
                {
                    GuiManager.instance.Alert(MultiLanguage.GetText(6));
                }
            }
            else
            {
                GuiManager.instance.Alert(MultiLanguage.GetText(7));
            }
        }
    }

    private void Charger()
    {
        if (!inHand || !Options.pc)
            return;

        if (Input.GetKeyDown(Options.batteryChargeKey))
        {
            EmpCharge();
        }
    }

    private void Enable()
    {
        if (energy > 0f)
        {
            isEnabled = true;
            energyDrain = true;
            signal.color = signalEnabledColor;
            DisplayEnergy();

            bulb[0].SetActive(true);
            offText.gameObject.SetActive(false);
            energyText.gameObject.SetActive(true);

            SetLevel(currentLevel);
        }
        else
        {
            GuiManager.instance.Alert(MultiLanguage.GetText(5));
        }
    }

    private void Disable()
    {
        isEnabled = false;
        energyDrain = false;
        signal.color = signalDisabledColor;

        foreach (Bulb _bulb in bulb)
            _bulb.SetActive(false);

        offText.gameObject.SetActive(true);
        energyText.gameObject.SetActive(false);

        SoundManager.instance.SetEmpLevel(0);
    }

    public void SetLevel(int level) //min level = 1
    {
        if (isEnabled)
        {
            SetErrorState(false);

            for (int i = 0; i < bulb.Length; i++)
            {
                if (i < level)
                    bulb[i].SetActive(true);
                else
                    bulb[i].SetActive(false);
            }

            if (level == 6)
                SetErrorState(true);

            SoundManager.instance.SetEmpLevel(level - 1);
        }

        currentLevel = level;
    }

    private void SetErrorState(bool error)
    {
        if (error == errorState)
            return;

        if (error)
        {
            errorState = true;
            energyText.fontSize = 5f;
            energyText.color = Color.red;
            energyText.text = MultiLanguage.GetText(11);
        }
        else
        {
            errorState = false;
            energyText.fontSize = 8f;
            energyText.color = MathExtra.SetColor(219, 219, 219, 255);
            DisplayEnergy();
        }
    }

    private void EnergyDrain()
    {
        if (!enabled)
            return;

        if (energyDrain)
        {
            if (energy > 0f)
            {
                energy -= Time.deltaTime * energySubstract;
                if (energy < 0f)
                    energy = 0f;

                DisplayEnergy();
            }
            else
            {
                energy = 0f;
                energyDrain = false;
                energyTextAnim.SetTrigger("noBattery");
            }
        }
    }

    private void DisplayEnergy()
    {
        if (!errorState)
            energyText.text = (int)energy + "%";
    }

    private void OnNoBatteryAnimEnd(int id)
    {
        if (id == animEndId)
        {
            Disable();
        }
    }

    public override void OnHoverEnter()
    {
        if (!inHand)
            base.OnHoverEnter();
    }

    public override void OnHoverExit()
    {
        if (!inHand)
            base.OnHoverExit();
    }

    public override void Interactive()
    {
        hover = false;
        inHand = true;

        if (!Options.pc)
        {
            GuiManager.instance.empActivateButton.SetActive(true);
            GuiManager.instance.empRechargeButton.SetActive(true);
        }

        SoundManager.instance.PlaySound(SoundManager.Sound.EmpTake);

        GuiManager.instance.SetAimType(GuiManager.AimType.Point);
        transform.position = Player.instance.empPoint.position;
        transform.SetParent(Player.instance.controller.empContAnim.transform);
        transform.localEulerAngles = Player.instance.empPoint.localEulerAngles;
        transform.localScale = Player.instance.empPoint.localScale;
        outline.enabled = false;
    }

    private void OnEnable()
    {
        AnimHelp.onAnimEnd += OnNoBatteryAnimEnd;
    }

    private void OnDisable()
    {
        AnimHelp.onAnimEnd -= OnNoBatteryAnimEnd;
    }
}
