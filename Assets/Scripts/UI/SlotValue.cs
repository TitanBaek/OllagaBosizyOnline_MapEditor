using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class SlotValue : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    private BlockSlot slot;
    private Image blockIcon;
    private Color blockIconColor;

    private string platformName;
    public string PlatformName { get { return platformName; } set { platformName = value; } }

    private Coroutine zoomCoroutine;
    private Coroutine zoomOutCoroutine;

    private void Start()
    {
        slot = GetComponentInParent<BlockSlot>();   // �� ������ ��� �ִ� �θ� BlockSlot�� ����
        blockIcon = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == InputButton.Left)
        {
            if(platformName == GameManager.Data.SelectedBlock)          // ���õ� ���� Ŭ������ ���
            {
                slot.ClearSlotScale();
                DisableBlock();
                GameManager.Data.SelectedBlock = "";
            } else                                                      // ���ʼ��� �Ǵ� ���õ��� ���� ���� Ŭ������ ���
            {
                // �ٸ� �̹����� ũ�� �ʱ�ȭ
                slot.ClearSlotScale();
                // �� �̹��� ũ�� Ű���
                EnableBlock();
                // Data �Ŵ����� ���� ���õ� �÷��� �� ����
                GameManager.Data.SelectedBlock = platformName;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       zoomCoroutine = StartCoroutine(iconZoom(0.2f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        zoomOutCoroutine = StartCoroutine(iconZoomOut(0.2f));
    }

    IEnumerator iconZoom(float delay)
    {
        float coroutineCoolTime = 0f;

        while (coroutineCoolTime < delay)
        {
            gameObject.transform.localScale = Vector2.Lerp(new Vector2(1f,1f),new Vector2(1.3f,1.3f), coroutineCoolTime * 5f);
            yield return new WaitForEndOfFrame();
            coroutineCoolTime += Time.deltaTime;
        }
        coroutineCoolTime = 0f;
        StopCoroutine(zoomCoroutine);
    }

    IEnumerator iconZoomOut(float delay)
    {
        float coroutineCoolTime = 0f;

        while(coroutineCoolTime < delay)
        {
            gameObject.transform.localScale = Vector2.Lerp(new Vector2(1.3f, 1.3f), new Vector2(1f, 1f), coroutineCoolTime * 5f);
            yield return new WaitForEndOfFrame();
            coroutineCoolTime += Time.deltaTime;
        }
        coroutineCoolTime = 0f;
        StopCoroutine(zoomOutCoroutine);
    }

    public void EnableBlock()
    {
        gameObject.transform.localScale = new Vector2(1.3f, 1.3f);
        blockIconColor = blockIcon.color;
        blockIconColor.a = 1f;
        blockIcon.color = blockIconColor;
    }

    public void DisableBlock()
    {
        gameObject.transform.localScale = new Vector2(1f, 1f);
        blockIconColor = blockIcon.color;
        blockIconColor.a = 0.5f;
        blockIcon.color = blockIconColor;
    }
}
