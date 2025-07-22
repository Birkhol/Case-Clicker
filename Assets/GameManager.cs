using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text caseText;
    public Button clickButton;
    public Button autoClicker;
    public Case[] cases;
    public Transform caseListParent;  // UI
    public GameObject caseButtonPrefab;  // Prefab button for buying/opening cases
    public GameObject caseListPanel;
    public Button toggleCasePanelButton;
    public Text prizeText;

    private int caseCount = 0;
    private int autoClickCount = 0;
    private float autoClickTimer = 0.0f;
    private int autoClickerCost = 50;

    void Start()
    {
        clickButton.onClick.AddListener(OnClickCase);
        autoClicker.onClick.AddListener(onBuyAutoClicker);
        toggleCasePanelButton.onClick.AddListener(ToggleCasePanel);

        UpdateCaseText();
        UpdateAutoClickerButtonText();
        GenerateCaseButtons();
        caseListPanel.SetActive(false);
    }

    void Update()
    {
        if(autoClickCount > 0)
        {
            autoClickTimer += Time.deltaTime;

            float interval = 1.0f / autoClickCount;

            while (autoClickTimer >= interval)
            {
                caseCount += 1;
                autoClickTimer -= interval;
                UpdateCaseText();  
            }
        }
        
    }

    void OnClickCase()
    {
        caseCount++;
        UpdateCaseText();
    }

    void UpdateCaseText()
    {
        caseText.text = "Case: " + caseCount.ToString();
    }

    void onBuyAutoClicker()
    {
        if(caseCount >= autoClickerCost)
        {            
            caseCount -= autoClickerCost;
            autoClickerCost *= 2;
            autoClickCount++;
            UpdateCaseText();
            UpdateAutoClickerButtonText();
        }
        
    }

    void UpdateAutoClickerButtonText()
    {
        autoClicker.GetComponentInChildren<Text>().text = "AutoClicker (" + autoClickerCost + ")";
    }

    void GenerateCaseButtons()
    {
        foreach (Case c in cases)
        {
            Case localCase = c;

            GameObject newButton = Instantiate(caseButtonPrefab, caseListParent);
            newButton.GetComponentInChildren<Text>().text = $"Open {localCase.name} ({localCase.cost})";

            newButton.GetComponent<Button>().onClick.AddListener(() => TryOpenCase(localCase));
        }
    }


    public void TryOpenCase(Case selectedCase)
    {
        if (caseCount >= selectedCase.cost)
        {
            caseCount -= selectedCase.cost;
            UpdateCaseText();

            // Get random prize
            string prize = selectedCase.possiblePrizes[Random.Range(0, selectedCase.possiblePrizes.Length)];

            Debug.Log($"You opened a {selectedCase.name} and won: {prize}");

            ShowPrize(prize); // optional
        }
        else
        {
            Debug.Log("Not enough cases to open " + selectedCase.name);
        }
    }


    void ToggleCasePanel()
    {
        caseListPanel.SetActive(!caseListPanel.activeSelf);
    }

    public void BuyCase(int caseIndex)
    {
        if (caseIndex < 0 || caseIndex >= cases.Length) return;

        Case selectedCase = cases[caseIndex];

        if (caseCount >= selectedCase.cost)
        {
            caseCount -= selectedCase.cost;
            UpdateCaseText();

            // Pick a random prize
            string prize = selectedCase.possiblePrizes[Random.Range(0, selectedCase.possiblePrizes.Length)];

            // Show the prize
            Debug.Log("You opened a " + selectedCase.name + " and got: " + prize);
            ShowPrize(prize); // Optional display method

            // Optionally: update player inventory here
        }
        else
        {
            Debug.Log("Not enough cases to buy: " + selectedCase.name);
        }
    }

    void ShowPrize(string prize)
    {
        prizeText.text = "You won: " + prize;
    }


}
