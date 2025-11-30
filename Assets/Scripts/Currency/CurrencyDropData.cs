namespace Game
{
    [System.Serializable]
    public class CurrencyDropData
    {
        public int currencyId;  //equivalent id from CurrencyData
        public double dropRate; //1 = 100% drop rate
        public double minAmount;
        public double maxAmount;
    }
}