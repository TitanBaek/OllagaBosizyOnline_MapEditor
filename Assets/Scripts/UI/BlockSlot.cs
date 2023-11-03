using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BlockSlot : MonoBehaviour
{
    object[] platforms; // Resources에 저장된 Block들을 담을 obejct형 배열
    List<SlotValue> slotValues;
    [SerializeField] private GameObject blockContents;

    private void Awake()
    {
        slotValues = new List<SlotValue>();
    }
    /// <summary>
    /// EditorScene으로 넘어오면 Start가 호출되면서 슬롯이 초기화됨.
    /// </summary>
    void Start()
    {
        // Resources/Platform의 데이터 개수만큼 Contents에 값 추가
        platforms = Resources.LoadAll<GameObject>("Platforms/");    // Resources 폴더에서 오브젝트들 가져오기
        if (platforms == null)                                      // 아무것도 없다면 return false로 씬이동이 되지 않게
            return;

        Debug.Log($"총 {platforms.Length} 개의 블럭이 저장되어있습니다.");    // 총 몇개의 블럭이 저장되어있는지 확인

        foreach (GameObject go in platforms)
        {
            SlotValue slotValue = GameManager.Resource.Instantiate<SlotValue>("UI/SlotValue"); // Platform의 개수만큼 생성해주고 이미지 넣어주기
            Image slotValueImage = slotValue.GetComponentInChildren<Image>();
            slotValue.transform.parent = blockContents.transform;                      // Contents의 자식개체로 설정
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
