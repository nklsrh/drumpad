using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ProtoUICurrency : MonoBehaviour
{
    public string currencyName; // The currency to track (e.g., "Coins", "Lives")
    public TextMeshProUGUI currencyText; // The main text field to show the currency value
    public GameObject regenBG; //  bg for regen
    public TextMeshProUGUI regenerationText; // Optional text field for regeneration time

    private void OnEnable()
    {
        // Subscribe to the currency change event
        CurrencyManager.OnCurrencyChanged += OnCurrencyChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        CurrencyManager.OnCurrencyChanged -= OnCurrencyChanged;
    }

    void Start()
    {
        if (CurrencyManager.Instance == null) return;

        OnCurrencyChanged(currencyName, CurrencyManager.Instance.GetCurrencyAmount(currencyName), 0);
    }

    private void Update()
    {
        if (CurrencyManager.Instance == null) return;

        var currency = CurrencyManager.Instance.GetCurrency(currencyName);
        // Update regeneration text if applicable
        if (CurrencyManager.Instance.GetCurrencyAmount(currencyName) < currency.maxAmount)
        {
            if (currency.isUnlimited)
            {
                if (regenerationText != null)
                {
                    regenBG?.SetActive(true);
                    var timeLeft = CurrencyManager.Instance.GetTimeUntilUnlimitedModeEnds(currencyName);
                    regenerationText.text = $"Ends in: {timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";
                }
            }
            else if (regenerationText != null)
            {
                var timeLeft = CurrencyManager.Instance.GetTimeUntilNextRegeneration(currencyName);
                if (timeLeft.TotalSeconds > 0)
                {
                    regenBG?.SetActive(true);
                    regenerationText.text = $"Next in: {timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";
                }
                else
                {
                    regenBG?.SetActive(false);
                }
            }
        }
        else if (regenBG != null)
        {
            regenBG.SetActive(false);
        }
    }

    private void OnCurrencyChanged(string changedCurrency, int newValue, int delta)
    {
        if (changedCurrency == currencyName)
        {
            UpdateUI(newValue, delta);
        }
    }

    private void UpdateUI(int newValue, int delta)
    {
        if (currencyText != null)
        {
            var c = CurrencyManager.Instance.GetCurrency(currencyName);
            if (c.isUnlimited)
            {
                currencyText.text =  "∞";
            }
            else
            {
                currencyText.text = newValue + (c.maxAmount != int.MaxValue ? "/" + c.maxAmount : "");
            }
        }
    }
}
