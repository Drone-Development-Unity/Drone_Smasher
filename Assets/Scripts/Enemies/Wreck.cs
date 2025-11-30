using Assets.Scripts.Interfaces.Enemy;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;

public class Wreck : MonoBehaviour, IPointerEnterHandler, ITraceableWreck
{
    private bool _collected = false;
    private SpriteRenderer _renderer;
    private WreckManager _manager;
    private float boundary = -40f;

    public float FallSpeed => fallSpeed;

    [SerializeField] private float fallSpeed = 2f;

    private bool _isTracked = false;
    public bool IsTracked { get => _isTracked; set => _isTracked = value; }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _manager = WreckManager.Instance;
    }

    private void Update()
    {
        // ruch w dół
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // jeśli wrak spadnie za daleko – zgłoś do menedżera
        if (transform.position.y < boundary)
        {
            _manager.RemoveWreck(this);
        }
    }

    // --- HOVER ----
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_collected)
        {
            Collect();
        }
    }

    private void Collect()
    {
        _collected = true;
        //Debug.Log("Wrak zebrany!");
        _renderer.color = Color.black;
        // tutaj możesz dodać logikę: punkty, zasoby itd.
        DropCurrency dropCurrency = gameObject.GetComponent<DropCurrency>();
        if (dropCurrency != null)
        {
            dropCurrency.TryGetCurrency();
        }
    }
}