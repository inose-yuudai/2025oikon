using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RhymeButtonManager : MonoBehaviour
{
    [SerializeField] private RhymeChecker rhymeChecker;
    [SerializeField] private PlayerWordData playerWordData;
    [SerializeField] private RhymeButton[] rhymeButtons;

    /// <summary>
    ///敵の単語に基づいてボタンをセットアップ
    /// </summary>
    public void SetupButtonsForTurn(string[] enemyWords)
    {
        if (playerWordData.playerWordPairs.Length < 4 || rhymeButtons.Length < 4)
        {
            Debug.LogWarning("十分なデータがありません");
            return;
        }

        // 韻が合う単語リスト
        var correctWords = playerWordData.playerWordPairs
            .Where(pair => enemyWords.Any(enemy => rhymeChecker.IsRhyme(enemy, pair.internalWord, 3)))
            .OrderBy(_ => Random.value)
            .Take(2)
            .ToArray();

        // 韻が合わない単語リスト
        var wrongWords = playerWordData.playerWordPairs
            .Where(pair => !enemyWords.Any(enemy => rhymeChecker.IsRhyme(enemy, pair.internalWord, 3)))
            .OrderBy(_ => Random.value)
            .Take(2)
            .ToArray();

        // 正解と不正解を混ぜてランダム配置
        var allWords = correctWords.Concat(wrongWords).OrderBy(_ => Random.value).ToArray();

        // ボタンにセット
        for (int i = 0; i < rhymeButtons.Length; i++)
        {
            rhymeButtons[i].SetWord(allWords[i]);
        }
    }

    /// <summary>
    /// ボタンを表示/非表示にする(ゲームオブジェクトごと)
    /// </summary>
    public void SetButtonsVisible(bool isVisible)
    {
        foreach (var button in rhymeButtons)
        {
            button.gameObject.SetActive(isVisible);
        }
    }

    /// <summary>
    /// ボタンの状態リセット
    /// </summary>
    public void ResetAllButtons()
    {
        foreach (var button in rhymeButtons)
        {
            button.ResetButtonState();
        }
    }

    /// <summary>
    /// ボタンの「押せる／押せない」を一括制御
    /// （RhymeButton 内で interactable プロパティを用意している場合）
    /// </summary>
    public void SetButtonsInteractable(bool canInteract)
    {
        foreach (var btn in rhymeButtons)
        {
            // ★ ここでエラーになる場合は、
            //   RhymeButton に public bool interactable { ... } が実装されているかを確認する
            btn.interactable = canInteract;
        }
    }
}
