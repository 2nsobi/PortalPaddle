using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public static SettingsController Instance;

    public Button buyNoAdsButton;
    public GameObject disabledNoAdsButtonFilter;
    public Text NoAdsPrice;
    public GameObject SoundOnIcon;
    public GameObject SoundOffIcon;

    Purchaser purchaser;
    BallController ballC;

    bool noSound;
    bool noAds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        noSound = PlayerPrefsX.GetBool("noSound");
        if (noSound)
        {
            SoundOffIcon.SetActive(true);
            SoundOnIcon.SetActive(false);
        }
        else
        {
            SoundOffIcon.SetActive(false);
            SoundOnIcon.SetActive(true);
        }

        noAds = PlayerPrefsX.GetBool("noAds");

        disabledNoAdsButtonFilter.SetActive(false);
    }

    private void Start()
    {
        purchaser = Purchaser.Instance;
        ballC = BallController.Instance; 
    }

    public void SetLocalizedPrices()
    {
        NoAdsPrice.text = purchaser.NoAdsPrice();
    }

    public void ToggleSound()
    {
        noSound = !noSound;

        if (noSound)
        {
            AudioListener.volume = 0;

            SoundOffIcon.SetActive(true);
            SoundOnIcon.SetActive(false);
        }
        else
        {
            AudioListener.volume = 1;

            SoundOffIcon.SetActive(false);
            SoundOnIcon.SetActive(true);
        }
        ballC.SetNoSound(noSound);
    }

    private void OnEnable()
    {
        if (noAds)
        {
            buyNoAdsButton.interactable = false;
            disabledNoAdsButtonFilter.SetActive(true);
        }
    }

    private void OnDisable()
    {
        PlayerPrefsX.SetBool("noSound", noSound);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefsX.SetBool("noSound", noSound);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefsX.SetBool("noSound", noSound);
        }
    }

    public void DisableBuyNoAdsButton()
    {
        buyNoAdsButton.interactable = false;
        disabledNoAdsButtonFilter.SetActive(true);
    }

    public void RestorePurchases()
    {
        purchaser.RestorePurchases();
    }
}
