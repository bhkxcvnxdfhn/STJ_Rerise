using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TeachPanel : MonoBehaviour
{
    public Transform panel;
    public Vector3 offset = Vector3.zero;
    public float durationTime = 0.5f;

    private Tween scaleTween;
    private Tween moveTween;
    private Vector3 oriScale;

    private void Start()
    {
        oriScale = panel.localScale;
        panel.position = transform.position;
        panel.localScale = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SwitchPanel(oriScale, offset);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SwitchPanel(Vector3.zero, Vector3.zero);
    }

    private void SwitchPanel(Vector3 scale, Vector3 offset)
    {
        //����Ĳ�oTween�� �R�����e��Tween
        if (scaleTween != null)
        {
            scaleTween.Kill();
            moveTween.Kill();
        }
        //���O�Y��
        scaleTween = panel.DOScale(scale, durationTime);
        //���O����
        moveTween = panel.DOLocalMove(offset, durationTime);
    }
}
