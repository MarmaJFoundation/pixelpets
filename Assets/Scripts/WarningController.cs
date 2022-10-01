using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningController : MonoBehaviour
{
    public CustomText titleText;
    public CustomText bodyText1;
    public CustomText bodyText2;
    public GameObject noButton;
    private MainMenuController mainMenuController;
    private System.Action<bool> callback;
    public void Setup(MainMenuController mainMenuController, string titleString, string bodyString1, string bodyString2, System.Action<bool> callback)
    {
        this.mainMenuController = mainMenuController;
        this.callback = callback;
        gameObject.SetActive(true);
        titleText.SetString(titleString);
        bodyText1.SetString(bodyString1);
        bodyText2.SetString(bodyString2);
        noButton.SetActive(callback != null);
    }
    public void OnAcceptClick()
    {
        gameObject.SetActive(false);
        mainMenuController.showingWarn = false;
        if (callback != null)
        {
            callback.Invoke(true);
        }
    }
    public void OnDenyClick()
    {
        gameObject.SetActive(false);
        mainMenuController.showingWarn = false;
        if (callback != null)
        {
            callback.Invoke(false);
        }
    }
}
