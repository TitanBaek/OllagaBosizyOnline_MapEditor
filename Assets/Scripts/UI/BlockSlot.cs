using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BlockSlot : MonoBehaviour
{
    object[] platforms; // Resources�� ����� Block���� ���� obejct�� �迭
    List<SlotValue> slotValues;
    [SerializeField] private GameObject blockContents;

    private void Awake()
    {
        slotValues = new List<SlotValue>();
    }
    /// <summary>
    /// EditorScene���� �Ѿ���� Start�� ȣ��Ǹ鼭 ������ �ʱ�ȭ��.
    /// </summary>
    void Start()
    {
        // Resources/Platform�� ������ ������ŭ Contents�� �� �߰�
        platforms = Resources.LoadAll<GameObject>("Platforms/");    // Resources �������� ������Ʈ�� ��������
        if (platforms == null)                                      // �ƹ��͵� ���ٸ� return false�� ���̵��� ���� �ʰ�
            return;

        Debug.Log($"�� {platforms.Length} ���� ���� ����Ǿ��ֽ��ϴ�.");    // �� ��� ���� ����Ǿ��ִ��� Ȯ��

        foreach (GameObject go in platforms)
        {
            SlotValue slotValue = GameManager.Resource.Instantiate<SlotValue>("UI/SlotValue"); // Platform�� ������ŭ �������ְ� �̹��� �־��ֱ�
            Image slotValueImage = slotValue.GetComponentInChildren<Image>();
            slotValue.transform.parent = blockContents.transform;                      // Contents�� �ڽİ�ü�� ����
            slotValue.PlatformName = go.name;
            Block block = go.GetComponent<Block>();
            slotValueImage.sprite = block.BlockIcon;
            slotValues.Add(slotValue);
        }
    }

    public void ClearSlotScale()
    {
        foreach(SlotValue slotValue in slotValues)
        {
            slotValue.gameObject.transform.localScale = Vector2.one;
            Image image = slotValue.GetComponentInChildren<Image>();
            Color iconColor = image.color;
            iconColor.a = 0.5f;
            image.color = iconColor;
        }
    }
}
