using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isAlive;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetState(bool state)
    {
        isAlive = state;
        spriteRenderer.color = isAlive ? Color.white : Color.black;
    }
}