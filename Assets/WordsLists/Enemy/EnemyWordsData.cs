using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWordsData", menuName = "ScriptableObjects/EnemyWordsData", order = 1)]
public class EnemyWordsData : ScriptableObject
{
    public EnemyWordPair[] enemyWordPairs;
}

[System.Serializable]
public struct EnemyWordPair
{
    public string displayWord;   // 画面に表示する文字 (例: "秋葉")
    public string internalWord;  // 判定用の文字    (例: "あきば")
    
    // ▼ 新規追加: 音声データ
    public AudioClip voiceClip;  // このセリフ(秋葉)に対応したボイス音源
}