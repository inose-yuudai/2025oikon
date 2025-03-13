using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RhymeButtonManager : MonoBehaviour
{
    [SerializeField] private RhymeChecker rhymeChecker;
    [SerializeField] private PlayerWordData playerWordData;
    [SerializeField] private RhymeButton[] rhymeButtons;

    /// <summary>
    /// 敵の単語に基づいてボタンをセットアップ
    /// </summary>
public void SetupButtonsForTurn(string[] enemyWords)
{
    // スペースキーが押されている場合は、全てのボタンにspecialワードをセットする
    if (ShouldCallSpecialWords())
    {
        // specialWordPairsが空でないかチェック
        if (playerWordData.specialWordPairs.Length == 0)
        {
            Debug.LogWarning("SpecialWordPairsが存在しません");
            return;
        }
        
        for (int i = 0; i < rhymeButtons.Length; i++)
        {
            // ランダムに選択（被り可）
            var randomIndex = Random.Range(0, playerWordData.specialWordPairs.Length);
            var specialWord = playerWordData.specialWordPairs[randomIndex];
            rhymeButtons[i].SetWord(specialWord);
        }
        return;
    }

    // スペースキーが押されていない場合は、従来通りの処理
    if (playerWordData.playerWordPairs.Length < 4 || rhymeButtons.Length < 4)
    {
        Debug.LogWarning("十分なデータがありません");
        return;
    }

    // 通常は、specialWordが取得された場合は1件、そうでなければ2件正解単語を選ぶ
    var specialWords = GetSpecialWords(enemyWords);
    int correctWordCount = specialWords.Length > 0 ? 1 : 2;

    // 韻が合う単語リスト
    var correctWords = playerWordData.playerWordPairs
        .Where(pair => enemyWords.Any(enemy => rhymeChecker.IsRhyme(enemy, pair.internalWord, 3)))
        .OrderBy(_ => Random.value)
        .Take(correctWordCount)
        .ToArray();

    // 韻が合わない単語リスト
    var wrongWords = playerWordData.playerWordPairs
        .Where(pair => !enemyWords.Any(enemy => rhymeChecker.IsRhyme(enemy, pair.internalWord, 3)))
        .OrderBy(_ => Random.value)
        .Take(2)
        .ToArray();

    // 正解・スペシャル・不正解を混ぜてランダム配置
    var allWords = specialWords.Concat(correctWords).Concat(wrongWords)
        .OrderBy(_ => Random.value)
        .ToArray();

    // ボタンにセット
    for (int i = 0; i < rhymeButtons.Length; i++)
    {
        rhymeButtons[i].SetWord(allWords[i]);
    }
}


    /// <summary>
    /// スペシャルワードを取得する
    /// </summary>
    private PlayerWordPair[] GetSpecialWords(string[] enemyWords)
    {
        if (ShouldCallSpecialWords())
        {
            // 条件を満たしている場合は、enemyWordsの韻チェックはせずに全specialWordPairsからランダムに1件取得
            var specialWords = playerWordData.specialWordPairs
                .OrderBy(_ => Random.value)
                .Take(1)
                .ToArray();
            return specialWords;
        }

        return new PlayerWordPair[0];
    }

    /// <summary>
    /// スペシャルワードを呼ぶべきかどうかを判定する
    /// </summary>
    private bool ShouldCallSpecialWords()
    {
        // 例: スペースキーが押されていたら true を返す
        var shouldCall = Input.GetKey(KeyCode.Space);

        return shouldCall;
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
            // ★ ここでエラーになる場合は、 RhymeButton に public bool interactable { ... } が実装されているかを確認する
            btn.interactable = canInteract;
        }
    }
}
