using UnityEngine;

public class Ratings : MonoBehaviour
{
    public static Ratings Instance;

    GameObject RateMePage;

    float timeSinceLastAsk = 0; // time since Ask4Rate() was called
    float timeOfLastAsk; //time since the rating panel popped up on screen
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

        if (timeSinceLastAsk / 420 >= 1)
        {
            timeOfLastAsk = Time.realtimeSinceStartup;

            RateMePage.SetActive(true);
        }
    }

    public void Go2Rate()
    {
        if (currentPlatform == "android")
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.nnaji.PortalPaddle");
        }
        else
        {
            Application.OpenURL("itms-apps://itunes.apple.com/app/id1444939274");
        }
    }

    public void ExitRateMePage()
    {
        RateMePage.SetActive(false);
    }
}
