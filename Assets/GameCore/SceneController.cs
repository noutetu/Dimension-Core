using UnityEngine;
using UnityEngine.SceneManagement;
// ===========================================
// シーン遷移を管理するクラス
// ===========================================
public class SceneController : MonoBehaviour
{
    // =====================
    /// タイトル画面へ遷移
    // =====================
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    // =====================
    /// ゲーム画面へ遷移
    // =====================
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    // =====================
    /// ゲームを終了
    // =====================
    public void QuitGame()
    {
        Application.Quit();
    }
}
