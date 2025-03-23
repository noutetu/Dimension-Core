using UnityEngine;

// ==========================================
// バトル中のテキスト表示を管理するクラス
// ==========================================
public class ShapeTextHandler : MonoBehaviour
{
    // UI キャンバス
    private Canvas uiCanvas;
    // ==========================================
    // 初期化
    // ==========================================
    private void Awake()
    {
        // UI キャンバスを取得
        uiCanvas = MainMessage.Instance.uiCanvas;
        if (uiCanvas == null)
        {
            DebugUtility.LogError("ShapeTextManager: uiCanvas が設定されていません！");
        }
    }

    // ------------------------------------------------
    // テキスト表示
    // ------------------------------------------------
    private void ShowText(string text, Color color, bool isEnemy, bool isDamageText = false)
    {
        // テキスト表示用のオブジェクトを取得
        EffectText effectTextObj = EffectTextPool.Instance.GetFromPool();

        if (effectTextObj == null || uiCanvas == null)
        {
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
        // 座標を設定
        effectTextObj.GetComponent<RectTransform>().anchoredPosition = uiPos;
        // テキストを設定
        effectTextObj.SetBattleText(text, color, isEnemy, isDamageText);
    }

    // ------------------------------------------------
    // 各種状況別の表示メソッド
    // ------------------------------------------------
    
    // ==========================================
    // 回避時のテキスト表示
    // ==========================================
    public void ShowDodge()
    {
        ShowText("Dodge!", Color.yellow, isEnemy: false);
        // isEnemy は本来呼び出し元の情報が必要な場合もあるので、引数にするかどうか要検討
    }
    // ==========================================
    // クリティカル時のテキスト表示
    // ==========================================
    public void ShowCritDamage(float damage, bool isEnemy)
    {
        ShowText($"CRIT! {Mathf.FloorToInt(damage)}", Color.magenta, isEnemy, true);
    }
    // ==========================================
    // 通常ダメージ時のテキスト表示
    // ==========================================
    public void ShowNormalDamage(float damage, bool isEnemy)
    {
        // 味方か敵かで色を変えたい場合はこのメソッド内で条件分岐する
        Color color = isEnemy ? Color.white : Color.red;
        // ダメージ表示
        ShowText($"{Mathf.FloorToInt(damage)}", color, isEnemy, true);
    }
    // ==========================================
    // 回復時のテキスト表示
    // ==========================================
    public void ShowHeal(float healValue, bool isEnemy)
    {
        ShowText($"Heal! {Mathf.FloorToInt(healValue)}", Color.green, isEnemy);
    }
    // ==========================================
    // バフ時のテキスト表示
    // ==========================================
    public void ShowBuff(bool isEnemy)
    {
        ShowText("Buff!", Color.yellow, isEnemy);
    }
    // ==========================================
    // デバフ時のテキスト表示
    // ==========================================
    public void ShowDebuff(bool isEnemy)
    {
        ShowText("Debuff!", Color.gray, isEnemy);
    }

    // ===========================================
    // Singularityのモードチェンジ時のテキスト表示
    // ===========================================
    public void ShowCurrentMode(bool isDefence)
    {
        ShowText(isDefence ? "Defence Mode!" : "Attack Mode!", Color.cyan, isEnemy: false);
    }
}

