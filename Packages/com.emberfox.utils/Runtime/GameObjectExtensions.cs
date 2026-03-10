using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (!comp)
            comp = go.AddComponent<T>();
        return comp;
    }
    
    public static void SetLayerRecursive(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
    
    public static T FindObjectOfTypeInAllScenes<T>() where T : Component
    {
        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            GameObject[] rootObjects = scene.GetRootGameObjects();

            foreach (GameObject root in rootObjects)
            {
                T found = root.GetComponentInChildren<T>(true);
                if (found != null)
                    return found;
            }
        }

        return null;
    }
    
    public static bool TryGetComponentInParent<T>(this Component component, out T result) where T : class
    {
        return component.gameObject.TryGetComponentInParent(out result);
    }

    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T result) where T : class
    {
        result = null;
        if (gameObject == null) return false;

        // Fast path for concrete Component types
        if (typeof(Component).IsAssignableFrom(typeof(T)))
        {
            result = gameObject.GetComponentInParent(typeof(T)) as T;
            return result != null;
        }

        // Interface / non-Component path: scan components in parents and cast
        var comps = gameObject.GetComponentsInParent<Component>(true);
        for (int i = 0; i < comps.Length; i++)
        {
            var c = comps[i];
            if (c is T t)
            {
                result = t;
                return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// Finds the first child (recursively) with the specified tag.
    /// Only searches within the children of this GameObject.
    /// Returns null if none found.
    /// </summary>
    public static GameObject FindChildWithTag(this GameObject parent, string tag, bool includeInactive = false)
    {
        foreach (Transform child in parent.transform)
        {
            if (!includeInactive && !child.gameObject.activeInHierarchy)
                continue;
            
            if (child.CompareTag(tag))
                return child.gameObject;

            var result = child.gameObject.FindChildWithTag(tag);
            if (result != null)
                return result;
        }

        return null;
    }
}
