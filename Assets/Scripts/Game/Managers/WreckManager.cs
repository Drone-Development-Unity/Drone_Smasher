using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WreckManager : MonoBehaviour
{
    public static WreckManager Instance { get; private set; }

    public GameObject wrecksContainer;
    
    private List<Wreck> wrecks = new List<Wreck>();

    // gatherer activation threshold
    [SerializeField] private Transform GatherersActivationThresholdPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Wreck ReserveClosest(Vector3 dronePos)
    {
        var freeWrecks = wrecks
            .Where(w => 
                !w.IsTracked &&
                w.transform.position.y < GatherersActivationThresholdPosition.position.y)
            .ToList();
        if (!freeWrecks.Any()) return null;

        var closestWreck = freeWrecks.OrderBy(w =>
            Vector3.Distance(dronePos, w.transform.position)).First();

        closestWreck.IsTracked = true;

        return closestWreck;
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
