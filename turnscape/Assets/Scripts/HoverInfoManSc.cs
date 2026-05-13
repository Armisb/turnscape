using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class HoverInfoManSc : MonoBehaviour
{
    public RectTransform panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public Image background;

    public Vector2 minSize = new Vector2(250, 120);
    public Vector2 maxSize = new Vector2(600, 350);
    public Vector2 offset = new Vector2(1, 1);
    public bool clampToScreen = true;

    public float titleFontSize = 24f;
    public float descFontSize = 18f;

    public float padding = 10f;
    public float spacing = 6f;
    public float maxTextWidth = 500f;

    public float scanInterval = 0.1f;
    public int refreshInterval = 10;

    [SerializeField] private char titleSeparator = '@';

    public Camera cam;

    public InfoDropSc current;

    InfoDropSc candidate;

    float delayTimer;
    float scanTimer;
    int refreshCounter;

    EventSystem eventSystem;

    Vector2 MinSize => current != null && current.overrideParameters ? current.minSize : minSize;
    Vector2 MaxSize => current != null && current.overrideParameters ? current.maxSize : maxSize;
    Vector2 Offset => current != null && current.overrideParameters ? current.offset : offset;
    bool ClampToScreen => current != null && current.overrideParameters ? current.clampToScreen : clampToScreen;

    void Start()
    {
        eventSystem = EventSystem.current;

        panel.gameObject.SetActive(false);

        titleText.enableWordWrapping = true;
        descText.enableWordWrapping = true;

        titleText.overflowMode = TextOverflowModes.Ellipsis;
        descText.overflowMode = TextOverflowModes.Ellipsis;

        titleText.alignment = TextAlignmentOptions.TopLeft;
        descText.alignment = TextAlignmentOptions.TopLeft;
    }

    void Update()
    {
        eventSystem = EventSystem.current;

        scanTimer += Time.deltaTime;

        if (scanTimer < scanInterval)
            return;

        scanTimer = 0f;

        HandleHover();
        UpdatePanelPosition();
    }

    void HandleHover()
    {
        Transform hit =
            MouseRaycastUtils.GetObjectUnderMouse(eventSystem, cam);

        InfoDropSc resolved =
            HierarchySearchUtils.ResolveBest<InfoDropSc>(hit, out _, Input.mousePosition);

        if (current != null)
        {
            if (resolved != current)
            {
                SetCurrent(resolved);
                return;
            }

            if (refreshCounter >= refreshInterval)
            {
                ApplyText(current);
                LayoutManually(current);
                refreshCounter = 0;
            }
            else
            {
                refreshCounter++;
            }
        }
        else
        {
            if (resolved == candidate)
            {
                delayTimer -= scanInterval;

                if (delayTimer <= 0)
                {
                    SetCurrent(candidate);
                    candidate = null;
                }
            }
            else
            {
                candidate = resolved;
                delayTimer = resolved != null ? resolved.delay : 0;
            }
        }
    }

    void SetCurrent(InfoDropSc info)
    {
        if (current == info)
            return;

        current = info;

        if (info == null)
        {
            panel.gameObject.SetActive(false);
            return;
        }

        panel.gameObject.SetActive(true);

        ApplyText(info);
        LayoutManually(info);
    }

    void ApplyText(InfoDropSc info)
    {
        titleText.fontSize = titleFontSize;
        descText.fontSize = descFontSize;

        titleText.text = info.title ?? "";
        descText.text = info.description ?? "";

        background.color = info.backgroundColor;
    }

    void LayoutManually(InfoDropSc info)
    {
        titleText.fontSize = titleFontSize;
        descText.fontSize = descFontSize;

        titleText.enableWordWrapping = true;
        descText.enableWordWrapping = true;

        titleText.overflowMode = TextOverflowModes.Ellipsis;
        descText.overflowMode = TextOverflowModes.Ellipsis;

        titleText.ForceMeshUpdate();

        Vector2 titleSize =
            titleText.GetPreferredValues(titleText.text, maxTextWidth, 0);

        Vector2 descSize =
            descText.GetPreferredValues(descText.text, maxTextWidth, 0);

        float width = Mathf.Clamp(
            Mathf.Max(titleSize.x, descSize.x) + padding * 2,
            MinSize.x,
            MaxSize.x
        );

        float height = Mathf.Clamp(
            padding + titleSize.y + spacing + descSize.y + padding,
            MinSize.y,
            MaxSize.y
        );

        panel.sizeDelta = new Vector2(width, height);

        RectTransform tr = titleText.rectTransform;

        tr.anchorMin = new Vector2(0, 1);
        tr.anchorMax = new Vector2(0, 1);
        tr.pivot = new Vector2(0, 1);

        tr.anchoredPosition = new Vector2(padding, -padding);

        tr.sizeDelta = new Vector2(width - padding * 2, titleSize.y);

        RectTransform dr = descText.rectTransform;

        dr.anchorMin = new Vector2(0, 1);
        dr.anchorMax = new Vector2(0, 1);
        dr.pivot = new Vector2(0, 1);

        dr.anchoredPosition =
            new Vector2(padding, -padding - titleSize.y - spacing);

        dr.sizeDelta =
            new Vector2(width - padding * 2,
                height - padding - titleSize.y - spacing - padding);

        string raw = titleText.text ?? "";

        string left = raw;
        string right = "";

        int sep = raw.IndexOf(titleSeparator);

        if (sep >= 0)
        {
            left = raw.Substring(0, sep);
            right = raw.Substring(sep + 1);
        }

        float availableWidth = width - padding * 2;

        int spaces = 1;
        int lastValid = 1;

        for (int i = 0; i < 100; i++)
        {
            string test = left + new string(' ', spaces) + right;
            float w = titleText.GetPreferredValues(test).x;

            if (w <= availableWidth)
            {
                lastValid = spaces;
                spaces++;
            }
            else
            {
                break;
            }
        }

        spaces = Mathf.Max(1, lastValid);

        string finalTitle = left + new string(' ', spaces) + right;

        float finalWidth = titleText.GetPreferredValues(finalTitle).x;

        if (finalWidth > availableWidth)
        {
            string cutRight = right;

            while (cutRight.Length > 0)
            {
                cutRight = cutRight.Substring(0, cutRight.Length - 1);

                string test = left + new string(' ', spaces) + cutRight + "...";

                if (titleText.GetPreferredValues(test).x <= availableWidth)
                {
                    finalTitle = test;
                    break;
                }
            }
        }

        titleText.text = finalTitle;
    }

    void UpdatePanelPosition()
    {
        if (current == null)
            return;

        Vector2 mouse = Input.mousePosition;

        RectTransform parent = panel.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            mouse,
            cam,
            out Vector2 localMouse
        );

        Vector2 size = panel.rect.size;

        Vector2 correctedOffset = Offset;

        if (ClampToScreen)
        {
            correctedOffset = ClampOffsetToScreen(localMouse, size, Offset, parent, cam);
        }

        Vector2 pivotOffset = new Vector2(
            size.x * correctedOffset.x * -0.5f,
            size.y * correctedOffset.y * -0.5f
        );

        panel.anchoredPosition = localMouse - pivotOffset;
    }

    Vector2 ClampOffsetToScreen(
        Vector2 localMouse,
        Vector2 size,
        Vector2 offset,
        RectTransform parent,
        Camera cam)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            cam,
            parent.TransformPoint(localMouse)
        );

        Vector2 half = size * 0.5f;

        Vector2 desired = screenPos + new Vector2(
            offset.x * half.x,
            offset.y * half.y
        );

        float left = desired.x - half.x;
        float right = desired.x + half.x;
        float bottom = desired.y - half.y;
        float top = desired.y + half.y;

        float correctionX = 0f;
        float correctionY = 0f;

        if (left < 0)
            correctionX = -left;
        else if (right > Screen.width)
            correctionX = Screen.width - right;

        if (bottom < 0)
            correctionY = -bottom;
        else if (top > Screen.height)
            correctionY = Screen.height - top;

        Vector2 correctedOffset = offset;

        if (half.x > 0.0001f)
            correctedOffset.x += correctionX / half.x;

        if (half.y > 0.0001f)
            correctedOffset.y += correctionY / half.y;

        desired = screenPos + new Vector2(
            correctedOffset.x * half.x,
            correctedOffset.y * half.y
        );

        return correctedOffset;
    }
}