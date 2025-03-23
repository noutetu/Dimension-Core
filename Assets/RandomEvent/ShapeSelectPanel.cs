using UnityEngine;
// ==========================
// 図形選択パネル
// ==========================
public class ShapeSelectPanel : MonoBehaviour
{
    public void DisableEventPanel()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // メッセージ表示
        MainMessage.ShowMessage("次元が崩壊している", true, "中心次元へ急げ", false);
    }
}
