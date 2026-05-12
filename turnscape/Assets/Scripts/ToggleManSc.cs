using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleManager : MonoBehaviour
{
    [System.Serializable]
    public class ToggleItem
    {
        public Toggle toggle;
        public GameObject targetObject;
    }

    public List<ToggleItem> items = new List<ToggleItem>();

    public Toggle activeToggle;
    public GameObject activeObject;

    private bool updating;

    private void Start()
    {
        SetupListeners();
        ApplyState();
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

        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        foreach (var result in results)
        {
            foreach (var item in items)
            {
                if (item.toggle != null && result.gameObject.transform.IsChildOf(item.toggle.transform))
                    return true;

                if (item.targetObject != null && result.gameObject.transform.IsChildOf(item.targetObject.transform))
                    return true;
            }
        }

        return false;
    }

    private void SetupListeners()
    {
        foreach (var item in items)
        {
            if (item.toggle == null) continue;

            var current = item;

            item.toggle.onValueChanged.AddListener(isOn =>
            {
                if (updating) return;

                if (isOn)
                    Activate(current);
                else if (activeToggle == current.toggle)
                    DeactivateAll();
            });
        }
    }

    private void Activate(ToggleItem selectedItem)
    {
        updating = true;

        foreach (var item in items)
        {
            bool selected = item == selectedItem;

            if (item.toggle != null)
                item.toggle.isOn = selected;

            if (item.targetObject != null)
                item.targetObject.SetActive(selected);

            if (item.toggle != null)
            {
                var images = item.toggle.GetComponentsInChildren<Image>(true);

                foreach (var img in images)
                    img.color = selected ? new Color(0.8f, 0.8f, 0.8f) : Color.white;
            }

            if (selected)
            {
                activeToggle = item.toggle;
                activeObject = item.targetObject;
            }
        }

        updating = false;
    }

    private void ApplyState()
    {
        foreach (var item in items)
        {
            if (item.toggle != null && item.toggle.isOn)
            {
                Activate(item);
                return;
            }
        }

        DeactivateAll();
    }

    public void DeactivateAll()
    {
        updating = true;

        activeToggle = null;
        activeObject = null;

        foreach (var item in items)
        {
            if (item.toggle != null)
                item.toggle.isOn = false;

            if (item.targetObject != null)
                item.targetObject.SetActive(false);

            if (item.toggle != null)
            {
                var images = item.toggle.GetComponentsInChildren<Image>(true);

                foreach (var img in images)
                    img.color = Color.white;
            }
        }

        updating = false;
    }
}