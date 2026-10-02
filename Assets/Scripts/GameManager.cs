using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Кубики")]
    public List<Dice> allDice = new List<Dice>();
    
    [Header("UI")]
    public Text scoreText;

    private DiceControls controls;
    private bool canThrow = true;

    void Awake()
    {
        controls = new DiceControls();
    }

    void Start()
    {        
        UpdateScoreText(0);
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Throw.performed += OnThrowPressed;
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
        controls.Gameplay.Throw.performed -= OnThrowPressed;
    }

    void OnThrowPressed(InputAction.CallbackContext context)
    {
        if (!canThrow) return;
        if (allDice.Count == 0) return;
        
        Debug.Log("Бросок!");
        StartCoroutine(ThrowAllDice());
    }

    public IEnumerator ThrowAllDice()
    {
        canThrow = false;
        UpdateScoreText(0);
        
        foreach (Dice dice in allDice)
        {
            if (dice != null)
                StartCoroutine(dice.RollDice());
        }

        bool allStopped = false;
        while (!allStopped)
        {
            allStopped = true;
            
            foreach (Dice dice in allDice)
            {
                if (dice != null && dice.IsRolling())
                {
                    allStopped = false;
                    break;
                }
            }
            
            yield return new WaitForSeconds(0.2f);
        }

        CalculateScore();
        canThrow = true;
    }

    void CalculateScore()
    {
        int totalScore = 0;
        
        foreach (Dice dice in allDice)
        {
            if (dice == null) continue;
            
            totalScore += dice.diceResult;
            
        }
        
        Debug.Log($"Общий счет: {totalScore}");
        UpdateScoreText(totalScore);
    }
    
    void UpdateScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Очки: {score}";
    }
}