using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapScrollRectController : MonoBehaviour
{

    public RectTransform content2Scroll;
    public RectTransform position2Snap2;

    RectTransform[] items;
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

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.horizontalNormalizedPosition = 1;

        items = new RectTransform[content2Scroll.childCount];
        for (int i = 0; i < content2Scroll.childCount; i++)
        {
            items[i] = content2Scroll.GetChild(i).GetComponent<RectTransform>();
        }

        distances2SnapPositon = new float[items.Length];

        itemSeperation = Mathf.Abs(items[0].anchoredPosition.x - items[1].anchoredPosition.x);
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
            scrollRect.StopMovement();
            snap2End = true;
        }

        if (content2Scroll.anchoredPosition.x > 0)
        {
            scrollRect.StopMovement();
            snap2Start = true;
        }
    }

}
