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

    public abstract IEnumerator LoadWithDependencies();
    public abstract IEnumerator BeforeReloadWithDependencies();

    protected virtual IEnumerator Load()
    {
        yield break;
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

    public static IEnumerator LoadAll()
    {
        foreach (var loader in Loaders)
        {
            yield return loader.BeforeReloadWithDependencies();
        }

        foreach (var loader in Loaders)
        {
            yield return loader.LoadWithDependencies();
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

    public sealed override IEnumerator LoadWithDependencies()
    {
        if (isLoaded)
            yield break;

        foreach (var depType in Dependencies)
        {
            var dep = Loaders.FirstOrDefault(l => l.GetType() == depType);

            if (dep != null && !dep.isLoaded)
            {
                yield return dep.LoadWithDependencies();
            }
        }

        yield return Load();

        isLoaded = true;
    }

    public sealed override IEnumerator BeforeReloadWithDependencies()
    {
        if (!isLoaded)
            yield break;

        foreach (var depType in Dependencies)
        {
            var dep = Loaders.FirstOrDefault(l => l.GetType() == depType);

            if (dep != null && dep.isLoaded)
            {
                yield return dep.BeforeReloadWithDependencies();
            }
        }

        BeforeReload();

        isLoaded = false;
    }

    protected override IEnumerator Load()
    {
        yield break;
    }

    protected override void Apply()
    {

    }

    protected override void BeforeReload()
    {

    }
}