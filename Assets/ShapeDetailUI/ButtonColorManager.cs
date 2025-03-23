using System.Collections.Generic;
// ===============================
// ボタンの色を管理するクラス
// ===============================
public class ButtonColorManager
{
    // ボタンのリスト
    private List<ShapeButton> buttons;
    // ===============================
    // コンストラクタ
    // ===============================
    public ButtonColorManager(List<ShapeButton> buttons)
    {
        this.buttons = buttons;
    }
    // ===============================
    // ボタンの色をリセットする
    // ===============================
    public void ResetButtonColors()
    {
        foreach (var button in buttons)
        {
            button.ResetColor();
        }
    }
    // ===============================
    // 選択されたボタンをハイライトする
    // ===============================
    public void HighlightSelectedButton(ShapeBaseStats shape)
    {
        // ボタンのリストをループして、選択されたボタンをハイライトする
        foreach (var button in buttons)
        {
            if (button.shapeData == shape)
            {
                button.SelectedColor();
                break;
            }
        }
    }
    // ===============================
    // ボタンのリストを更新する
    // ===============================
    public void UpdateButtons(List<ShapeButton> newButtons)
    {
        this.buttons = newButtons;
    }
}
