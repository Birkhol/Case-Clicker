using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text caseText;
    public Button clickButton;
    public Button autoClicker;

    private int caseCount = 0;
    private int autoClickCount = 0;
    private float autoClickTimer = 0.0f;
    private int autoClickerCost = 50;

    void Start()
    {
        clickButton.onClick.AddListener(OnClickCase);
        autoClicker.onClick.AddListener(onBuyAutoClicker);
        UpdateCaseText();
        UpdateAutoClickerButtonText();
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
    
}
