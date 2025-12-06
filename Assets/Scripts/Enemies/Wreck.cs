using Assets.Scripts.Interfaces.Enemy;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;

public class Wreck : MonoBehaviour, IPointerEnterHandler, ITraceableWreck
{
    private bool _collected = false;
    private SpriteRenderer _renderer;
    private WreckManager _wreckManager;

    private float boundary = -13f;

    public float FallSpeed => fallSpeed;

    [SerializeField] private float fallSpeed = 2f;

    private bool _isTracked = false;
    [HideInInspector] public bool IsTracked { get => _isTracked; set => _isTracked = value; }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _wreckManager = WreckManager.Instance;
    }

    private void Update()
    {
        // ruch w dół
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // jeśli wrak spadnie za daleko – zgłoś do menedżera
        if (transform.position.y < boundary)
        {
            _wreckManager.RemoveWreck(this);
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