using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoginController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public CustomInput customInput;
    public CustomText nearText;
    public CustomText modalTitleText;
    public GameObject loadingObj;
    public GameObject loginButtonObj;
    public GameObject authButtonObj;
    public GameObject modalWindowObj;
    public NearHelper nearhelper;
    public EventSystem eventSystem;
    //public bool simulateError;
    private string playerAccountName;
    private void Start()
    {
        if (BaseUtils.offlineMode)
        {
            Database.databaseStruct.playerAccount = "Offline";
            OnReceiveLoginAuthorise(true);
        }
        mainMenuController.UpdateBackground();
    }
    public void Setup()
    {
        loadingObj.SetActive(true);
        customInput.Setup();
        customInput.OnPointerClick(null);
        eventSystem.SetSelectedGameObject(customInput.gameObject);
        nearText.SetString(nearhelper.Testnet ? ".testnet" : ".near");
    }
    public void ResetLogin()
    {
        Setup();
        customInput.enabled = true;
        customInput.ResetInput();
        authButtonObj.SetActive(false);
        loginButtonObj.SetActive(true);
    }
    //#protocol
    public void OnLoginClick()
    {
        /*
        if (customInput.typeString == "")
        {
            mainMenuController.ShowWarning("Error!", "Address cannot be empty.", "Enter your Near Wallet adress.");
            return;
        }
        if (customInput.typeString.Length >= 64)
        {
            mainMenuController.ShowWarning("Error!", "Address not supported. Contact support", "If you wish to use non near accounts.");
            return;
        }
        playerAccountName = customInput.typeString.ToLower();
        customInput.enabled = false;
        */

        //nearhelper.Login(playerAccountName);
        
        nearhelper.WalletSelectorLogin();
        
        //if (!skipPopup)
        //{
        //mainMenuController.ShowLoading();
        //StartCoroutine(WaitAndShowAuthButton());
        //Application.OpenURL("https://pixelpets.pixeldapps.co/");
        //}
    }
    private IEnumerator WaitAndShowAuthButton()
    {
        yield return new WaitForSeconds(1);
        authButtonObj.SetActive(true);
        loginButtonObj.SetActive(false);
    }
    //player clicked the authorized button after the login
    public void OnAuthClick()
    {
        nearhelper.CheckForValidLogin();
    }
    public void OnLoginError(int errorType)
    {
        switch (errorType)
        {
            //not authorized in near page
            case 0:
                mainMenuController.ShowWarning("Error!", "Login info was not authorized.", "Please try again!");
                break;
            //wrong account name
            case 1:
                mainMenuController.ShowWarning("Error!", "Wallet adress is not correct.", "Please try again!");
                break;
            //uknown/server error
            case 2:
                mainMenuController.ShowWarning("Error!", "The servers are acting weird!", "Please try again later.");
                break;
        }
        mainMenuController.HideLoading();
        loginButtonObj.SetActive(true);
        authButtonObj.SetActive(false);
        customInput.enabled = true;
        customInput.ResetInput();
    }
    //#Dan, needs some kind of bool as argument here to know if player needs to purchase modal
    public void OnReceiveLoginAuthorise(bool hasModal)
    {
        mainMenuController.HideLoading();
        loadingObj.SetActive(false);
        BaseUtils.syncedDatabase = true;
        if (hasModal)
        {
            mainMenuController.Setup();
        }
        else
        {
            modalTitleText.SetString($"Welcome to pixel pets, {Database.databaseStruct.playerAccount}!");
            modalWindowObj.SetActive(true);
        }
    }
    //#Dan -> player clicked accept purchase here
    public void OnAcceptModalPurchase()
    {
        //Application.OpenURL("https://pixelpets.pixeldapps.co/");
        mainMenuController.ShowLoading();
    }
    //#Dan -> call this once the purchase has been authorised
    public void OnReceivePurchaseAuthorise()
    {
        modalWindowObj.SetActive(false);
        mainMenuController.Setup();
    }
}
