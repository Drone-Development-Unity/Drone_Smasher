using UnityEngine;

public class CannonPlaceIdentity : MonoBehaviour
{
    [SerializeField]
    private int placeId;

    public int GetPlaceId()
    {
        return placeId;
    }
}
