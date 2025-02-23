using UnityEngine;
using UnityEngine.UI;

public class RhymeButton : MonoBehaviour
{
    [SerializeField] private Text buttonText;
    [SerializeField] private Image buttonBackground;  // ボタン背景部分のImage
    [SerializeField] private Color defaultColor = Color.white;   // 初期状態の色
    [SerializeField] private Color selectedColor = Color.green;  // 選択状態の色

    private string internalPlayerWord;  // 内部判定用の文字列
    private bool isSelected = false;    // ボタンが選択済みかどうか
    private Button buttonComponent;     // ボタンコンポーネント

    /// <summary>
    /// RhymeButton が持つ「ボタンを押せる／押せない」プロパティ
    /// RhymeButtonManager など外部から操作するために用意。
    /// </summary>
    public bool interactable
    {
        get => (buttonComponent != null) && buttonComponent.interactable;
        set
        {
            if (buttonComponent != null)
            {
                buttonComponent.interactable = value;
            }
        }
    }

    private void Awake()
    {
        // ボタンコンポーネントを取得
        buttonComponent = GetComponent<Button>();
        if (buttonComponent == null)
        {
            Debug.LogError("RhymeButton: Button コンポーネントがアタッチされていません");
        }
    }

    /// <summary>
    /// ボタンに単語をセット
    /// </summary>
    public void SetWord(PlayerWordPair pair)
    {
        internalPlayerWord = pair.internalWord;
        if (buttonText != null)
        {
            buttonText.text = pair.displayWord;
        }
        ResetButtonState();
    }

    /// <summary>
    /// ボタンがクリックされたときの処理
    /// </summary>
    public void OnButtonClick()
    {
        if (isSelected) return; // すでに選択されている場合は無視

        // ▼ 特別な処理：もし internalPlayerWord が "せきずい" だったら PlaySISAnimation を呼ぶ
        if (internalPlayerWord == "せきずい")
        {
            // Scene 上にある PlaySISAnimation コンポーネントを探す
            var sisAnimator = FindObjectOfType<PlaySISAnimation>();
            if (sisAnimator != null)
            {
                // アニメーションを再生し、その終了後に他の処理を実行
                StartCoroutine(PlayAnimationAndContinue(sisAnimator));
            }
            else
            {
                Debug.LogWarning("PlaySISAnimation をアタッチしたオブジェクトが見つかりませんでした。");
                ExecuteOtherProcesses(); // アニメーションが見つからない場合でも続行
            }
        }
        else
        {
            // 通常の処理をそのまま実行
            ExecuteOtherProcesses();
        }
    }

    /// <summary>
    /// アニメーションを再生し、その後に処理を続行するコルーチン
    /// </summary>
    private System.Collections.IEnumerator PlayAnimationAndContinue(PlaySISAnimation sisAnimator)
    {
        sisAnimator.PlayAnimation(); // アニメーション再生
        yield return new WaitForSeconds((float)sisAnimator.playableDirector.duration); // アニメーションが終わるまで待機
        ExecuteOtherProcesses(); // アニメーション終了後に処理を続行
    }

    /// <summary>
    /// 既存の処理をまとめたメソッド
    /// </summary>
    private void ExecuteOtherProcesses()
    {
        isSelected = true;
        ChangeButtonColor(selectedColor); // ボタンの色を選択色に変更

        // ボタンを押せないようにする
        if (buttonComponent != null)
        {
            buttonComponent.interactable = false;
        }

        // GameManager に選択内容を伝える
        var gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnPlayerSelect(internalPlayerWord);
        }
    }


    /// <summary>
    /// ボタンの色を変更
    /// </summary>
    private void ChangeButtonColor(Color color)
    {
        if (buttonBackground != null)
        {
            buttonBackground.color = color;
        }
        else
        {
            Debug.LogWarning("RhymeButton: buttonBackground が設定されていません");
        }
    }

    /// <summary>
    /// ボタンの状態をリセット
    /// </summary>
    public void ResetButtonState()
    {
        isSelected = false;
        ChangeButtonColor(defaultColor); // ボタンの色を初期色に戻す

        // ボタンを再度押せるようにする
        if (buttonComponent != null)
        {
            buttonComponent.interactable = true;
        }
    }
}
