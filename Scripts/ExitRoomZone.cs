using UnityEngine;

public class ExitRoomZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!StateProvider.playerInExitZone)
        {
            if (other.gameObject.CompareTag("player"))
            {
                StateProvider.playerInExitZone = true;
                Player.instance.controller.SetControllerMode(Controller.ControllerMode.Freeze);
                GuiManager.instance.onGameEndText.text = MultiLanguage.GetText(26);
                GuiManager.instance.deathPanel.SetActive(true);
            }
        }
    }
}
