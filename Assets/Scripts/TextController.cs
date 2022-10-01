using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public RectTransform rectTransform;
    public CustomText customText;

    private FightController fightController;
    public void Setup(FightController fightController, Vector3 fromPos, string textString, Color goColor, int damage)
    {
        this.fightController = fightController;
        gameObject.SetActive(true);
        customText.SetString(textString, goColor);
        fromPos += Vector3.up * 20;
        fightController.activeTexts.Add(this);
        StartCoroutine(FloatAndDisappear(fromPos, damage));
    }
    private IEnumerator FloatAndDisappear(Vector3 fromPos, int damage)
    {
        float timer = 0;
        float goScale = 2 + Mathf.Clamp(damage * .001f, 0, .4f);
        Vector3 randomPos = Vector3.right * BaseUtils.RandomInt(20, 40) * BaseUtils.RandomSign();
        Vector3 upPos = Vector3.up * BaseUtils.RandomInt(30, 40);
        while (timer <= 1)
        {
            rectTransform.localPosition = Vector3.Lerp(fromPos, fromPos + upPos + randomPos, timer.Evaluate(CurveType.EaseOut));
            rectTransform.localScale = Vector3.Lerp(Vector3.one * goScale * .75f, Vector3.one * goScale, timer.Evaluate(CurveType.PeakCurve));
            timer += Time.deltaTime * 1.5f;
            yield return null;
        }
        for (int i = 0; i < 3; i++)
        {
            rectTransform.localPosition = fromPos + upPos + randomPos;
            yield return new WaitForSeconds(.05f / (i + 1));
            rectTransform.localPosition = Vector3.left * 9000;
            yield return new WaitForSeconds(.05f / (i + 1));
        }
        fightController.activeTexts.Remove(this);
        Dispose();
    }
    public void Dispose()
    {
        gameObject.SetActive(false);
        fightController.textPool.Enqueue(this);
    }
}
