using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class PlayerUI : MonoBehaviour
{
    [Header("コンテンツパネル")]
    [SerializeField] private Transform contentPanel;

    [Header("図形エントリープレハブ")]
    [SerializeField] private GameObject shapeEntryPrefab;

    [Header("図形詳細パネル")]
    [SerializeField] private GameObject shapeDetailPanel;

    private Player player;
    ShapeBaseStats selectedShape;

    private void OnEnable()
    {
        selectedShape = null;
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            DebugUtility.LogError("Playerオブジェクトが見つかりません！");
            return;
        }
        UpdatePlayerShapesUI();
        shapeDetailPanel.SetActive(false);
    }

    public void UpdatePlayerShapesUI()
    {
        ClearContentPanel();

        var groupedShapes = player.shapeManager.OwnedShapes.GroupBy(instance => instance);

        foreach (var group in groupedShapes)
        {
            CreateShapeEntry(group.Key, group.Count());
        }
    }

    private void ClearContentPanel()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateShapeEntry(ShapeBaseStats shapeStats, int shapeCount)
    {
        GameObject entryObject = Instantiate(shapeEntryPrefab, contentPanel);
        SetShapeEntryDetails(entryObject, shapeStats, shapeCount);
    }

    private void SetShapeEntryDetails(GameObject entryObject, ShapeBaseStats shapeStats, int shapeCount)
    {
        Text entryNameText = entryObject.transform.Find("NameText")?.GetComponent<Text>();
        Image iconImage = entryObject.transform.Find("IconImage")?.GetComponent<Image>();
        Text countText = entryObject.transform.Find("CountText")?.GetComponent<Text>();
        Button shapeButton = entryObject.GetComponentInChildren<Button>();

        // アイコンの名前を設定
        if(entryNameText != null)
        {
            entryNameText.text = shapeStats.name;
        }
        else
        {
            DebugUtility.LogError("Textコンポーネント（NameText）が見つかりません！");
        }

        if (iconImage != null)
        {
            iconImage.sprite = shapeStats.ShapeIcon;
            Button button = iconImage.GetComponent<Button>();
            if (button != null)
            {
                DebugUtility.Log($"Button コンポーネントが見つかりました: {shapeStats.name}");    
            }
            else
            {
                DebugUtility.LogError($"Imageコンポーネントまたは ShapeBaseStats が null: {shapeStats.name}");
            }
        }

        if (countText != null)
        {
            countText.text = $"×{shapeCount}";
        }
        if (shapeButton != null)
        {
            shapeButton.onClick.AddListener(() => ToggleShapeDetailPanel(shapeStats));
        }
    }
    private void ToggleShapeDetailPanel(ShapeBaseStats shapeStats)
    {
        if (shapeStats == selectedShape)
        {
            shapeDetailPanel.SetActive(false);
            selectedShape = null;
            return;
        }
        selectedShape = shapeStats;
        shapeDetailPanel.SetActive(true);
        shapeDetailPanel.GetComponent<UpgradePanel>().SetShapeDetail(shapeStats,ClosedShapeDetailPanel);
    }
    private void ClosedShapeDetailPanel()
    {
        selectedShape = null;
        shapeDetailPanel.SetActive(false);
    }
}
