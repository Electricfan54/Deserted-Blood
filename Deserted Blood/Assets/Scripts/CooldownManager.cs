using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;

public class CooldownManager : MonoBehaviour
{
    [SerializeField] Image CDMeter;
    [SerializeField] Image CDMeter2;

    public void AddCoolDown(string text, float CDTimer)
    {
        if(!CDMeter.IsActive())
        {
            CDMeter.gameObject.SetActive(true);
            StartCoroutine(AddCD(CDTimer, CDMeter));
            CDMeter.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }
        else
        {
            CDMeter2.gameObject.SetActive(true);
            StartCoroutine(AddCD(CDTimer, CDMeter2));
            CDMeter2.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }
  

    }


    IEnumerator AddCD(float CDTimer, Image meter)
    {
        float curTime = 0f;
        meter.fillAmount = 1f;

        while(curTime < CDTimer)
        {
            curTime += Time.deltaTime;
            meter.fillAmount = Mathf.Lerp(1f, 0f, curTime / CDTimer);
            yield return null;
        }

        meter.fillAmount = 0f;
        meter.gameObject.SetActive(false);
    }

}
