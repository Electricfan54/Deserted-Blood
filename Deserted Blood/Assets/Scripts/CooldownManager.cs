using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;

public class CooldownManager : MonoBehaviour
{
    [SerializeField] Image CDMeter;

    public void AddCoolDown(string text, float CDTimer)
    {
        CDMeter.gameObject.SetActive(true);
        StartCoroutine(AddCD(CDTimer));
        CDMeter.GetComponentInChildren<TextMeshProUGUI>().text = text;

    }


    IEnumerator AddCD(float CDTimer)
    {
        float curTime = 0f;
        CDMeter.fillAmount = 1f;

        while(curTime < CDTimer)
        {
            curTime += Time.deltaTime;
            CDMeter.fillAmount = Mathf.Lerp(1f, 0f, curTime / CDTimer);
            yield return null;
        }

        CDMeter.fillAmount = 0f;
        CDMeter.gameObject.SetActive(false);
    }

}
