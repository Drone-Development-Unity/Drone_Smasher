using UnityEngine;

public class Switcher : MonoBehaviour
{
	public GameObject StatsPanel;
	public GameObject ShopPanel;

	public void ShowA()
	{
        StatsPanel.SetActive(true);
		ShopPanel.SetActive(false);
	}

	public void ShowB()
	{
        StatsPanel.SetActive(false);
		ShopPanel.SetActive(true);
	}
}
