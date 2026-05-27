using UnityEngine;

public class InfoDropSc : MonoBehaviour
{
    public int priority { get; private set; }

    public bool overrideParameters = false;

    public string title;
    [TextArea] public string description;

    public Color backgroundColor = Color.black;

    public float delay = 0;

    public Vector2 minSize = new Vector2(250, 120);
    public Vector2 maxSize = new Vector2(520, 240);
    public Vector2 offset = new Vector2(-1, -1);
    public bool clampToScreen = true;
}