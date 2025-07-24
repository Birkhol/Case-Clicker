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

    [Header("Animation")]
    public float animationDuration = 3f;

    private bool isSpinning = false;

    /// <summary>
    /// Call this from GameManager when opening a case.
    /// </summary>
    public void StartCaseOpening(Case selectedCase)
    {
        if (isSpinning) return;

        caseOpeningPanel.SetActive(true);
        StartCoroutine(SpinCase(selectedCase));

    }

    private IEnumerator SpinCase(Case selectedCase)
    {
        isSpinning = true;

        // Clean up old images
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // Random prize to win
        int actualPrizeIndex = Random.Range(0, selectedCase.possiblePrizes.Length);
        string prizeName = selectedCase.possiblePrizes[actualPrizeIndex];
        Sprite prizeSprite = selectedCase.prizeImages[actualPrizeIndex];

        // Fill list with random images (and make sure winning one ends centered)
        int totalItems = 30;
        int winningIndex = Random.Range(10, 20);

        for (int i = 0; i < totalItems; i++)
        {
            GameObject img = Instantiate(prizeImagePrefab, content);
            int index = (i == winningIndex) ? actualPrizeIndex : Random.Range(0, selectedCase.possiblePrizes.Length);
            img.GetComponent<Image>().sprite = selectedCase.prizeImages[index];
            img.name = selectedCase.possiblePrizes[index];
        }

        yield return null; // wait for layout

        // Scroll to winning item
        float itemWidth = prizeImagePrefab.GetComponent<RectTransform>().rect.width;
        float spacing = content.GetComponent<HorizontalLayoutGroup>().spacing;
        float targetPosition = (itemWidth + spacing) * winningIndex;

        float startX = content.anchoredPosition.x;
        float endX = -targetPosition + scrollRect.viewport.rect.width / 2 - itemWidth / 2;

        float time = 0f;
        while (time < animationDuration)
        {
            float t = time / animationDuration;
            t = 1f - Mathf.Pow(1f - t, 3); // ease-out
            content.anchoredPosition = new Vector2(Mathf.Lerp(startX, endX, t), content.anchoredPosition.y);
            time += Time.deltaTime;
            yield return null;
        }

        content.anchoredPosition = new Vector2(endX, content.anchoredPosition.y);
        resultText.text = $"You won: {prizeName}";
        Debug.Log($"You won: {prizeName}");

        isSpinning = false;

    }
}
