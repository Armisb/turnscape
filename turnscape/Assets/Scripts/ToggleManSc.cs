using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class ToggleManager : LoaderBehaviour<ToggleManager>
{
    [System.Serializable]
    public class ToggleItem
    {
        public string id;
        public Toggle toggle;
        public GameObject targetObject;
    }

    public List<ToggleItem> items = new();

    private List<ToggleItem> matchedItems = new();

    private List<MenuToggle> allToggles = new();
    private List<MenuPanel> allPanels = new();

    private static Stack<string> menuStack = new();

    public Toggle activeToggle;
    public GameObject activeObject;

    private bool updating;

    protected override void Load()
    {
        menuStack.Clear();

        BuildLists();
        SetupListeners();
        SetupBackExitButtons();
        ApplyState();
        RefreshVisualState();
    }

    private void BuildLists()
    {
        matchedItems.Clear();
        allToggles.Clear();
        allPanels.Clear();

        matchedItems.AddRange(items);

        FindSceneObjects();
        FindScenePairs();
    }

    private void FindSceneObjects()
    {
        allToggles = FindObjectsByType<MenuToggle>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None).ToList();

        allPanels = FindObjectsByType<MenuPanel>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None).ToList();
    }

    private void FindScenePairs()
    {
        foreach (var menuToggle in allToggles)
        {
            if (menuToggle == null)
                continue;

            if (string.IsNullOrWhiteSpace(menuToggle.uniqueName))
                continue;

            MenuPanel matchingPanel =
                allPanels.FirstOrDefault(x =>
                    x != null &&
                    x.uniqueName == menuToggle.uniqueName);

            if (matchingPanel == null)
                continue;

            Toggle toggle = menuToggle.GetComponent<Toggle>();
            if (toggle == null)
                continue;

            bool alreadyExists =
                matchedItems.Any(x =>
                    x.toggle == toggle ||
                    x.targetObject == matchingPanel.gameObject);

            if (alreadyExists)
                continue;

            matchedItems.Add(new ToggleItem
            {
                id = menuToggle.uniqueName,
                toggle = toggle,
                targetObject = matchingPanel.gameObject
            });
        }
    }

    private void SetupBackExitButtons()
    {
        foreach (var menuToggle in allToggles)
        {
            if (menuToggle == null)
                continue;

            if (string.IsNullOrWhiteSpace(menuToggle.uniqueName))
                continue;

            string name = menuToggle.uniqueName.ToLower();

            if (name != "back" && name != "exit")
                continue;

            Button btn = menuToggle.GetComponent<Button>();
            if (btn == null)
                continue;

            btn.onClick.RemoveAllListeners();

            if (name == "back")
            {
                btn.onClick.AddListener(() =>
                {
                    Instance.CloseLastMenu();
                });
            }
            else if (name == "exit")
            {
                btn.onClick.AddListener(() =>
                {
                    Instance.DeactivateAll();
                });
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsClickOnManagedUI())
                DeactivateAll();
        }
    }

    private bool IsClickOnManagedUI()
    {
        if (EventSystem.current == null)
            return false;

        if (!EventSystem.current.IsPointerOverGameObject())
            return false;

        PointerEventData data = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        foreach (var result in results)
        {
            foreach (var item in matchedItems)
            {
                if (item?.toggle != null &&
                    result.gameObject.transform.IsChildOf(item.toggle.transform))
                    return true;

                if (item?.targetObject != null &&
                    result.gameObject.transform.IsChildOf(item.targetObject.transform))
                    return true;
            }
        }

        return false;
    }

    private void SetupListeners()
    {
        foreach (var item in matchedItems)
        {
            if (item.toggle == null || string.IsNullOrEmpty(item.id))
                continue;

            var current = item;

            item.toggle.onValueChanged.AddListener(isOn =>
            {
                if (updating)
                    return;

                if (isOn)
                {
                    OpenMenu(current.id);
                }
                else
                {
                    if (menuStack.Count > 0 &&
                        menuStack.Peek() == current.id)
                    {
                        CloseLastMenu();
                    }
                    else
                    {
                        RefreshVisualState();
                    }
                }
            });
        }
    }

    public void OpenMenu(string id)
    {
        if (string.IsNullOrEmpty(id))
            return;

        updating = true;

        if (menuStack.Contains(id))
        {
            var reordered = menuStack.Reverse()
                .Where(x => x != id)
                .ToList();

            menuStack.Clear();

            foreach (var s in reordered)
                menuStack.Push(s);
        }

        menuStack.Push(id);

        updating = false;

        RefreshVisualState();
    }

    public void CloseLastMenu()
    {
        if (menuStack.Count == 0)
            return;

        updating = true;

        menuStack.Pop();

        updating = false;

        RefreshVisualState();
    }

    public void DeactivateAll()
    {
        Debug.Log("Closing menus");

        updating = true;

        menuStack.Clear();

        activeToggle = null;
        activeObject = null;

        updating = false;

        RefreshVisualState();
    }

    private void ApplyState()
    {
        foreach (var item in matchedItems)
        {
            if (item.toggle != null && item.toggle.isOn && !string.IsNullOrEmpty(item.id))
            {
                OpenMenu(item.id);
            }
        }
    }

    private void RefreshVisualState()
    {
        updating = true;

        string topId = menuStack.Count > 0 ? menuStack.Peek() : null;

        ToggleItem topItem =
            matchedItems.FirstOrDefault(x => x.id == topId);

        activeToggle = topItem?.toggle;
        activeObject = topItem?.targetObject;

        foreach (var item in matchedItems)
        {
            bool inStack = !string.IsNullOrEmpty(item.id) && menuStack.Contains(item.id);
            bool isTop = item.id == topId;

            if (item.toggle != null)
                item.toggle.isOn = inStack;

            if (item.targetObject != null)
                item.targetObject.SetActive(isTop);

            if (item.toggle != null)
            {
                var images = item.toggle.GetComponentsInChildren<Image>(true);

                foreach (var img in images)
                {
                    img.color =
                        inStack
                        ? new Color(0.8f, 0.8f, 0.8f)
                        : Color.white;
                }
            }
        }

        updating = false;
    }
}