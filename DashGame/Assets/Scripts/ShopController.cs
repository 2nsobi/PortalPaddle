using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;
    public Gradient blackGrayGradient;
    public Button viewBallsButton;
    public Button viewPaddlesButton;
    public Button viewIAPButton;
    public Button selectButton;
    public GameObject selectedIcon;
    public Button unlockButton;
    public Button purchaseButton;
    public Button buyGemsButton;
    public Button buyNoAdsButton;
    public GameObject disabledNoAdsButtonFilter;

    string currentMenu = null;
    Text purchaseButtonPrice;
    Text unlockButtonGemCost;
    GameObject ballSelectionMenu;
    GameObject paddleSelectionMenu;
    GameObject IAPmenu;
    SnapScrollRectController ballScrollRect, paddleScrollRect;
    Purchaser purchaser;
    GameManager game;
    AdManager ads;
    bool noAds = false;

    public enum buttonLayout { selected, unlocked, locked }

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

        ballSelectionMenu = transform.Find("BallScollPanel").gameObject;
        paddleSelectionMenu = transform.Find("TempPaddleMenu").gameObject; //paddleSelectionMenu = transform.Find("PaddleScollPanel").gameObject;
        IAPmenu = transform.Find("IAPMenu").gameObject;

        ballScrollRect = ballSelectionMenu.GetComponent<SnapScrollRectController>();
        paddleScrollRect = paddleSelectionMenu.GetComponent<SnapScrollRectController>();
        unlockButtonGemCost = unlockButton.GetComponentInChildren<Text>();
        purchaseButtonPrice = purchaseButton.GetComponentInChildren<Text>();
        purchaseButtonPrice.text = "$0.99";

        noAds = PlayerPrefsX.GetBool("noAds");

        disabledNoAdsButtonFilter.SetActive(false);
        if (noAds)
        {
            buyNoAdsButton.interactable = false;
            disabledNoAdsButtonFilter.SetActive(true);
        }
    }

    private void Start()
    {
        game = GameManager.Instance;
        ads = AdManager.Instance;
        purchaser = Purchaser.Instance;
    }

    public void SetButtonLayout(SnapScrollRectController.ShopItem item,int gemCost)
    {
        switch (item.buttonLayout)
        {
            case buttonLayout.selected:
                selectedIcon.SetActive(true);
                selectButton.gameObject.SetActive(false);
                unlockButton.gameObject.SetActive(false);
                purchaseButton.gameObject.SetActive(false);
                break;

            case buttonLayout.unlocked:
                selectedIcon.SetActive(false);
                selectButton.gameObject.SetActive(true);
                unlockButton.gameObject.SetActive(false);
                purchaseButton.gameObject.SetActive(false);
                break;

            case buttonLayout.locked:
                selectedIcon.SetActive(false);
                selectButton.gameObject.SetActive(false);
                unlockButton.gameObject.SetActive(true);
                purchaseButton.gameObject.SetActive(true);

                unlockButtonGemCost.text = gemCost.ToString();
                if (gemCost == 500)
                {
                    purchaseButtonPrice.text = "$0.99";
                }
                else
                {
                    purchaseButtonPrice.text = "$1.99";
                }
                break;
        }
    }

    public void GoToBallSelection()
    {
        currentMenu = "ball";

        viewBallsButton.interactable = false;
        viewPaddlesButton.interactable = true;
        viewIAPButton.interactable = true;

        ballSelectionMenu.SetActive(true);
        paddleSelectionMenu.SetActive(false);
        IAPmenu.SetActive(false);

        ballScrollRect.Go2Shop();
        ballScrollRect.Go2Selected();
    }

    //public void GoToPaddleSelection()
    //{
    //    currentMenu = "paddle";

    //    viewBallsButton.interactable = true;
    //    viewPaddlesButton.interactable = false;
    //    viewIAPButton.interactable = true;

    //    ballSelectionMenu.SetActive(false);
    //    paddleSelectionMenu.SetActive(true);
    //    IAPmenu.SetActive(false);

    //    paddleScrollRect.Go2Shop();
    //    paddleScrollRect.Go2Selected();
    //}

    public void GoToPaddleSelection()
    {
        currentMenu = "paddle";

        viewBallsButton.interactable = true;
        viewPaddlesButton.interactable = false;
        viewIAPButton.interactable = true;

        selectedIcon.SetActive(false);
        selectButton.gameObject.SetActive(false);
        unlockButton.gameObject.SetActive(false);
        purchaseButton.gameObject.SetActive(false);

        ballSelectionMenu.SetActive(false);
        paddleSelectionMenu.SetActive(true);
        IAPmenu.SetActive(false);
    }

    public void GoToIAP()
    {
        currentMenu = "IAP";

        viewBallsButton.interactable = true;
        viewPaddlesButton.interactable = true;
        viewIAPButton.interactable = false;

        selectedIcon.SetActive(false);
        selectButton.gameObject.SetActive(false);
        unlockButton.gameObject.SetActive(false);
        purchaseButton.gameObject.SetActive(false);

        ballSelectionMenu.SetActive(false);
        paddleSelectionMenu.SetActive(false);
        IAPmenu.SetActive(true);
    }

    public void Go2Shop()
    {
        currentMenu = "ball";

        viewBallsButton.interactable = false;
        viewPaddlesButton.interactable = true;
        viewIAPButton.interactable = true;

        ballSelectionMenu.SetActive(true);
        paddleSelectionMenu.SetActive(false);
        IAPmenu.SetActive(false);

        ballScrollRect.Go2Shop();
        ballScrollRect.Go2Selected();
    }

    public void SelectItem()
    {
        if (currentMenu == "ball")
        {
            ballScrollRect.SelectItem();
        }
        else if (currentMenu == "paddle")
        {
            paddleScrollRect.SelectItem();
        }
    }

    public void Request2BuyItem()
    {
        if (currentMenu == "ball")
        {
            ballScrollRect.RequestPurchase();
        }
        else if (currentMenu == "paddle")
        {
            paddleScrollRect.RequestPurchase();
        }
    }

    public void BuyItem()
    {
        if (currentMenu == "ball")
        {
            ballScrollRect.BuyItem();
        }
        else if (currentMenu == "paddle")
        {
            paddleScrollRect.BuyItem();
        }
    }

    public void GetFreeGems()
    {
        ads.ShowRewardVideo();
    }

    public void Buy1800Gems()
    {
        purchaser.Buy1800GemChest();
    }

    public void BuyNoAds()
    {
        purchaser.BuyNoAds();
    }

    public void DisableBuyNoAdsButton()
    {
        buyNoAdsButton.interactable = false;
        disabledNoAdsButtonFilter.SetActive(true);
    }
}
