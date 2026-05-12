using UnityEngine;

public static class HierarchySearchUtils
{
    public static T ResolveBest<T>(
        Transform root,
        out Transform bestObject,
        Vector3? worldPosition = null)
        where T : Component
    {
        bestObject = null;
        T best = null;

        if (root == null)
            return null;

        SearchDown(root, worldPosition, ref bestObject, ref best);
        SearchUp(root, worldPosition, ref bestObject, ref best);

        return best;
    }

    static void SearchDown<T>(
        Transform node,
        Vector3? worldPosition,
        ref Transform bestObject,
        ref T best)
        where T : Component
    {
        if (node == null)
            return;

        if (!node.gameObject.activeInHierarchy)
            return;

        TryCompare(node, worldPosition, ref bestObject, ref best, true);

        foreach (Transform child in node)
            SearchDown(child, worldPosition, ref bestObject, ref best);
    }

    static void SearchUp<T>(
        Transform node,
        Vector3? worldPosition,
        ref Transform bestObject,
        ref T best)
        where T : Component
    {
        Transform parent = node.parent;

        while (parent != null)
        {
            if (parent.gameObject.activeInHierarchy)
                TryCompare(parent, worldPosition, ref bestObject, ref best, false);

            parent = parent.parent;
        }
    }

    static void TryCompare<T>(
        Transform node,
        Vector3? worldPosition,
        ref Transform bestObject,
        ref T best,
        bool childWinsTie)
        where T : Component
    {
        T comp = node.GetComponent<T>();

        if (comp == null)
            return;

        // Position filter
        if (worldPosition.HasValue && !ContainsPoint(node, worldPosition.Value))
            return;

        if (IsBetter(best, comp, childWinsTie))
        {
            best = comp;
            bestObject = node;
        }
    }

    static bool IsBetter<T>(T current, T candidate, bool childWinsTie)
        where T : Component
    {
        if (candidate == null)
            return false;

        if (current == null)
            return true;

        int ca = GetPriority(current);
        int cb = GetPriority(candidate);

        if (cb > ca)
            return true;

        if (cb < ca)
            return false;

        return childWinsTie;
    }

    static int GetPriority(Component c)
    {
        var f = c.GetType().GetField("priority");

        if (f == null || f.FieldType != typeof(int))
            return 0;

        return (int)f.GetValue(c);
    }

    static bool ContainsPoint(Transform t, Vector3 worldPoint)
    {
        // UI RectTransform
        if (t.TryGetComponent<RectTransform>(out var rect))
        {
            Camera cam = Camera.main;

            return RectTransformUtility.RectangleContainsScreenPoint(
                rect,
                worldPoint,
                cam);
        }

        // 2D collider
        if (t.TryGetComponent<Collider2D>(out var col2D))
            return col2D.OverlapPoint(worldPoint);

        // 3D collider
        if (t.TryGetComponent<Collider>(out var col3D))
            return col3D.bounds.Contains(worldPoint);

        // SpriteRenderer fallback
        if (t.TryGetComponent<SpriteRenderer>(out var sprite))
            return sprite.bounds.Contains(worldPoint);

        return false;
    }
}