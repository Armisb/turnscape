using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class MouseRaycastUtils
{
    public static Transform GetObjectUnderMouse(EventSystem eventSystem, Camera cam)
    {
        Transform ui = GetUI(eventSystem);
        if (ui != null) return ui;

        return GetWorld(cam);
    }

    static Transform GetUI(EventSystem eventSystem)
    {
        if (eventSystem == null) return null;

        PointerEventData data = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(data, results);

        foreach (var r in results)
        {
            if (r.gameObject == null) continue;
            if (!r.gameObject.activeInHierarchy) continue;

            return r.gameObject.transform;
        }

        return null;
    }

    static Transform GetWorld(Camera cam)
    {
        if (cam == null) return null;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit3D))
            return hit3D.collider.transform;

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        if (hit2D.collider != null)
            return hit2D.collider.transform;

        return null;
    }
}