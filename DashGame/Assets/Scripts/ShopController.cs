using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public class Item
    {
        public Color originalColor;
        public Gradient originalGradient;
        public Gradient maxGradient; //for random color between two gradients option for particle system
        public Gradient minGradient; //for random color between two gradients option for particle system
        public Color maxColor;//for random color between two colors option for particle system
        public Color minColor;//for random color between two colors option for particle system
        public GameObject gameObject;
        public bool unlocked;
        public int gemCost;
        public bool selected;
        public ParticleSystem.MainModule PSMainMod;
        public ParticleSystem particleSystem = null;
        public SpriteRenderer spriteRenderer = null;
        ShopController shopC = ShopController.Instance;
        public ShopController.buttonLayout buttonLayout;
        public int index;
        public bool ball; // true if ball, false if paddle
        public bool valueChanged;
        public string name;
        public bool particleShowcase;

        public Item(int gemCost, GameObject GO, bool ball, bool particleShowcase, bool defaultItem = false)
        {
            gameObject = GO;
            name = GO.name;
            this.particleShowcase = particleShowcase;
            this.ball = ball;

            if (defaultItem)
            {
                unlocked = true;
                selected = PlayerPrefsX.GetBool(name + "Selected");

                if (selected)
                    buttonLayout = buttonLayout.selected;
                else
                    buttonLayout = buttonLayout.unlocked;
            }
            else
            {
                this.gemCost = gemCost;
                unlocked = PlayerPrefsX.GetBool(name);
                selected = PlayerPrefsX.GetBool(name + "Selected");

                if (!unlocked)
                {
                    buttonLayout = buttonLayout.locked;
                }
                else
                {
                    buttonLayout = buttonLayout.unlocked;
                }

                if (selected)
                {
                    buttonLayout = buttonLayout.selected;
                }

                if (!particleShowcase)
                {
                    spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    originalColor = spriteRenderer.color;
                    if (!unlocked)
                    {
                        spriteRenderer.color = new Color32(61, 61, 61, 255);
                    }
                }
                else
                {
                    particleSystem = gameObject.GetComponent<ParticleSystem>();
                    PSMainMod = particleSystem.main;
                    if (PSMainMod.startColor.mode == ParticleSystemGradientMode.Color)
                    {
                        originalColor = PSMainMod.startColor.color;
                        if (!unlocked)
                        {
                            PSMainMod.startColor = new ParticleSystem.MinMaxGradient(new Color32(61, 61, 61, 255));
                        }
                    }
                    if (PSMainMod.startColor.mode == ParticleSystemGradientMode.TwoGradients)
                    {
                        maxGradient = PSMainMod.startColor.gradientMax;
                        minGradient = PSMainMod.startColor.gradientMin;
                        if (!unlocked)
                        {
                            PSMainMod.startColor = new ParticleSystem.MinMaxGradient(shopC.blackGrayGradient, shopC.blackGrayGradient);
                        }
                    }
                    if (PSMainMod.startColor.mode == ParticleSystemGradientMode.Gradient)
                    {
                        originalGradient = PSMainMod.startColor.gradient;
                        if (!unlocked)
                        {
                            PSMainMod.startColor = new ParticleSystem.MinMaxGradient(shopC.blackGrayGradient);
                        }
                    }
                    if (PSMainMod.startColor.mode == ParticleSystemGradientMode.TwoColors)
                    {
                        maxColor = PSMainMod.startColor.colorMax;
                        minColor = PSMainMod.startColor.colorMin;
                        if (!unlocked)
                        {
                            PSMainMod.startColor = new ParticleSystem.MinMaxGradient(Color.black, new Color32(61, 61, 61, 255));
                        }
                    }
                }
            }
        }

        public void Unlock() //no need to update valuechanged here since unlock is called from select() below
        {
            unlocked = true;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            else
            {
                if (PSMainMod.startColor.mode == ParticleSystemGradientMode.Color)
                {
                    PSMainMod.startColor = originalColor;
                }
                if (PSMainMod.startColor.mode == ParticleSystemGradientMode.TwoGradients)
                {
                    PSMainMod.startColor = new ParticleSystem.MinMaxGradient(minGradient, maxGradient);
                }
                if (PSMainMod.startColor.mode == ParticleSystemGradientMode.Gradient)
                {
                    PSMainMod.startColor = originalGradient;
                }
                if (PSMainMod.startColor.mode == ParticleSystemGradientMode.TwoColors)
                {
                    PSMainMod.startColor = new ParticleSystem.MinMaxGradient(minColor, maxColor);
                }

                particleSystem.Clear();
                particleSystem.Simulate(10);
                particleSystem.Play();
            }
        }

        public void Select()
        {
            if (!unlocked)
            {
                Unlock();
            }
            valueChanged = true;

            selected = true;

            buttonLayout = buttonLayout.selected;

            if (ball)
            {
                BallController.Instance.SetBall(index);
            }
            else
            {
                GameManager.Instance.SetPaddle(index);
            }
        }

        public void UnSelect()
        {
            selected = false;
            valueChanged = true;

            buttonLayout = buttonLayout.unlocked;
        }

        public void SetPlayerPrefs()
        {
            if (valueChanged)
            {
                PlayerPrefsX.SetBool(name, unlocked);
                PlayerPrefsX.SetBool(name + "Selected", selected);
                valueChanged = false;
            }
        }
    }

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
    public Text NoAdsPrice;
    public Text GemChestPrice;
    public Transform Balls4Sale;
    public Transform Paddles4Sale;
    public GameObject ballSelectionMenu;
    public GameObject paddleSelectionMenu;
    public GameObject IAPmenu;
    public GameObject comingSoonNorm;
    public GameObject comingSoonMad;

    [HideInInspector]
    public int selectedBallIndex, selectedPaddleIndex;

    static Item[] Balls;
    static Item[] Paddles;
    int currentMenu = 1; // 1 for ballmenu and 2 for paddle menu and 3 for IAP menu
    Text purchaseButtonPrice;
    Text unlockButtonGemCost;
    SnapScrollRectController ballScrollRect, paddleScrollRect;
    Purchaser purchaser;
    GameManager game;
    AdManager ads;
    AudioManager audioManager;
    BallController ballC;
    bool noAds = false;
    string ballPrice, premiumBallPrice;
    Button comingSoonButton;
    Coroutine comingSoonCoroutine;
    int item2Purchase = -1; // apple does not pause game during purchase process, so use a different item purchase reference for balls, paddles, etc. just in case
    ShopItem[] ballshopItems, paddleshopItems;
    Item tempItem;
    int tempFocal;

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

        comingSoonButton = paddleSelectionMenu.transform.GetChild(0).GetComponent<Button>();

        ballScrollRect = ballSelectionMenu.GetComponent<SnapScrollRectController>();
        paddleScrollRect = paddleSelectionMenu.GetComponent<SnapScrollRectController>();
        unlockButtonGemCost = unlockButton.GetComponentInChildren<Text>();
        purchaseButtonPrice = purchaseButton.GetComponentInChildren<Text>();

        ballPrice = "$0.99";
        premiumBallPrice = "$1.99";

        disabledNoAdsButtonFilter.SetActive(false);
    }

    private void Start()
    {
        game = GameManager.Instance;
        ads = AdManager.Instance;
        purchaser = Purchaser.Instance;
        audioManager = AudioManager.Instance;
        ballC = BallController.Instance;

        noAds = PlayerPrefsX.GetBool("noAds");
        if (noAds)
        {
            buyNoAdsButton.interactable = false;
            disabledNoAdsButtonFilter.SetActive(true);
        }

        Balls = new Item[Balls4Sale.childCount];
        ballshopItems = Balls4Sale.GetComponentsInChildren<ShopItem>();
        ShopItem ballshopItem = null;
        for (int i = 0; i < Balls4Sale.childCount; i++)
        {
            if (i == 0)
            {
                ballshopItem = ballshopItems[i];
                Balls[i] = new Item(ballshopItem.GemCost, ballshopItem.gameObject, true, ballshopItem.ParticleShowcase, true);
            }
            else
            {
                ballshopItem = ballshopItems[i];
                Balls[i] = new Item(ballshopItem.GemCost, ballshopItem.gameObject, true, ballshopItem.ParticleShowcase);
            }
            Balls[i].index = ballC.Link2BallItem(Balls[i].name);
        }

        selectedBallIndex = ZPlayerPrefs.GetInt("selectedBallIndex", 0);
        if (selectedBallIndex == 0)
        {
            Balls[0].Select();
        }
    }

    public void UnselectOtherItem()
    {
        if (currentMenu == 1) //for balls
        {
            for (int i = 0; i < Balls.Length; i++)
            {
                if (Balls[i].selected)
                {
                    Balls[i].UnSelect();
                    break;
                }
            }
        }
        else if (currentMenu == 2) //for paddles
        {
            for (int i = 0; i < Paddles.Length; i++)
            {
                if (Paddles[i].selected)
                {
                    Paddles[i].UnSelect();
                    break;
                }
            }
        }
    }

    public void SelectItem()
    {
        if (currentMenu == 1) //for balls
        {
            tempFocal = ballScrollRect.focalItemNum;
            if (Balls[tempFocal].unlocked)
            {
                audioManager.PlayUISound("selectItem");

                UnselectOtherItem();
                Balls[tempFocal].Select();
                selectedBallIndex = tempFocal;
            }
            else
            {
                if (Balls[tempFocal].gemCost <= game.Gems)
                {
                    audioManager.PlayUISound("unlockItem");

                    UnselectOtherItem();
                    Balls[tempFocal].Select();
                    selectedBallIndex = tempFocal;

                    game.UpdateGems(Balls[tempFocal].gemCost, true);
                }
            }
        }
        else if (currentMenu == 2) //for paddles
        {
            tempFocal = paddleScrollRect.focalItemNum;
            if (Paddles[tempFocal].unlocked)
            {
                audioManager.PlayUISound("selectItem");

                UnselectOtherItem();
                Paddles[tempFocal].Select();
                selectedPaddleIndex = tempFocal;
            }
            else
            {
                if (Paddles[tempFocal].gemCost <= game.Gems)
                {
                    audioManager.PlayUISound("unlockItem");

                    UnselectOtherItem();
                    Paddles[tempFocal].Select();
                    selectedPaddleIndex = tempFocal;

                    game.UpdateGems(Paddles[tempFocal].gemCost, true);
                }
            }
        }
    }

    public void RequestItemPurchase()
    {
        if (currentMenu == 1) //for balls
        {
            if (item2Purchase == -1)
            {
                item2Purchase = ballScrollRect.focalItemNum;

                if (Balls[item2Purchase].gemCost == 500)
                {
                    purchaser.BuyBall();
                }
                else
                {
                    purchaser.BuyPremiumBall();
                }
            }
        }
        else if (currentMenu == 2) //for paddles
        {
            if (item2Purchase == -1)
            {
                item2Purchase = paddleScrollRect.focalItemNum;

                if (Paddles[item2Purchase].gemCost == 500)
                {
                    purchaser.BuyBall();
                }
                else
                {
                    purchaser.BuyPremiumBall();
                }
            }
        }
    }

    public void PurchaseItem()
    {
        UnselectOtherItem();
        if (currentMenu == 1) //for balls
        {
            Balls[item2Purchase].Select();
            selectedBallIndex = item2Purchase;
            item2Purchase = -1;
        }
        else if (currentMenu == 2) //for paddles
        {
            Paddles[item2Purchase].Select();
            selectedPaddleIndex = item2Purchase;
            item2Purchase = -1;
        }
    }

    public void SetLocalizedPrices()
    {
        NoAdsPrice.text = purchaser.NoAdsPrice();
        GemChestPrice.text = purchaser.GemChestPrice();
        ballPrice = purchaser.BallPrice();
        premiumBallPrice = purchaser.PremiumBallPrice();
    }

    public void SetButtonLayout(int itemIndex)
    {
        if (currentMenu == 1)
        {
            tempItem = Balls[itemIndex];
        }
        //else
        //{
        //    tempItem1 = Paddles[itemIndex];
        //}

        switch (tempItem.buttonLayout)
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

                unlockButtonGemCost.text = tempItem.gemCost.ToString();
                if (tempItem.gemCost == 500)
                {
                    purchaseButtonPrice.text = ballPrice;
                }
                else
                {
                    purchaseButtonPrice.text = premiumBallPrice;
                }
                break;
        }
    }

    public void GoToBallSelection()
    {
        StopComingSoonShake();

        audioManager.PlayUISound("switchPage");

        currentMenu = 1;

        viewBallsButton.interactable = false;
        viewPaddlesButton.interactable = true;
        viewIAPButton.interactable = true;

        ballSelectionMenu.SetActive(true);
        paddleSelectionMenu.SetActive(false);
        IAPmenu.SetActive(false);

        ballScrollRect.Go2Selected();
    }

    // Edit this function once you add paddles skins
    public void GoToPaddleSelection()
    {
        // take this out once you add paddles skins----------------------------------------------------------
        comingSoonNorm.SetActive(true);
        comingSoonMad.SetActive(false);
        //--------------------------------------------------------------------------------------------------

        audioManager.PlayUISound("switchPage");

        currentMenu = 2;

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
        StopComingSoonShake();

        audioManager.PlayUISound("switchPage");

        currentMenu = 3;

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
        StopComingSoonShake();

        currentMenu = 1;

        viewBallsButton.interactable = false;
        viewPaddlesButton.interactable = true;
        viewIAPButton.interactable = true;

        ballSelectionMenu.SetActive(true);
        paddleSelectionMenu.SetActive(false);
        IAPmenu.SetActive(false);

        ballScrollRect.Go2Selected();
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

    public void ShakeButton(RectTransform rectTransform)
    {
        audioManager.PlayUISound("comingSoon");

        comingSoonCoroutine = StartCoroutine(ShakeUIElement(rectTransform));
    }

    public IEnumerator ShakeUIElement(RectTransform rectTransform)
    {
        comingSoonButton.interactable = false;
        comingSoonNorm.SetActive(false);
        comingSoonMad.SetActive(true);

        Vector3 originalPos = rectTransform.localPosition;
        float elapsedTime = 0;
        float CameraShakeIntensity = 1.5f;
        float shakeDuration = 1.436f;

        while (elapsedTime < 1.5f)
        {
            elapsedTime += Time.deltaTime;

            float percentComplete = elapsedTime / shakeDuration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= Mathf.PerlinNoise(x, y) * CameraShakeIntensity * damper;
            y *= Mathf.PerlinNoise(x, y) * CameraShakeIntensity * damper;

            rectTransform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            yield return null;
        }

        rectTransform.localPosition = originalPos;

        comingSoonNorm.SetActive(true);
        comingSoonMad.SetActive(false);
        comingSoonButton.interactable = true;
    }

    void StopComingSoonShake()
    {
        if (comingSoonCoroutine != null)
        {
            StopCoroutine(comingSoonCoroutine);
        }
        comingSoonButton.interactable = true;
        audioManager.PlayUISound("comingSoon", true);
    }

    public void SetPlayerPrefs4Items()
    {
        foreach (Item ball in Balls)
        {
            ball.SetPlayerPrefs();
        }

        ZPlayerPrefs.SetInt("selectedBallIndex", selectedBallIndex);
    }

    public void OnApplicationQuit()
    {
        SetPlayerPrefs4Items();
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SetPlayerPrefs4Items();
        }
    }

    public void OnDisable()
    {
        SetPlayerPrefs4Items();
    }
}
