using UnityEngine;

public static class HierarchySearchUtils
{
    public static T ResolveBest<T>(Transform root, out Transform bestObject)
        where T : Component
    {
        bestObject = null;
        T best = null;

        if (root == null) return null;

        SearchDown(root, ref bestObject, ref best);
        SearchUp(root, ref bestObject, ref best);

        return best;
    }

    static void SearchDown<T>(Transform node, ref Transform bestObject, ref T best)
        where T : Component
    {
        if (node == null) return;
        if (!node.gameObject.activeInHierarchy) return;

        TryCompare(node, ref bestObject, ref best, true);

        foreach (Transform child in node)
            SearchDown(child, ref bestObject, ref best);
    }

    static void SearchUp<T>(Transform node, ref Transform bestObject, ref T best)
        where T : Component
    {
        Transform parent = node.parent;

        while (parent != null)
        {
            if (parent.gameObject.activeInHierarchy)
                TryCompare(parent, ref bestObject, ref best, false);

            parent = parent.parent;
        }
    }

    static void TryCompare<T>(Transform node, ref Transform bestObject, ref T best, bool childWinsTie)
        where T : Component
    {
        T comp = node.GetComponent<T>();
        if (comp == null) return;

        if (IsBetter(best, comp, childWinsTie))
        {
            best = comp;
            bestObject = node;
        }
    }

    static bool IsBetter<T>(T current, T candidate, bool childWinsTie)
        where T : Component
    {
        if (candidate == null) return false;
        if (current == null) return true;

        int ca = GetPriority(current);
        int cb = GetPriority(candidate);

        if (cb > ca) return true;
        if (cb < ca) return false;

        return childWinsTie;
    }

    static int GetPriority(Component c)
    {
        var f = c.GetType().GetField("priority");
        if (f == null || f.FieldType != typeof(int)) return 0;
        return (int)f.GetValue(c);
    }
}