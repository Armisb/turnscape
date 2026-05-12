using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class HoverInfoManSc : MonoBehaviour
{
    [Header("UI")]
    public RectTransform panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public Image background;

    [Header("Size Limits")]
    public Vector2 minSize = new Vector2(250, 120);
    public Vector2 maxSize = new Vector2(600, 350);

    [Header("Text Settings")]
    public float titleFontSize = 24f;
    public float descFontSize = 18f;

    [Header("Layout")]
    public float padding = 10f;
    public float spacing = 6f;
    public float maxTextWidth = 500f;

    [Header("Settings")]
    public float scanInterval = 0.1f;
    public int refreshInterval = 10;

    [SerializeField] private char titleSeparator = '@';

    int refreshCounter = 0;

    public Camera cam;

    public InfoDropSc current;

    InfoDropSc candidate;
    float delayTimer;
    float scanTimer;

    EventSystem eventSystem;

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
            }
            else
            {
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

                delayTimer =
                    resolved != null
                    ? resolved.delay
                    : 0;
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

        if (info == null) return;

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

        float width;
        float height;

        Vector2 titleSize =
            titleText.GetPreferredValues(
                titleText.text,
                maxTextWidth,
                0
            );

        Vector2 descSize =
            descText.GetPreferredValues(
                descText.text,
                maxTextWidth,
                0
            );

        if (info != null && info.overrideSize)
        {
            width = Mathf.Clamp(
                info.size.x,
                minSize.x,
                maxSize.x
            );

            height = Mathf.Clamp(
                info.size.y,
                minSize.y,
                maxSize.y
            );
        }
        else
        {
            width = Mathf.Clamp(
                Mathf.Max(titleSize.x, descSize.x) + padding * 2,
                minSize.x,
                maxSize.x
            );

            float preferredHeight =
                padding
                + titleSize.y
                + spacing
                + descSize.y
                + padding;

            height = Mathf.Clamp(
                preferredHeight,
                minSize.y,
                maxSize.y
            );
        }

        panel.sizeDelta = new Vector2(width, height);

        RectTransform tr = titleText.rectTransform;

        tr.anchorMin = new Vector2(0, 1);
        tr.anchorMax = new Vector2(0, 1);

        tr.pivot = new Vector2(0, 1);

        tr.anchoredPosition =
            new Vector2(
                padding,
                -padding
            );

        tr.sizeDelta =
            new Vector2(
                width - padding * 2,
                titleSize.y
            );

        RectTransform dr = descText.rectTransform;

        dr.anchorMin = new Vector2(0, 1);
        dr.anchorMax = new Vector2(0, 1);

        dr.pivot = new Vector2(0, 1);

        dr.anchoredPosition =
            new Vector2(
                padding,
                -padding - titleSize.y - spacing
            );

        float allowedHeight =
            height
            - padding
            - titleSize.y
            - spacing
            - padding;

        dr.sizeDelta =
            new Vector2(
                width - padding * 2,
                allowedHeight
            );

        string raw = titleText.text ?? "";

        string left = raw;
        string right = "";

        int sep = raw.IndexOf(titleSeparator);

        if (sep >= 0)
        {
            left = raw.Substring(0, sep);
            right = raw.Substring(sep + 1);
        }

        float availableWidth = width - (padding * 2);

        Vector2 leftSize = titleText.GetPreferredValues(left);
        Vector2 rightSize = titleText.GetPreferredValues(right);

        int spaces = 1;
        int lastValidSpaces = 1;

        int breakTimer = 0;
        while (breakTimer < 100)
        {
            breakTimer++;
            string test = left + new string(' ', spaces) + right;

            float testWidth = titleText.GetPreferredValues(test).x;

            if (testWidth <= availableWidth)
            {
                lastValidSpaces = spaces;
                spaces++;
            }
            else
            {
                break;
            }
        }

        spaces = Mathf.Max(1, lastValidSpaces);

        string finalTitle = left + new string(' ', spaces) + right;

        float finalWidth = titleText.GetPreferredValues(finalTitle).x;

        if (finalWidth > availableWidth)
        {
            string cutRight = right;

            while (cutRight.Length > 0)
            {
                cutRight = cutRight.Substring(0, cutRight.Length - 1);

                string test = left + new string(' ', spaces) + cutRight + "...";

                float w = titleText.GetPreferredValues(test).x;

                if (w <= availableWidth)
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

        RectTransform parent =
            panel.parent as RectTransform;

        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(
                parent,
                mouse,
                cam,
                out Vector2 localMouse
            );

        Vector2 size = panel.rect.size;

        Vector2 pivotOffset =
            new Vector2(
                size.x * panel.pivot.x,
                size.y * panel.pivot.y
            );

        panel.anchoredPosition =
            localMouse - pivotOffset;
    }
}