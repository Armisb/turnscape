using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LoaderBehaviour : MonoBehaviour
{
    public static List<LoaderBehaviour> Loaders = new();

    public abstract bool isLoaded { get; set; }

    public abstract IEnumerator LoadWithDependencies(string sceneName = "");

    protected abstract void Load(string sceneName = "");

    protected abstract void SceneReload(string sceneName = "");

    protected abstract void Apply(string sceneName = "");

    protected virtual void Awake()
    {
        
    }

    public static IEnumerator LoadAllUnloaded(string sceneName = "")
    {
        foreach (var loader in Loaders)
        {
            yield return loader.LoadWithDependencies(sceneName);
        }

        foreach (var loader in Loaders)
        {
            loader.Apply();
            yield return null;
        }
    }

    public static IEnumerator SceneReloadAll(string sceneName = "")
    {
        foreach (var loader in Loaders)
        {
            loader.SceneReload(sceneName);
            yield return null;
        }
    }
}

public abstract class LoaderBehaviour<T> : LoaderBehaviour where T : LoaderBehaviour
{
    public static T Instance { get; private set; }

    public override bool isLoaded { get; set; } = false;

    public virtual List<Type> Dependencies => new();

    protected sealed override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        if (!Loaders.Contains(this))
        {
            Loaders.Add(this);
        }
    }

    public sealed override IEnumerator LoadWithDependencies(string sceneName = "")
    {
        if (!isLoaded)
        {
            foreach (var depType in Dependencies)
            {
                var dep = Loaders.FirstOrDefault(l => l.GetType() == depType);
                if (dep != null && !dep.isLoaded)
                {
                    yield return dep.LoadWithDependencies(sceneName);
                }
            }

            Load(sceneName);

            isLoaded = true;
        }
    }

    protected abstract override void Load(string sceneName = "");
    protected abstract override void SceneReload(string sceneName = "");
    protected abstract override void Apply(string sceneName = "");
}
