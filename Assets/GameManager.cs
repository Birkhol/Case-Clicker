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
            GameObject newButton = Instantiate(caseButtonPrefab, caseListParent);
            newButton.GetComponentInChildren<Text>().text = $"Open {c.name} ({c.cost})";

            newButton.GetComponent<Button>().onClick.AddListener(() => TryOpenCase(c));
        }
    }

    void TryOpenCase(Case c)
    {
        if (caseCount >= c.cost)
        {
            caseCount -= c.cost;
            UpdateCaseText();

            int index = Random.Range(0, c.possiblePrizes.Length);
            string reward = c.possiblePrizes[index];

            Debug.Log($"You opened a {c.name} and got: {reward}");
            // Optionally: Display reward with UI popup, animation, sound, etc.
        }
        else
        {
            Debug.Log("Not enough cases to open this.");
        }
    }

    void ToggleCasePanel()
    {
        caseListPanel.SetActive(!caseListPanel.activeSelf);
    }

}
