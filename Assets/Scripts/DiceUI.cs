using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DiceUI : MonoBehaviour
{
    [Header("Тексты (Вывод)")]
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text diceCountText;
    [SerializeField] private TMP_Text tieScoreText;
    [SerializeField] private TMP_Text winScoreText;
    [SerializeField] private TMP_Text loseScoreText;
    [SerializeField] private TMP_Text resultText;

    [Header("Поля ввода (InputField)")]
    [SerializeField] private TMP_InputField diceCountInput;
    [SerializeField] private TMP_InputField tieScoreInput;
    [SerializeField] private TMP_InputField winScoreInput;
    [SerializeField] private TMP_InputField loseScoreInput;

    [Header("Кнопки")]
    [SerializeField] private Button rerollButton;

    public event Action<int, int, int, int> OnRerollClicked;

    public event Action<int> OnScoreUpdated;
    
    // 3. Ивент "Количество кубиков изменилось"
    public event Action<int> OnDiceCountUpdated;

    public event Action<int, int, int> OnTargetsUpdated;

    private void Awake()
    {
        if (rerollButton != null)
        {
            rerollButton.onClick.AddListener(HandleRerollButtonClick);
        }
    }

    private void OnEnable()
    {
        OnScoreUpdated += UpdateTotalScoreText;
        OnDiceCountUpdated += UpdateDiceCountText;
        OnTargetsUpdated += UpdateTargetsText;
    }

    private void OnDisable()
    {
        OnScoreUpdated -= UpdateTotalScoreText;
        OnDiceCountUpdated -= UpdateDiceCountText;
        OnTargetsUpdated -= UpdateTargetsText;
    }

    private void HandleRerollButtonClick()
    {
        int diceCount = ParseInput(diceCountInput.text, 2, 1, 6);
        int tieScore = ParseInput(tieScoreInput.text, 10, 0, 100);
        int winScore = ParseInput(winScoreInput.text, 20, 0, 100);
        int loseScore = ParseInput(loseScoreInput.text, 5, 0, 100);

        OnRerollClicked?.Invoke(diceCount, tieScore, winScore, loseScore);
    }

    private int ParseInput(string text, int defaultValue, int min, int max)
    {
        if (int.TryParse(text, out int result))
            return Mathf.Clamp(result, min, max);
        return defaultValue;
    }

    private void UpdateTotalScoreText(int score)
    {
        if (totalScoreText != null) totalScoreText.text = $"Очки: {score}";
    }

    private void UpdateDiceCountText(int count)
    {
        if (diceCountText != null) diceCountText.text = $"Кубиков: {count}";
    }

    private void UpdateTargetsText(int tie, int win, int lose)
    {
        if (tieScoreText != null) tieScoreText.text = $"Ничья: {tie}";
        if (winScoreText != null) winScoreText.text = $"Победа: {win}";
        if (loseScoreText != null) loseScoreText.text = $"Поражение: {lose}";
    }

    public void TriggerScoreUpdate(int score) => OnScoreUpdated?.Invoke(score);
    public void TriggerDiceCountUpdate(int count) => OnDiceCountUpdated?.Invoke(count);
    public void TriggerTargetsUpdate(int tie, int win, int lose) => OnTargetsUpdated?.Invoke(tie, win, lose);

        public void ShowRoundResult(int totalScore, int winScore, int loseScore)
    {
        if (totalScore > winScore) 
        {
            if (resultText != null) 
            {
                resultText.text = "ПОБЕДА!";
                resultText.color = Color.green;
            }
        }
        else if (totalScore < loseScore) 
        {
            if (resultText != null) 
            {
                resultText.text = "ПОРАЖЕНИЕ!";
                resultText.color = Color.red;
            }
        }
        else 
        {
            if (resultText != null) 
            {
                resultText.text = "НИЧЬЯ!";
                resultText.color = new Color(1f, 0.5f, 0f); 
            }
        }
    }

    public void SetTargetInputs(int tie, int win, int lose)
    {
        if (tieScoreInput != null) tieScoreInput.text = tie.ToString();
        if (winScoreInput != null) winScoreInput.text = win.ToString();
        if (loseScoreInput != null) loseScoreInput.text = lose.ToString();
    }
}