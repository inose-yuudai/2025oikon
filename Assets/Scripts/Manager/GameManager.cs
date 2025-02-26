using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyWordPresenter enemyWordPresenter;
    [SerializeField] private RhymeButtonManager rhymeButtonManager;
    [SerializeField] private RhymeChecker rhymeChecker;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TimeManager timeManager;

    [Header("EnemyWordsData の管理")]
    [SerializeField] private EnemyWordsData[] enemyWordsDataList; // 複数の EnemyWordsData

    // エフェクト関連
    [SerializeField] private RapidPunchEffect punchEffectController;       // 正解エフェクト
    [SerializeField] private RapidPunchEffect incorrectEffectController;   // 不正解エフェクト
    [SerializeField] private AudioClip correctSound;                       // 正解音

    [Header("エフェクト表示位置")]
    [SerializeField] private Vector3 leftEffectPosition = new Vector3(-5, 0, 0);
    [SerializeField] private Vector3 rightEffectPosition = new Vector3(5, 0, 0);

    [Header("エフェクト間隔(秒)")]
    [SerializeField] private float effectDelay = 0.5f;

    private string[] currentEnemyWords = new string[2]; // 敵が出す2つの単語
    private string[] playerSelections = new string[2];  // プレイヤーが選んだ2つの単語
    private int currentSelectionIndex = 0;              // プレイヤーの選択順序を追跡
    private bool isPlayingAudio = false;                // 再生中フラグ
    private bool isGameStart = false;                   // ゲーム再生フラグ

    /// <summary>
    /// EnemyWordsData を指定してセット
    /// </summary>
    public void SetEnemyWordsData(EnemyWordsData newData)
    {
        if (newData == null)
        {
            Debug.LogWarning("無効な EnemyWordsData が指定されました");
            return;
        }
        enemyWordPresenter.SetEnemyWordsData(newData);
        Debug.Log($"EnemyWordsData を {newData.name} にセットしました");
    }

    /// <summary>
    /// 任意のタイミングで呼び出される新しいターンの開始
    /// </summary>
    public void StartNewTurn()
    {
        Debug.Log("新しいターンを開始します");
        if (!isGameStart)
        {
            timeManager.StartTimer();
            isGameStart = true;
        }
        currentSelectionIndex = 0;

        // 敵の単語を2つ選んで表示
        enemyWordPresenter.ShowRandomEnemyWords();
            Debug.Log("ゲームを開始します");
        currentEnemyWords = enemyWordPresenter.CurrentEnemyWords;

        Debug.Log($"敵の単語: {currentEnemyWords[0]}, {currentEnemyWords[1]}");

        // 韻が合う選択肢を含むボタンをセットアップ
        rhymeButtonManager.SetupButtonsForTurn(currentEnemyWords);

        // ボタンを押せない状態にしておく
        rhymeButtonManager.SetButtonsInteractable(false);

        // 敵ボイスを順番に再生（再生終了後にボタンを押せるようにする）
        StartCoroutine(PlayEnemyVoices());
    }

    /// <summary>
    /// ボイスを順番に再生するコルーチン
    /// </summary>
    private IEnumerator PlayEnemyVoices()
    {
        isPlayingAudio = true;

        var enemyClips = enemyWordPresenter.CurrentEnemyVoiceClips;
        var audioSource = enemyWordPresenter.AudioSource;

        foreach (var clip in enemyClips)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
            }
        }

        isPlayingAudio = false;

        // ボタンを押せるようにする
        rhymeButtonManager.SetButtonsInteractable(true);
    }

    /// <summary>
    /// プレイヤーが選択した単語を処理
    /// </summary>
    public void OnPlayerSelect(string selectedWord)
    {
        if (isPlayingAudio)
        {
            Debug.LogWarning("再生中は選択できません");
            return;
        }

        if (currentSelectionIndex >= 2)
        {
            Debug.LogWarning("すでに2つの選択が完了しています");
            return;
        }

        playerSelections[currentSelectionIndex] = selectedWord;
        currentSelectionIndex++;

        if (currentSelectionIndex == 2)
        {
            rhymeButtonManager.SetButtonsInteractable(false);
            StartCoroutine(EvaluateSelectionsAndShowEffects());
        }
    }

    /// <summary>
    /// 選択結果を評価し、正解/不正解ごとにエフェクトを表示
    /// </summary>
    private IEnumerator EvaluateSelectionsAndShowEffects()
    {
        bool[] isCorrect = new bool[2];

        for (int i = 0; i < 2; i++)
        {
            isCorrect[i] = rhymeChecker.IsRhyme(currentEnemyWords[i], playerSelections[i], 3);
            Debug.Log(
                isCorrect[i]
                    ? $"選択 {i + 1}: 正解！ (Enemy: {currentEnemyWords[i]}, Player: {playerSelections[i]})"
                    : $"選択 {i + 1}: 不正解 (Enemy: {currentEnemyWords[i]}, Player: {playerSelections[i]})"
            );
        }

        ShowEffect(isCorrect[0], leftEffectPosition);
        yield return new WaitForSeconds(effectDelay);

        ShowEffect(isCorrect[1], rightEffectPosition);

        if (isCorrect[0] && isCorrect[1] && correctSound != null)
        {
            var audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(correctSound);
            }
        }

        yield return new WaitForSeconds(effectDelay);

        rhymeButtonManager.ResetAllButtons();
        StartNewTurn();
    }

    /// <summary>
    /// 正解/不正解に応じたエフェクトを表示
    /// </summary>
    private void ShowEffect(bool isCorrect, Vector3 position)
    {
        RapidPunchEffect effectController = isCorrect ? punchEffectController : incorrectEffectController;

        if (effectController != null)
        {
            effectController.transform.position = position;
            if (isCorrect)
            {
                effectController.PlayCorrectEffect();
            }
            else
            {
                effectController.PlayIncorrectEffect();
            }
        }
    }
}
