using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LoaderBehaviour : MonoBehaviour
{
    //public static List<LoaderBehaviour> NewLoaders = new();
    public static List<LoaderBehaviour> Loaders = new();

    public abstract bool isLoaded { get; set; }

    public abstract void LoadWithDependencies();
    public abstract void BeforeReloadWithDependencies();

    protected virtual void Load()
    {

    }

    protected virtual void Apply()
    {

    }

    protected virtual void BeforeReload()
    {

    }

    protected virtual void Awake()
    {

    }

    public static void LoadAll()
    {
        foreach (var loader in Loaders)
        {
            loader.BeforeReloadWithDependencies();
        }

        foreach (var loader in Loaders)
        {
            loader.LoadWithDependencies();
        }

        foreach (var loader in Loaders)
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
                {
                    dep.LoadWithDependencies();
                }
            }

            Load();

            isLoaded = true;
        }
    }

    public sealed override void BeforeReloadWithDependencies()
    {
        if (isLoaded)
        {
            foreach (var depType in Dependencies)
            {
                var dep = Loaders.FirstOrDefault(l => l.GetType() == depType);
                if (dep != null && !dep.isLoaded)
                {
                    dep.BeforeReloadWithDependencies();
                }
            }

            BeforeReload();

            isLoaded = false;
        }
    }

    protected override void Load()
    {

    }
    protected override void Apply()
    {

    }
    protected override void BeforeReload()
    {

    }

}
