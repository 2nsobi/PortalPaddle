using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator SceneFaderAnimC;

    int scene2Load;

    public static SceneChanger Instance;

    private void Awake()
    {
        Instance = this;
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
