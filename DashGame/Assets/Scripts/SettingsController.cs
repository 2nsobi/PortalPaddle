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
    public GameObject MusicOnIcon;
    public GameObject MusicOffIcon;

    Purchaser purchaser;
    BallController ballC;
    AudioManager audioManager;

    bool noSound;
    bool noAds;
    bool noMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        disabledNoAdsButtonFilter.SetActive(false);
    }

    private void Start()
    {
        purchaser = Purchaser.Instance;
        ballC = BallController.Instance;
        audioManager = AudioManager.Instance;

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

        noMusic = PlayerPrefsX.GetBool("noMusic");
        if (noMusic)
        {
            MusicOffIcon.SetActive(true);
            MusicOnIcon.SetActive(false);
        }
        else
        {
            MusicOffIcon.SetActive(false);
            MusicOnIcon.SetActive(true);
        }

        noAds = PlayerPrefsX.GetBool("noAds");
        if (noAds)
        {
            buyNoAdsButton.interactable = false;
            disabledNoAdsButtonFilter.SetActive(true);
        }
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

    public void ToggleMusic()
    {
        audioManager.PlayUISound("switchPage");

        noMusic = !noMusic;
        if (noMusic)
        {
            MusicOffIcon.SetActive(true);
            MusicOnIcon.SetActive(false);
        }
        else
        {
            MusicOffIcon.SetActive(false);
            MusicOnIcon.SetActive(true);
        }

        audioManager.SetNoMusic(noMusic);
    }

    private void OnDisable()
    {
        PlayerPrefsX.SetBool("noSound", noSound);
        PlayerPrefsX.SetBool("noMusic", noMusic);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefsX.SetBool("noSound", noSound);
        PlayerPrefsX.SetBool("noMusic", noMusic);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefsX.SetBool("noSound", noSound);
            PlayerPrefsX.SetBool("noMusic", noMusic);
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
