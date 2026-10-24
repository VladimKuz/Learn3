using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Кубики")]
    [SerializeField] private List<Dice> allDice = new List<Dice>();
    
    [Header("Ссылки")]
    [SerializeField] private DiceUI ui;
    [SerializeField] private GameObject dicePrefab; 
    [SerializeField] private Transform diceSpawnPoint;

    private int currentTieScore = 0;
    private int currentWinScore = 0;
    private int currentLoseScore = 0;

    private DiceControls controls;
    private bool canThrow = true;

    void Awake()
{
    controls = new DiceControls();
    
    if (ui != null)
    {
        ui.OnRerollClicked += OnRerollRequested;
        ui.TriggerScoreUpdate(0);
        ui.TriggerDiceCountUpdate(allDice.Count > 0 ? allDice.Count : 2);
    }
}

    void Start()
    {        
        GenerateRandomTargets();
        
        if (allDice.Count == 0 && dicePrefab != null)
        {
            UpdateDiceCount(2);
        }
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

    void OnDestroy()
    {
        if (ui != null)
        {
            ui.OnRerollClicked -= OnRerollRequested;
        }
    }

    void OnThrowPressed(InputAction.CallbackContext context)
    {
        if (!canThrow) return;
        if (allDice.Count == 0) return;
        
        StartCoroutine(ThrowAllDice());
    }

    private void OnRerollRequested(int diceCount, int tie, int win, int lose)
    {
        if (!canThrow) return;

        currentTieScore = tie;
        currentWinScore = win;
        currentLoseScore = lose;
        
        ui.TriggerTargetsUpdate(currentTieScore, currentWinScore, currentLoseScore);
        UpdateDiceCount(diceCount);
        StartCoroutine(ThrowAllDice());
    }

    public IEnumerator ThrowAllDice()
    {
        canThrow = false;
        ui.TriggerScoreUpdate(0);
        
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
        
        ui.TriggerScoreUpdate(totalScore);
        ui.ShowRoundResult(totalScore, currentWinScore, currentLoseScore);
    }
    
    private void GenerateRandomTargets()
    {
        currentTieScore = Random.Range(5, 16);
        currentWinScore = currentTieScore + Random.Range(1, 6);
        currentLoseScore = currentTieScore - Random.Range(1, 6);
        
        if (ui != null)
            ui.TriggerTargetsUpdate(currentTieScore, currentWinScore, currentLoseScore);
            ui.SetTargetInputs(currentTieScore, currentWinScore, currentLoseScore);
    }

    private void UpdateDiceCount(int targetCount)
    {
        targetCount = Mathf.Clamp(targetCount, 1, 6);

        while (allDice.Count > targetCount)
        {
            Dice lastDice = allDice[allDice.Count - 1];
            allDice.RemoveAt(allDice.Count - 1);
            Destroy(lastDice.gameObject);
        }
        
        while (allDice.Count < targetCount && dicePrefab != null && diceSpawnPoint != null)
        {
            Vector3 spawnPos = diceSpawnPoint.position + new Vector3(allDice.Count * 1.5f, 0, 0);
            GameObject newDiceObj = Instantiate(dicePrefab, spawnPos, Quaternion.identity);
            allDice.Add(newDiceObj.GetComponent<Dice>());
        }
        
        if (ui != null)
            ui.TriggerDiceCountUpdate(allDice.Count);
    }
}