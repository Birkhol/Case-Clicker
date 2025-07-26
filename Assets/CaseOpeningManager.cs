using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CaseOpeningManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject caseOpeningPanel;
    public RectTransform content;
    public GameObject prizeImagePrefab;
    public ScrollRect scrollRect;
    public Text resultText;
    public Text caseCountText;
    public Button sellButton;
    public Button backButton;
    public Text caseListCountText;

    private string currentPrize;
    private GameManager gameManager;


    [Header("Animation")]
    public float animationDuration = 8f;

    private bool isSpinning = false;

    private System.Action onSpinFinished;

    public void StartCaseOpening(Case selectedCase, System.Action callback)
    {
        if (isSpinning) return;

        onSpinFinished = callback;
        StartCoroutine(SpinCase(selectedCase));
    }


    private IEnumerator SpinCase(Case selectedCase)
    {
        isSpinning = true;  
        caseOpeningPanel.SetActive(true);
        UpdateCaseCountText();
        sellButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);


        // Clear existing items
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        content.anchoredPosition = Vector2.zero;

        // Step 1: Create enough scroll items
        int totalItems = 120;
        int winningIndex = Random.Range(110, 115);
        int actualPrizeIndex = Random.Range(0, selectedCase.possiblePrizes.Length);

        for (int i = 0; i < totalItems; i++)
        {
            GameObject img = Instantiate(prizeImagePrefab, content);
            int prizeIndex = (i == winningIndex) ? actualPrizeIndex : Random.Range(0, selectedCase.possiblePrizes.Length);
            img.GetComponent<Image>().sprite = selectedCase.prizeImages[prizeIndex];
            img.name = selectedCase.possiblePrizes[prizeIndex];
        }

        yield return new WaitForEndOfFrame();

        // Step 2: Calculate scroll target
        float itemWidth = prizeImagePrefab.GetComponent<RectTransform>().rect.width;
        float spacing = content.GetComponent<HorizontalLayoutGroup>().spacing;
        float distancePerItem = itemWidth + spacing;

        float targetPosition = distancePerItem * winningIndex;
        float duration = animationDuration;

        float time = 0f;
        Vector2 startPos = content.anchoredPosition;
        float endX = -targetPosition + scrollRect.viewport.rect.width / 2 - itemWidth / 2;

        // Step 3: Animate the scroll over duration
        while (time < duration)
        {
            float t = time / duration;
            t = Mathf.Clamp01(t);
            // Optional: smooth ease-out
            float easedT = 1f - Mathf.Pow(1f - t, 3);
            float newX = Mathf.Lerp(startPos.x, endX, easedT);

            content.anchoredPosition = new Vector2(newX, startPos.y);
            time += Time.deltaTime;
            yield return null;
        }

        // Snap to final position
        content.anchoredPosition = new Vector2(endX, startPos.y);

        // after scroll animation
        string wonPrize = selectedCase.possiblePrizes[actualPrizeIndex];
        currentPrize = wonPrize;
        resultText.text = $"You won: {wonPrize}";

        sellButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);

        isSpinning = false;
        onSpinFinished?.Invoke();


    }

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        sellButton.onClick.AddListener(OnSellPrize);
        backButton.onClick.AddListener(ClosePanel);

        gameManager.OnCaseCountChanged += HandleCaseCountChanged;
    }

    void OnSellPrize()
    {
        gameManager.AddCase(1); // give 1 case back (or change value)
        UpdateCaseCountText();
        sellButton.gameObject.SetActive(false);
        resultText.text = "";
    }

    void ClosePanel()
    {
        caseOpeningPanel.SetActive(false);
        sellButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        resultText.text = "";
    }

    public void UpdateCaseCountText()
    {
        string countText = "Cases: " + gameManager.GetCaseCount();

        if (caseCountText != null)
            caseCountText.text = countText;

        if (caseListCountText != null)
            caseListCountText.text = countText;
    }


    public void ResetUI()
    {
        sellButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        resultText.text = "";
    }

    public void ShowPostSpinUI(string prizeName)
    {
        resultText.text = $"You won: {prizeName}";
        sellButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    void HandleCaseCountChanged(int newCount)
    {
        UpdateCaseCountText();
    }

}
