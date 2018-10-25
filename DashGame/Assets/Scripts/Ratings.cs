using UnityEngine;

public class Ratings : MonoBehaviour
{
    public static Ratings Instance;

    GameObject RateMePage;

    float timeSinceLastAsk = 0;
    float timeOfLastAsk;
    static string currentPlatform = (Application.platform == RuntimePlatform.IPhonePlayer) ? "apple" : "android";

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

        DontDestroyOnLoad(gameObject);

        RateMePage = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        timeOfLastAsk = 0;
    }

    public void Ask4Rate()
    {
        timeSinceLastAsk = Time.realtimeSinceStartup - timeOfLastAsk;

        if (timeSinceLastAsk / 360 >= 1)
        {
            timeOfLastAsk = Time.realtimeSinceStartup;

            RateMePage.SetActive(true);
        }
    }

    public void Go2Rate()
    {
        if (currentPlatform == "android")
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.nnaji.Portal.Paddle");
        }
    }

    public void ExitRateMePage()
    {
        RateMePage.SetActive(false);
    }
}
