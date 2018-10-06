using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance;

    public delegate void tutorialDelegate();
    public static event tutorialDelegate NowStartGame;

    BallController ballC;

    Image filter;
    GameObject tip1;
    GameObject tip2;
    GameObject tip3;

    bool paddleSpawned = false;
    bool canPassTip1 = false;
    bool fadeIn = false;
    bool fadeOut = false;
    float t;

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

        filter = transform.GetChild(0).GetComponent<Image>();
        tip1 = transform.GetChild(1).gameObject;
        tip2 = transform.GetChild(2).gameObject;
        tip3 = transform.GetChild(3).gameObject;
    }

    private void Start()
    {
        ballC = BallController.Instance;
    }

    private void OnEnable()
    {
        fadeIn = true;
        t = 0;

        tip1.SetActive(false);
        tip2.SetActive(false);
        tip3.SetActive(false);
    }

    IEnumerator TutorialEnum1()
    {
        tip1.SetActive(true);

        paddleSpawned = false;

        canPassTip1 = false;
        yield return new WaitForSeconds(1);
        canPassTip1 = true;
    }

    IEnumerator TutorialEnum2()
    {
        tip1.SetActive(false);
        tip2.SetActive(true);

        yield return new WaitForSeconds(3);

        tip2.SetActive(false);
        tip3.SetActive(true);

        yield return new WaitForSeconds(4);

        tip3.SetActive(false);

        fadeOut = true;
        t = 0;
    }

    private void Update()
    {
        if (canPassTip1)
        {
            if (Input.touchCount > 1)
            {
                paddleSpawned = true;
                canPassTip1 = false;

                StartCoroutine(TutorialEnum2());
            }
        }

        if (fadeIn)
        {
            t += Time.deltaTime / 0.2f;

            filter.color = new Color(0, 0, 0, Mathf.Lerp(0, 0.5254f, t));

            if (filter.color.a == 0.5254f)
            {
                fadeIn = false;
                StartCoroutine(TutorialEnum1());
            }
        }

        if (fadeOut)
        {
            t += Time.deltaTime / 0.2f;

            filter.color = new Color(0, 0, 0, Mathf.Lerp(0.5254f, 0, t));

            if (filter.color.a == 0)
            {
                PlayerPrefsX.SetBool("tutorialDisabled", true);

                NowStartGame();

                ballC.SetTutorial(true);

                gameObject.SetActive(false);
            }
        }
    }
}
