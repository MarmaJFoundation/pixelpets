using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SungenController : MonoBehaviour
{
    public CustomText timerText;
    public RectTransform timerBar;
    public GameObject getEggButton;
    public GameObject titleText;
    public GameObject sungeonWindow;
    public MainMenuController mainMenuController;
    public EggController eggController;
    public NearHelper nearHelper;
    public void Setup()
    {
        timerText.UpdateText();
    }
    public void OnCardClick()
    {
        sungeonWindow.SetActive(true);
    }
    public void OnExitClick()
    {
        sungeonWindow.SetActive(false);
    }
    //number ranges from 0 to 2.592.000
    public void UpdateSungenTimer(float sungenTimer)
    {
        if (sungenTimer <= 0)
        {
            getEggButton.SetActive(true);
            titleText.SetActive(false);
            timerText.gameObject.SetActive(false);
            timerBar.localScale = new Vector3(0, 1, 1);
            return;
        }
        getEggButton.SetActive(false);
        titleText.SetActive(true);
        timerText.gameObject.SetActive(true);
        timerText.SetString($"Time left: {sungenTimer.GetNiceTime()}");
        timerBar.localScale = new Vector3(Mathf.Clamp(sungenTimer / 2592000f, .01f, 1), 1, 1);
    }
    public void OnGetEggClick()
    {
        getEggButton.SetActive(false);
        StartCoroutine(nearHelper.RequestSungenEgg());
    }
    public void SungenEggCallback(bool success, long eggTimestamp)
    {
        if (success)
        {
            Database.databaseStruct.eggAmount[2]++;
            Database.databaseStruct.sungenTimer = TimestampHelper.GetDateTimeFromTimestamp(eggTimestamp);
            mainMenuController.ShowWarning("Sucess!", "You received a free epic egg", "hatch it when you can.");
        }
        else
        {
            mainMenuController.ShowWarning($"Error on egg get", $"The request for a sungen egg has failed", $"please try again!");
            getEggButton.SetActive(true);
        }
    }
}
