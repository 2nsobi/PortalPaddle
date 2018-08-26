using UnityEngine;
using UnityEngine.UI;

public class GDPRConsent : MonoBehaviour {

    public Image mainPanel;
    public Image yesPanel;
    public Image noPanel;

    public void onYesClick()
    {
        ZPlayerPrefs.SetInt("result_gdpr", 1);
        ZPlayerPrefs.SetInt("result_gdpr_sdk", 1);
        mainPanel.gameObject.SetActive(false);
        yesPanel.gameObject.SetActive(true);

        AdManager.Instance.InitializeAds();
    }

    public void onNoClick()
    {
        ZPlayerPrefs.SetInt("result_gdpr", 1);
        ZPlayerPrefs.SetInt("result_gdpr_sdk", 0);
        mainPanel.gameObject.SetActive(false);
        noPanel.gameObject.SetActive(true);

        AdManager.Instance.InitializeAds();
    }

    public void onPLClick()
    {
        Application.OpenURL("https://www.appodeal.com/privacy-policy");
    }

    public void onCloseClick()
    {
        GameManager.Instance.GDPRConsentForm.SetActive(false);    
    }
}
