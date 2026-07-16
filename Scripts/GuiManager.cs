using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    public static GuiManager instance = null;

    private AimType aimType = AimType.Point;
    private bool aimChangeIsLock = false;

    public Image pointAim;
    public Image handAim;
    public Image handLock;
    public Image noteNew;

    public GameObject pauseCont;
    public GameObject pauseButtonsCont;
    public GameObject settingsCont;
    public GameObject noteViewingCont;
    public GameObject wheelZonesCont;
    public GameObject deathPanel;
    public GameObject pauseButton;
    public GameObject joystickCont;
    public GameObject interactiveButton;
    public GameObject empActivateButton;
    public GameObject empRechargeButton;
    public GameObject pauseLogo;

    public TMP_Text noteText;
    public TMP_Text keysText;
    public TMP_Text handAimText;
    public TMP_Text alertText;
    public TMP_Text noteViewingExitText;
    public TMP_Text noteViewingCounterText;
    public TMP_Text noteEmptyText;
    public TMP_Text batteryText;
    public TMP_Text settingsVolumeText;
    public TMP_Text settingsSensText;
    public TMP_Text settingsBackText;
    public TMP_Text settingsResetText;
    public TMP_Text mainMenuText;
    public TMP_Text settingsText;
    public TMP_Text continueText;
    public TMP_Text lockCodeExitText;
    public TMP_Text lockCodeOpenText;
    public TMP_Text onGameEndText;
    public TMP_Text noteGamePaused;
    public TMP_Text squarePauseText;

    public Animator alertTextAnim;
    public Animator onGameEndAnim;

    public Button pauseButtonComponent;

    public FixedJoystick joystick;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Localization();
        SwitchUI();
    }

    private void Localization()
    {
        if (Options.pc)
        {
            noteViewingExitText.text = MultiLanguage.GetText(12);
            lockCodeExitText.text = MultiLanguage.GetText(21);
            squarePauseText.text = MultiLanguage.GetText(33);
        }
        else
        {
            noteViewingExitText.text = MultiLanguage.GetText(28);
            lockCodeExitText.text = MultiLanguage.GetText(31);
        }

        noteEmptyText.text = MultiLanguage.GetText(13);
        settingsVolumeText.text = MultiLanguage.GetText(14);
        settingsSensText.text = MultiLanguage.GetText(15);
        settingsBackText.text = MultiLanguage.GetText(16);
        settingsResetText.text = MultiLanguage.GetText(17);
        mainMenuText.text = MultiLanguage.GetText(18);
        settingsText.text = MultiLanguage.GetText(19);
        continueText.text = MultiLanguage.GetText(20);
        lockCodeOpenText.text = MultiLanguage.GetText(22);
        noteGamePaused.text = MultiLanguage.GetText(32);

    }

    private void SwitchUI()
    {
        pauseLogo.SetActive(!Options.pc);
        pauseButtonComponent.interactable = !Options.pc;
        squarePauseText.gameObject.SetActive(Options.pc);
        joystickCont.SetActive(!Options.pc);
        interactiveButton.SetActive(false);
        empActivateButton.SetActive(false);
        empRechargeButton.SetActive(false);
    }

    public void SetAimHand(string text, bool isLock)
    {
        handAimText.text = text;
        handLock.gameObject.SetActive(isLock);
        handAim.enabled = !isLock;
    }

    public void SetAimType(AimType _aimType)
    {
        if (!Options.pc)
        {
            if (_aimType == AimType.Hand && aimType != AimType.Hand)
            {
                interactiveButton.SetActive(true);
            }
            else if (_aimType != AimType.Hand && aimType == AimType.Hand)
            {
                interactiveButton.SetActive(false);
            }
        }

        aimType = _aimType;
        DisplayAim();
    }

    private void DisplayAim()
    {
        bool isPoint = aimType == AimType.Point;
        if (!aimChangeIsLock)
        {
            handAim.gameObject.SetActive(!isPoint);
            pointAim.gameObject.SetActive(isPoint);
        }
    }

    public void LockHideAim(bool isLock)
    {
        aimChangeIsLock = isLock;

        if (isLock)
        {
            handAim.gameObject.SetActive(false);
            pointAim.gameObject.SetActive(false);
        }
        else
        {
            DisplayAim();
        }
    }

    public void SetActiveControllUI(bool active)
    {
        if (Options.pc)
            return;

        if (!active)
        {
            joystick.UpPointer();
            interactiveButton.SetActive(false);
            joystickCont.SetActive(false);
            empActivateButton.SetActive(false);
            empRechargeButton.SetActive(false);
        }
        else
        {
            if (aimType == AimType.Hand)
                interactiveButton.SetActive(true);

            if (EMP.instance.inHand)
            {
                empActivateButton.SetActive(true);
                empRechargeButton.SetActive(true);
            }

            joystickCont.SetActive(true);
        }
    }

    public void Alert(string text)
    {
        alertText.text = text;
        alertTextAnim.SetTrigger("Show");
    }

    public void HideAlert()
    {
        alertTextAnim.SetTrigger("Hide");
    }

    public enum AimType
    {
        Point,
        Hand
    }
}
