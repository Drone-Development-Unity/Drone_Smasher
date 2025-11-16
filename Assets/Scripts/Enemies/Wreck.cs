using UnityEngine;
using Game;

public class Wreck : MonoBehaviour, IInteractable
{
    private bool _collected = false;
    private SpriteRenderer _renderer;
    private WreckManager _manager;
    private float boundary = -10f;

    [SerializeField] private float fallSpeed = 2f;

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

    // --- IInteractable ---
    public void OnHoverEnter()
    {
        if (!_collected)
        {
            Collect();
        }
    }

    public void OnHoverExit() { }

    public void OnClick() { }

    public void OnClickExit() { }

    private void Collect()
    {
        _collected = true;
        Debug.Log("Wrak zebrany!");
        _renderer.color = Color.black;
        // tutaj możesz dodać logikę: punkty, zasoby itd.
    }
}