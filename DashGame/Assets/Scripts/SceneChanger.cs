using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator SceneFaderAnimC;

    int scene2Load;

    public static SceneChanger Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Fade2Scene(int levelIndex)
    {
        scene2Load = levelIndex;
        SceneFaderAnimC.SetTrigger("FadeOut");
    }

    public void SceneFadeDone()
    {
        SceneManager.LoadScene(scene2Load);
    }
}
