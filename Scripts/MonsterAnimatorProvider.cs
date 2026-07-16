using UnityEngine;

public class MonsterAnimatorProvider : MonoBehaviour
{
    public Monster monster;
    public MonsterEventer monsterEventer;

    public void OnTrashCunHandEnd()
    {
        monsterEventer.TrashCunHandEnd();
    }

    public void OnPlayerGrab()
    {
        monster.StopMaxAggressionSP();
        monster.PlayGrabAggressiveSound();
    }

    public void ActiveDeathPanel()
    {
        GuiManager.instance.onGameEndText.text = MultiLanguage.GetText(25);
        GuiManager.instance.deathPanel.SetActive(true);
    }
}
