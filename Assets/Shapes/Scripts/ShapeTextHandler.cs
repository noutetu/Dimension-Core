using UnityEngine;

// ==========================================
// バトル中のテキスト表示を管理するクラス
// ==========================================
public class ShapeTextHandler : MonoBehaviour
{
    private Canvas uiCanvas;

    private void Awake()
    {
        uiCanvas = SystemMessage.Instance.uiCanvas; 
        if (uiCanvas == null)
        {
            DebugUtility.LogError("ShapeTextManager: uiCanvas が設定されていません！");
        }
    }

    // ------------------------------------------------
    // 汎用メソッド（内部で共通処理を行う）
    // ------------------------------------------------
    private void ShowText(string text, Color color, bool isEnemy, bool isDamageText = false)
    {
        EffectText effectTextObj = EffectTextPool.Instance.GetFromPool();

        if (effectTextObj == null || uiCanvas == null)
        {
            DebugUtility.LogError("ShapeTextManager: EffectTextPrefab または uiCanvas が設定されていません！");
            return;
        }

        // テキストを表示する座標を計算（Shape の座標 + 少し上）
        Vector3 worldPos = transform.position + Vector3.up * 1.5f;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiCanvas.GetComponent<RectTransform>(),
            Camera.main.WorldToScreenPoint(worldPos),
            uiCanvas.worldCamera,
            out Vector2 uiPos
        );

        effectTextObj.GetComponent<RectTransform>().anchoredPosition = uiPos;
        effectTextObj.SetBattleText(text, color, isEnemy, isDamageText);
    }

    // ------------------------------------------------
    // 各種状況別の表示メソッド
    // ------------------------------------------------
    public void ShowDodge()
    {
        ShowText("Dodge!", Color.yellow, isEnemy: false); 
        // isEnemy は本来呼び出し元の情報が必要な場合もあるので、引数にするかどうか要検討
    }

    public void ShowCritDamage(float damage, bool isEnemy)
    {
        ShowText($"CRIT! {Mathf.FloorToInt(damage)}", Color.magenta, isEnemy, true);
    }

    public void ShowNormalDamage(float damage, bool isEnemy)
    {
        // 味方か敵かで色を変えたい場合はこのメソッド内で条件分岐する
        Color color = isEnemy ? Color.white : Color.red;
        ShowText($"{Mathf.FloorToInt(damage)}", color, isEnemy, true);
    }

    public void ShowHeal(float healValue, bool isEnemy)
    {
        ShowText($"Heal! {Mathf.FloorToInt(healValue)}", Color.green, isEnemy);
    }

    public void ShowBuff(bool isEnemy)
    {
        ShowText("Buff!", Color.yellow, isEnemy);
    }

    public void ShowDebuff(bool isEnemy)
    {
        ShowText("Debuff!", Color.gray, isEnemy);
    }

    // 上記以外にもバフ・デバフに応じてメソッド追加など
    // ===========================================
    // Singularityのモードチェンジ時のテキスト表示
    // ===========================================
    public void ShowCurrentMode(bool isDefence)
    {
        ShowText(isDefence ? "Defence Mode!" : "Attack Mode!", Color.cyan, isEnemy: false);
    }

}

