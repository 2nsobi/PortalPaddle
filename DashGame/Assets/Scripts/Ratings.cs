using UnityEngine;

public class Ratings : MonoBehaviour
{
    public static Ratings Instance;

    GameObject RateMePage;

    float timeSinceStartup;
    float timeSinceLastAsk = 0;
    static float timeOfLastAsk = 0;
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

    public void Ask4Rate()
    {
        timeSinceStartup = Time.realtimeSinceStartup;

        timeSinceLastAsk = timeSinceStartup - timeOfLastAsk;

        if (timeSinceLastAsk / 30 >= 1)
        {
            timeOfLastAsk = timeSinceStartup;

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
