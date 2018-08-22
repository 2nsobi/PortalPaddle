using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapScrollRectController : MonoBehaviour
{
    public class ShopItem
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
        public SpriteRenderer spriteRenderer;
        ShopController shopC = ShopController.Instance;
        public ShopController.buttonLayout buttonLayout;

        public ShopItem(int gemCost, GameObject GO)
        {
            this.gemCost = gemCost;
            gameObject = GO;
            unlocked = PlayerPrefsX.GetBool(gameObject.name);
            selected = PlayerPrefsX.GetBool(gameObject.name + "Selected");

            if (!unlocked)
            {
                buttonLayout = ShopController.buttonLayout.locked;
            }
            else
            {
                buttonLayout = ShopController.buttonLayout.unlocked;
            }

            if (selected)
            {
                buttonLayout = ShopController.buttonLayout.selected;
            }

            if (gameObject.name[gameObject.name.Length - 1].ToString() == "S")
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
                PSMainMod = gameObject.GetComponent<ParticleSystem>().main;
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

        public void Unlock()
        {
            PlayerPrefsX.SetBool(this.gameObject.name, true);

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
        }

        public void Select()
        {
            if (unlocked)
            {
                buttonLayout = ShopController.buttonLayout.selected;


            }
        }
    }

    public RectTransform content2Scroll;
    public RectTransform position2Snap2;

    RectTransform[] items;
    ShopItem[] shopItems;
    ScrollRect scrollRect;
    float[] distances2SnapPositon;
    float itemSeperation;
    float minDistance;
    float newX;
    int focalItemNum; // the index of the item that is closest to the postion2snap2 rect transform
    bool swiping = false;
    bool snap2Start = false;
    bool snap2End = false;
    Coroutine stopMovement;
    ShopController shopC;

    private void Start()
    {
        shopC = ShopController.Instance;

        scrollRect = GetComponent<ScrollRect>();
        scrollRect.horizontalNormalizedPosition = 1;

        items = new RectTransform[content2Scroll.childCount];
        shopItems = new ShopItem[content2Scroll.childCount];
        for (int i = 0; i < content2Scroll.childCount; i++)
        {
            items[i] = content2Scroll.GetChild(i).GetComponent<RectTransform>();
            shopItems[i] = new ShopItem(400, content2Scroll.GetChild(i).gameObject);
        }

        distances2SnapPositon = new float[items.Length];

        itemSeperation = Mathf.Abs(items[0].anchoredPosition.x - items[1].anchoredPosition.x);
    }

    public void SelectItem(ShopItem item)
    {

    }

    private void Update()
    {
        for (int i = 0; i < distances2SnapPositon.Length; i++)
        {
            distances2SnapPositon[i] = Mathf.Abs(position2Snap2.transform.position.x - items[i].transform.position.x);
        }

        minDistance = Mathf.Min(distances2SnapPositon);

        for (int i = 0; i < distances2SnapPositon.Length; i++)
        {
            if (minDistance == distances2SnapPositon[i])
            {
                focalItemNum = i;
            }
        }

        shopC.SetButtonLayout(shopItems[focalItemNum]);

        if (!swiping) // || (content2Scroll.anchoredPosition.x <= 0 && content2Scroll.anchoredPosition.x >= (distances2SnapPositon.Length - 1) * -itemSeperation))
        {
            content2Scroll.anchoredPosition = Vector2.Lerp(content2Scroll.anchoredPosition, new Vector2(focalItemNum * -itemSeperation, 0), 10 * Time.deltaTime);
        }

        if (snap2End)
        {
            content2Scroll.anchoredPosition = Vector2.Lerp(content2Scroll.anchoredPosition, new Vector2((distances2SnapPositon.Length - 1) * -itemSeperation, 0), 10 * Time.deltaTime);
            if (content2Scroll.anchoredPosition.x > (distances2SnapPositon.Length - 1) * -itemSeperation - 0.01f)
            {
                snap2End = false;
            }
        }

        if (snap2Start)
        {
            content2Scroll.anchoredPosition = Vector2.Lerp(content2Scroll.anchoredPosition, Vector2.zero, 10 * Time.deltaTime);
            if (content2Scroll.anchoredPosition.x < 0.01f)
            {
                snap2Start = false;
            }
        }
    }

    IEnumerator StopMovement()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        scrollRect.StopMovement();
    }

    public void StartSwiping()
    {
        swiping = true;
        if (stopMovement != null)
        {
            StopCoroutine(stopMovement);
        }
    }

    public void StopSwiping()
    {
        swiping = false;
        stopMovement = StartCoroutine(StopMovement());

        if (content2Scroll.anchoredPosition.x < (distances2SnapPositon.Length - 1) * -itemSeperation)
        {
            StopCoroutine(stopMovement);
            scrollRect.StopMovement();
            snap2End = true;
        }

        if (content2Scroll.anchoredPosition.x > 0)
        {
            StopCoroutine(stopMovement);
            scrollRect.StopMovement();
            snap2Start = true;
        }
    }

}
