using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HpBar : MonoBehaviour
{
    void Start()
    {
        // �ڭ�X�{! �q�\��q��s
        GameManager.instance.Act_HpChange += UpdateHpBar;
    }

    private void OnDestroy()
    {
        // ��ڱ��F���ɭ�! ����h�q��q��s
        GameManager.instance.Act_HpChange -= UpdateHpBar;
    }

    [SerializeField] Image bar = null;
    void UpdateHpBar(float input)
    {
        bar.fillAmount = input;
    }
}



