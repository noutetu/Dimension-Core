using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    /// <summary>
    /// タイトル画面へ遷移
    /// </summary>
    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    /// <summary>
    /// ゲーム画面へ遷移
    /// </summary>
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// ゲームを終了
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
