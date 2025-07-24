using System.Collections;
using System.Threading;
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
    public CaseOpeningManager caseOpeningManager;
    public GameObject caseOpeningPanel;
    public ScrollRect scrollRect;
    public GameObject prizeImagePrefab;
    public RectTransform content;


    private int caseCount = 0;
    private int autoClickCount = 0;
    private float autoClickTimer = 0.0f;
    private int autoClickerCost = 50;
    private bool isSpinning = false;

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
        if (isSpinning) return;

        if (caseCount >= selectedCase.cost)
        {
            isSpinning = true;
            caseCount -= selectedCase.cost;
            UpdateCaseText();

            caseOpeningPanel.SetActive(true); // Show panel
            StartCoroutine(SpinCase(selectedCase)); // Start animated spin

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

    private IEnumerator SpinCase(Case selectedCase)
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        int totalItems = 30;
        int winningIndex = Random.Range(10, 20);
        int actualPrizeIndex = Random.Range(0, selectedCase.possiblePrizes.Length);

        for (int i = 0; i < totalItems; i++)
        {
            GameObject img = Instantiate(prizeImagePrefab, content);
            int index = (i == winningIndex) ? actualPrizeIndex : Random.Range(0, selectedCase.possiblePrizes.Length);
            img.GetComponent<Image>().sprite = selectedCase.prizeImages[index];
            img.name = selectedCase.possiblePrizes[index];
        }

        yield return null; // wait for layout update

        float itemWidth = prizeImagePrefab.GetComponent<RectTransform>().rect.width;
        float spacing = content.GetComponent<HorizontalLayoutGroup>().spacing;
        float targetPosition = (itemWidth + spacing) * winningIndex;

        float duration = 3f;
        float time = 0f;
        float startX = content.anchoredPosition.x;
        float endX = -targetPosition + scrollRect.viewport.rect.width / 2 - itemWidth / 2;

        while (time < duration)
        {
            float t = time / duration;
            t = 1f - Mathf.Pow(1f - t, 3); // ease out
            content.anchoredPosition = new Vector2(Mathf.Lerp(startX, endX, t), content.anchoredPosition.y);
            time += Time.deltaTime;
            yield return null;
        }

        content.anchoredPosition = new Vector2(endX, content.anchoredPosition.y);

        string wonPrize = selectedCase.possiblePrizes[actualPrizeIndex];
        Debug.Log($"Player won: {wonPrize}");

        isSpinning = false;
 
    }


}
