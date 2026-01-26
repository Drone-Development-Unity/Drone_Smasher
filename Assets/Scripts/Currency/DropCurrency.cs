using System.Collections.Generic;
using Game;
using UnityEngine;

public class DropCurrency : MonoBehaviour
{
    public List<CurrencyDropData> dropCurrencies;
    private List<CurrencyData> currencyData;
    private CurrencyManager  currencyManager;
    private System.Random rnd = new System.Random();
    [SerializeField] private ParticleSystem dropParticles;
    void Start()
    {
        currencyManager = CurrencyManager.Instance;
    }

    public void TryGetCurrency()
    {
        foreach (CurrencyDropData currency in dropCurrencies)
        {
            double value = rnd.NextDouble(); // range 0.0–1.0
            if (value < currency.dropRate)
            {
                currencyManager.AddCurrency(currency.currencyId, GetDropAmount(currency.currencyId));
            }
        }
        PlayAnimation();
    }
    public double GetDropAmount(int currId)
    {
        CurrencyDropData currency = dropCurrencies.Find(c => c.currencyId == currId);
        return Random.Range((float)currency.minAmount, (float)currency.maxAmount);
    }
    public void PlayAnimation()
    {
        var ps = Instantiate(dropParticles, transform.position, dropParticles.transform.rotation);
        ps.transform.localScale = Vector3.one;
        ps.Play();
    }
}
