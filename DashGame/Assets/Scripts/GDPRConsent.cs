using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;

public class GDPRConsent : MonoBehaviour
{
    public class JSONInfo { public string countryCode; }

    public GameObject GDPRCanvas;

    public Image mainPanel;
    public Image yesPanel;
    public Image noPanel;

    string[] GDPRCountries = {"AT","BE","BG","HR","CY","CZ","DK", "EE", "FI", "FR", "DE", "EL","GR","HU", "IE", "IT", "LV", "LT", "LU",
        "MT", "NL", "PL", "PT", "RO", "SK", "SI", "ES", "SE", "UK", "GB" };

    private void Start()
    {
        if (PlayerPrefsX.GetBool("answeredGDPR") == false)
        {
            StartCoroutine(GetCountryCode());
        }
        else
        {
            AdManager.Instance.InitializeAds(PlayerPrefsX.GetBool("GDPRCompliant"));
        }
    }

    IEnumerator GetCountryCode()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://ip-api.com/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string code = GetCountryCodeFromJSON(www.downloadHandler.text);

            bool foundCountry = false;
            for(int i = 0; i < GDPRCountries.Length;i++)
            {
                if (code == GDPRCountries[i])
                {
                    GDPRCanvas.SetActive(true);
                    foundCountry = true;
                    break;
                }
            }

            if (!foundCountry)
            {
                PlayerPrefsX.SetBool("answeredGDPR", true);
                PlayerPrefsX.SetBool("GDPRCompliant", true);
                AdManager.Instance.InitializeAds(true);
            }

            // Show results as text
            Debug.Log(code + ", length = " + code.Length);
        }
    }

    string GetCountryCodeFromJSON(string json)
    {
        JSONInfo info = JsonUtility.FromJson<JSONInfo>(json);
        return info.countryCode.ToUpper();
    }


    public void onYesClick()
    {
        PlayerPrefsX.SetBool("answeredGDPR",true);
        PlayerPrefsX.SetBool("GDPRCompliant", true);
        mainPanel.gameObject.SetActive(false);
        yesPanel.gameObject.SetActive(true);

        AdManager.Instance.InitializeAds(true);
    }

    public void onNoClick()
    {
        PlayerPrefsX.SetBool("answeredGDPR", true);
        PlayerPrefsX.SetBool("GDPRCompliant", false);
        mainPanel.gameObject.SetActive(false);
        noPanel.gameObject.SetActive(true);

        AdManager.Instance.InitializeAds(false);
    }

    public void onPLClick()
    {
        Application.OpenURL("https://www.appodeal.com/privacy-policy");
    }

    public void onCloseClick()
    {
        GDPRCanvas.SetActive(false);
    }
}
