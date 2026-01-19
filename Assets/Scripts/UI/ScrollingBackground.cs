using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float speed = 2f;        // prêdkoœæ scrolla
    public float spriteHeight;      // wysokoœæ sprite'a w units
    public float resetOffset = 5f;

    private Transform[] backgrounds;

    void Start()
    {
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            backgrounds[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        foreach (Transform bg in backgrounds)
        {
            bg.Translate(Vector3.down * speed * Time.deltaTime);

            if (bg.position.y <= -spriteHeight - resetOffset)
            {
                bg.position += Vector3.up * spriteHeight * 2f;
            }
        }
    }
}