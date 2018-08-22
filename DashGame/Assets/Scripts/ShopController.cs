using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;
    public Gradient blackGrayGradient;
    public Button viewBallsButton;
    public Button viewPaddlesButton;
    public Button viewIAPButton;

    GameObject ballSelectionMenu;
    GameObject paddleSelectionMenu;
    GameObject IAPmenu;

    public enum buttonLayout { selected, unlocked, locked }

    private void Awake()
    {
        Instance = this;

        ballSelectionMenu = transform.Find("BallScollPanel").gameObject;
        paddleSelectionMenu = transform.Find("PaddleScollPanel").gameObject;
        IAPmenu = transform.Find("IAP").gameObject;
    }

    public void SetButtonLayout(SnapScrollRectController.ShopItem item)
    {
        switch (item.buttonLayout)
        {
            case buttonLayout.selected:

                break;

            case buttonLayout.unlocked:

                break;

            case buttonLayout.locked:

                break;

        }
    }

    public void GoToBallSelection()
    {
        viewBallsButton.interactable = false;
        viewPaddlesButton.interactable = true;
        viewIAPButton.interactable = true;

        ballSelectionMenu.SetActive(true);
        paddleSelectionMenu.SetActive(false);
        IAPmenu.SetActive(false);
    }

    public void GoToPaddleSelection()
    {
        viewBallsButton.interactable = true;
        viewPaddlesButton.interactable = false;
        viewIAPButton.interactable = true;

        ballSelectionMenu.SetActive(false);
        paddleSelectionMenu.SetActive(true);
        IAPmenu.SetActive(false);
    }

    public void GoToIAP()
    {
        viewBallsButton.interactable = true;
        viewPaddlesButton.interactable = true;
        viewIAPButton.interactable = false;

        ballSelectionMenu.SetActive(false);
        paddleSelectionMenu.SetActive(false);
        IAPmenu.SetActive(true);
    }

    public void Go2Shop()
    {
        viewBallsButton.interactable = false;
        viewPaddlesButton.interactable = true;
        viewIAPButton.interactable = true;

        ballSelectionMenu.SetActive(true);
        paddleSelectionMenu.SetActive(false);
        IAPmenu.SetActive(false);
    }

}
