using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator SceneFaderAnimC;
    public PaddleController paddle;

    int scene2Load;

    public static SceneChanger Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        paddle = PaddleController.Instance;
    }

    public void Fade2Scene(int levelIndex)
    {
        scene2Load = levelIndex;
        SceneFaderAnimC.SetTrigger("FadeOut");
    }

    public void SceneFadeDone()
    {
        paddle.DontDestroyActivePaddle();
        SceneManager.LoadScene(scene2Load);
    }
}
