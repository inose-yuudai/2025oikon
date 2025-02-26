using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening; // DOTween の名前空間

public class EnemyWordPresenter : MonoBehaviour
{
    [SerializeField] private EnemyWordsData[] enemyWordsDataList; // 複数の EnemyWordsData
    [SerializeField] private Text enemyWordText1;
    [SerializeField] private Text enemyWordText2;
    [SerializeField] private RectTransform speechBubble1; // 吹き出し用 Transform（Text の背後に配置）
    [SerializeField] private RectTransform speechBubble2;
    [SerializeField] private AudioSource audioSource;

    private EnemyWordsData currentEnemyWordsData; // 現在使用中の EnemyWordsData
    private EnemyWordPair[] currentEnemyWordPairs = new EnemyWordPair[2];

    public RhymeButton rhymeButton; // ライムボタン

    public string[] CurrentEnemyWords => new string[]
    {
        currentEnemyWordPairs[0].internalWord,
        currentEnemyWordPairs[1].internalWord
    };

    public AudioClip[] CurrentEnemyVoiceClips => new AudioClip[]
    {
        currentEnemyWordPairs[0].voiceClip,
        currentEnemyWordPairs[1].voiceClip
    };

    public AudioSource AudioSource => audioSource;

    private void Start()
    {
        if (enemyWordsDataList.Length > 0)
        {
            SetEnemyWordsData(enemyWordsDataList[0]); // 初期値
        }
        else
        {
            Debug.LogWarning("EnemyWordsData が設定されていません");
        }
    }

    /// <summary>
    /// 使用する EnemyWordsData を設定
    /// </summary>
    public void SetEnemyWordsData(EnemyWordsData newEnemyWordsData)
    {
        if (newEnemyWordsData == null)
        {
            Debug.LogWarning("無効な EnemyWordsData が指定されました");
            return;
        }
        currentEnemyWordsData = newEnemyWordsData;
    }

    /// <summary>
    /// 敵の単語を2つランダムに選んで表示し、アニメーションで出現させる
    /// </summary>
    public void ShowRandomEnemyWords()
    {
        if (currentEnemyWordsData == null || currentEnemyWordsData.enemyWordPairs.Length < 2)
        {
            Debug.LogWarning("EnemyWordsData に十分な単語がありません");
            return;
        }
        rhymeButton.HideCurrentBubble(); // 現在の吹き出しを非表示にする

        // 単語をシャッフルして2つ選択
        var shuffledPairs = currentEnemyWordsData.enemyWordPairs.OrderBy(x => Random.value).ToArray();
        currentEnemyWordPairs[0] = shuffledPairs[0];
        currentEnemyWordPairs[1] = shuffledPairs[1];

        // 表示用テキストにセット
        if (enemyWordText1 != null) enemyWordText1.text = currentEnemyWordPairs[0].displayWord;
        if (enemyWordText2 != null) enemyWordText2.text = currentEnemyWordPairs[1].displayWord;

        // 各オブジェクトの元のスケールを取得して目標値とする
        if (enemyWordText1 != null)
        {
            Vector3 originalScale = enemyWordText1.transform.localScale;
            enemyWordText1.transform.localScale = Vector3.zero;
            enemyWordText1.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);
        }
        if (enemyWordText2 != null)
        {
            Vector3 originalScale = enemyWordText2.transform.localScale;
            enemyWordText2.transform.localScale = Vector3.zero;
            enemyWordText2.transform.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);
        }
        if (speechBubble1 != null)
        {
            Vector3 originalScale = speechBubble1.localScale;
            speechBubble1.localScale = Vector3.zero;
            speechBubble1.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);
        }
        if (speechBubble2 != null)
        {
            Vector3 originalScale = speechBubble2.localScale;
            speechBubble2.localScale = Vector3.zero;
            speechBubble2.DOScale(originalScale, 0.5f).SetEase(Ease.OutBack);
        }
    }

    /// <summary>
    /// 敵のテキスト1の Transform を取得
    /// </summary>
    public Transform GetEnemyWordText1Transform()
    {
        return enemyWordText1?.transform;
    }

    /// <summary>
    /// 敵のテキスト2の Transform を取得
    /// </summary>
    public Transform GetEnemyWordText2Transform()
    {
        return enemyWordText2?.transform;
    }
}
