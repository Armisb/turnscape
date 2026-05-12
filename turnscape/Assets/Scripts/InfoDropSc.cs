using UnityEngine;

public class InfoDropSc : MonoBehaviour
{
    public string title;

    [TextArea]
    public string description;

    public Color backgroundColor = Color.black;

    public float delay = 0;

    public int priority { get; private set; }

    public bool overrideSize = false;
    public Vector2 size = new Vector2(250, 120);
}