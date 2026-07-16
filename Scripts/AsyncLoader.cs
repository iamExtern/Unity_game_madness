using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoader : MonoBehaviour
{
    public GameObject button1;
    public GameObject button2;

    public Animator canvasAnim;
    public Image progressBarImage;

    public void LoadGameScene()
    {
        canvasAnim.SetTrigger("sceneLoad");
    }

    public void DisableButtons()
    {
        button1.SetActive(false);
        button2.SetActive(false);
    }

    public void EnableButtons()
    {
        button1.SetActive(true);
        button2.SetActive(true);
    }

    public void OnAnimEnd()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(1);

        while (!loadOp.isDone)
        {
            float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
            progressBarImage.fillAmount = progress;
            yield return null;
        }
    }
}
