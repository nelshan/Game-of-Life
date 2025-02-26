using UnityEngine;

public class Cell : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetState(bool state)
    {
        spriteRenderer.color = state ? Color.white : Color.black;
    }
}