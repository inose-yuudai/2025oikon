using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text resultText; // 判定結果を表示するテキスト

    private int currentScore = 0;

    /// <summary>
    /// スコアを追加
    /// </summary>
    /// <param name="value">加算するスコア値</param>
    public void AddScore(int value, string result = "")
    {
        currentScore += value;
        UpdateScoreUI();

        // 判定結果があればUIに表示
        if (!string.IsNullOrEmpty(result))
        {
            ShowResult(result);
        }
    }

    /// <summary>
    /// スコアUIを更新
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }

    /// <summary>
    /// 判定結果を表示
    /// </summary>
    /// <param name="result">判定結果の文字列</param>
    private void ShowResult(string result)
    {
        if (resultText != null)
        {
            resultText.text = result;

            // 判定結果を一時的に表示し、消すためのコルーチンを呼び出す
            StopAllCoroutines(); // 前回のコルーチンを停止
            StartCoroutine(HideResultAfterDelay());
        }
    }

    /// <summary>
    /// 判定結果を一定時間後に消す
    /// </summary>
    private System.Collections.IEnumerator HideResultAfterDelay()
    {
        yield return new WaitForSeconds(1.0f); // 1秒後に非表示
        if (resultText != null)
        {
            resultText.text = "";
        }
    }
}