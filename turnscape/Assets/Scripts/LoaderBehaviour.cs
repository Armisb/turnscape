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
    public abstract void LoadWithDependencies();

    protected abstract void Load();

    protected abstract void Apply();

    protected virtual void Awake()
    {

    }

    public static void LoadAllUnloaded()
    {
        foreach (var loader in LoaderBehaviour.Loaders)
        {
            loader.LoadWithDependencies();
        }

        foreach (var loader in LoaderBehaviour.Loaders)
        {
            loader.Apply();
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

    public sealed override void LoadWithDependencies()
    {
        if (!isLoaded)
        {
            foreach (var depType in Dependencies)
            {
                var dep = Loaders.FirstOrDefault(l => l.GetType() == depType);
                if (dep != null && !dep.isLoaded)
                    dep.LoadWithDependencies();
            }
            Load();
            isLoaded = true;
        }
    }

    protected abstract override void Load();

    protected abstract override void Apply();
}
