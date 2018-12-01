using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SnapScrollRectController : MonoBehaviour
{
    public RectTransform content2Scroll;
    public RectTransform position2Snap2;

    [HideInInspector]
    public int focalItemNum; // the index of the item that is closest to the postion2snap2 rect transform

    RectTransform[] items;
    string[] shopItems;
    ScrollRect scrollRect;
    float[] distances2SnapPositon;
    float itemSeperation;
    float minDistance;
    float newX;
    bool swiping = false;
    bool snap2Start = false;
    bool snap2End = false;
    Coroutine stopMovement;
    ShopController shopC;
    Purchaser purchaser;
    GameManager game;
    AudioManager audioManager;
    int ball2Purchase;
    int currentMenu; // 1=ballmenu, 2=paddlemenu

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        if (this.gameObject.name == "BallScrollPanel")
        {
            currentMenu = 1;
        }
        if (this.gameObject.name == "PaddleScrollPanel")
        {
            currentMenu = 2;
        }

        shopItems = new string[content2Scroll.childCount];
        items = new RectTransform[content2Scroll.childCount];
        for (int i = 0; i < content2Scroll.childCount; i++)
        {
            shopItems[i] = content2Scroll.GetChild(i).name;
            items[i] = content2Scroll.GetChild(i).GetComponent<RectTransform>();
        }

        distances2SnapPositon = new float[items.Length];

        itemSeperation = Mathf.Abs(items[0].anchoredPosition.x - items[1].anchoredPosition.x);
    }

    private void Start()
    {
        shopC = ShopController.Instance;
        game = GameManager.Instance;
        purchaser = Purchaser.Instance;
        audioManager = AudioManager.Instance;
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

        shopC.SetButtonLayout(focalItemNum);

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

    public void Go2Selected()
    {
        if (currentMenu == 1)
        {
            content2Scroll.anchoredPosition = new Vector2(shopC.selectedBallIndex * -itemSeperation, 0);
        }
        else
        {
            content2Scroll.anchoredPosition = new Vector2(shopC.selectedPaddleIndex * -itemSeperation, 0);
        }
    }
}
