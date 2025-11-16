using System.Collections.Generic;
using UnityEngine;

public class WreckManager : MonoBehaviour
{
    public static WreckManager Instance { get; private set; }

    public GameObject wrecksContainer;
    
    private List<Wreck> wrecks = new List<Wreck>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterWreck(Wreck wreck)
    {
        wrecks.Add(wreck);
    }

    public void RemoveWreck(Wreck wreck)
    {
        if (wrecks.Contains(wreck))
        {
            wrecks.Remove(wreck);
            Destroy(wreck.gameObject);
        }
    }
}
