using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class CurrencyManager : MonoBehaviour
{
    public const string CURRENCY_COINS = "coins";
    public const string CURRENCY_LIVES = "lives";
    
    public static CurrencyManager Instance;

    [Serializable]
    public class Currency
    {
        public string name;
        public int amount;
        public int maxAmount = int.MaxValue;
        public bool regenerates;
        public int regenerationTimeMinutes;
        public bool isUnlimited; // Whether the currency is in unlimited mode
        public DateTime unlimitedModeEndTime; // Time when unlimited mode ends

        public string lastUpdateKey;

        public Currency(string name, int amount, int maxAmount, bool regenerates = false, int regenerationTimeMinutes = 0)
        {
            this.name = name;
            this.amount = amount;
            this.maxAmount = maxAmount;
            this.regenerates = regenerates;
            this.regenerationTimeMinutes = regenerationTimeMinutes;
            this.isUnlimited = false;
            this.unlimitedModeEndTime = DateTime.MinValue;
            this.lastUpdateKey = $"{name}_LastUpdate";
        }
    }

    // Event to notify currency changes
    public static event Action<string, int, int> OnCurrencyChanged; // CurrencyName, NewValue, Delta

    private Dictionary<string, Currency> currencies = new Dictionary<string, Currency>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!currencies.ContainsKey(CURRENCY_COINS))
            {
                AddCurrency(CURRENCY_COINS, 1000, int.MaxValue);
            }

            if (!currencies.ContainsKey(CURRENCY_LIVES))
            {
                AddCurrency(CURRENCY_LIVES, 5, 5, true, 5);
            }

            StartCoroutine(WaitThenRefresh());
            RefreshAllCurrencies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator WaitThenRefresh()
    {
        yield return null;
        yield return null;
        RefreshAllCurrencies();

        // im lazy as fuck sorrynotsorry
    }

    /// <summary>   
    /// Refreshes all currencies.
    /// </summary>
    public void RefreshAllCurrencies()
    {
        foreach (var name in currencies)
        {
            if (currencies.TryGetValue(name.Key, out var currency))
            {
                // Trigger regeneration if applicable
                if (currency.regenerates)
                {
                    RegenerateCurrency(currency);
                }

                // Notify listeners of the latest value
                OnCurrencyChanged?.Invoke(name.Key, currency.amount, 0);
            }
        }

        Debug.Log("All currencies refreshed.");
    }


    private void Update()
    {
        foreach (var currency in currencies.Values)
        {
            if (currency.isUnlimited)
            {
                CheckUnlimitedModeEnd(currency);
            }
            else if (currency.regenerates)
            {
                RegenerateCurrency(currency);
            }
        }
    }

    public void AddCurrency(string name, int initialAmount, int maxAmount, bool regenerates = false, int regenerationTimeMinutes = 0)
    {
        if (!currencies.ContainsKey(name))
        {
            var currency = new Currency(name, initialAmount, maxAmount, regenerates, regenerationTimeMinutes);
            currencies[name] = currency;

            if (PlayerPrefs.HasKey(name))
            {
                currency.amount = PlayerPrefs.GetInt(name);
            }

            if (currency.regenerates && PlayerPrefs.HasKey(currency.lastUpdateKey))
            {
                RegenerateCurrency(currency);
            }
        }
    }

    public void AddToCurrency(string name, int amount)
    {
        if (currencies.TryGetValue(name, out var currency))
        {
            int oldAmount = currency.amount;
            currency.amount = Mathf.Clamp(currency.amount + amount, 0, currency.maxAmount);
            PlayerPrefs.SetInt(name, currency.amount);
            OnCurrencyChanged?.Invoke(name, currency.amount, currency.amount - oldAmount);
        }
    }

    public bool SpendCurrency(string name, int amount)
    {
        if (currencies.TryGetValue(name, out var currency))
        {
            if (currency.amount >= amount)
            {
                int oldAmount = currency.amount;
                currency.amount -= amount;
                PlayerPrefs.SetInt(name, currency.amount);

                if (currency.regenerates && currency.amount < currency.maxAmount)
                {
                    PlayerPrefs.SetString(currency.lastUpdateKey, DateTime.UtcNow.ToString());
                }

                OnCurrencyChanged?.Invoke(name, currency.amount, currency.amount - oldAmount);
                return true;
            }
        }
        return false;
    }

    public Currency GetCurrency(string id)
    {
        currencies.TryGetValue(id, out var currency);
        return currency;
    }


    public int GetCurrencyAmount(string name)
    {
        if (currencies.TryGetValue(name, out var currency))
        {
            return currency.amount;
        }
        return 0;
    }

    private void RegenerateCurrency(Currency currency)
    {
        if (currency.amount >= currency.maxAmount) return;

        if (PlayerPrefs.HasKey(currency.lastUpdateKey))
        {
            DateTime lastUpdate = DateTime.Parse(PlayerPrefs.GetString(currency.lastUpdateKey));
            TimeSpan timeElapsed = DateTime.UtcNow - lastUpdate;

            int unitsToRegenerate = (int)(timeElapsed.TotalMinutes / currency.regenerationTimeMinutes);

            if (unitsToRegenerate > 0)
            {
                int oldAmount = currency.amount;
                currency.amount = Mathf.Clamp(currency.amount + unitsToRegenerate, 0, currency.maxAmount);
                PlayerPrefs.SetInt(currency.name, currency.amount);

                TimeSpan leftoverTime = timeElapsed - TimeSpan.FromMinutes(unitsToRegenerate * currency.regenerationTimeMinutes);
                PlayerPrefs.SetString(currency.lastUpdateKey, DateTime.UtcNow.Subtract(leftoverTime).ToString());

                OnCurrencyChanged?.Invoke(currency.name, currency.amount, currency.amount - oldAmount);
            }
        }
    }

    public TimeSpan GetTimeUntilNextRegeneration(string name)
    {
        if (currencies.TryGetValue(name, out var currency) && currency.regenerates && currency.amount < currency.maxAmount)
        {
            DateTime lastUpdate = DateTime.Parse(PlayerPrefs.GetString(currency.lastUpdateKey));
            TimeSpan timeElapsed = DateTime.UtcNow - lastUpdate;
            TimeSpan timeUntilNextUnit = TimeSpan.FromMinutes(currency.regenerationTimeMinutes) - timeElapsed;

            return timeUntilNextUnit > TimeSpan.Zero ? timeUntilNextUnit : TimeSpan.Zero;
        }
        return TimeSpan.Zero;
    }

    public void ActivateUnlimitedMode(string name, int durationInSeconds)
    {
        if (currencies.TryGetValue(name, out var currency))
        {
            currency.isUnlimited = true;
            currency.unlimitedModeEndTime = DateTime.UtcNow.AddSeconds(durationInSeconds);
            Debug.Log($"Unlimited mode activated for '{name}' for {durationInSeconds} seconds.");
        }
        else
        {
            Debug.LogError($"Currency '{name}' not found.");
        }
    }

    public TimeSpan GetTimeUntilUnlimitedModeEnds(string name)
    {
        if (currencies.TryGetValue(name, out var currency) && currency.isUnlimited)
        {
            TimeSpan timeUntilNextUnit = currency.unlimitedModeEndTime - DateTime.UtcNow;

            return timeUntilNextUnit > TimeSpan.Zero ? timeUntilNextUnit : TimeSpan.Zero;
        }
        return TimeSpan.Zero;
    }

    private void CheckUnlimitedModeEnd(Currency currency)
    {
        if (DateTime.UtcNow >= currency.unlimitedModeEndTime)
        {
            currency.isUnlimited = false;
            currency.amount = currency.maxAmount; // Reset to max value
            PlayerPrefs.SetInt(currency.name, currency.amount);
            OnCurrencyChanged?.Invoke(currency.name, currency.amount, 0);
            Debug.Log($"Unlimited mode ended for '{currency.name}'. Currency reset to max value: {currency.maxAmount}.");
        }
    }
}
