using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LoaderBehaviour : MonoBehaviour
{
    public static List<LoaderBehaviour> Loaders = new();

    public static bool IsLoading { get; private set; }
    public static int TotalTasks { get; private set; }
    public static int CompletedTasks { get; private set; }

    public static float Progress =>
        TotalTasks == 0 ? 1f : (float)CompletedTasks / TotalTasks;

    public abstract bool isLoaded { get; set; }
    public abstract void LoadWithDependencies();
    public abstract void PrepareWithDependencies();

    protected abstract IEnumerator Download(CoroutineScope scope);
    protected abstract void Load();
    protected abstract void Apply();
    protected abstract void Unload();
    protected abstract IEnumerator Upload(CoroutineScope scope);

    protected static int isUploading = 0;

    protected virtual void Awake() { }

    public static IEnumerator ReloadAll()
    {
        Debug.Log("Relaoding all");

        IsLoading = true;
        CompletedTasks = 0;
        TotalTasks = 0;

        yield return new WaitUntil(() => isUploading == 0);

        foreach (var loader in Loaders)
            loader.isLoaded = true;

        foreach (var loader in Loaders)
            loader.PrepareWithDependencies();

        yield return new WaitUntil(() => isUploading == 0);

        foreach (var loader in Loaders)
            loader.isLoaded = false;

        var snapshot = Loaders.ToArray();
        TotalTasks += snapshot.Length;

        yield return DownloadAll();

        foreach (var loader in snapshot)
            loader.LoadWithDependencies();

        foreach (var loader in snapshot)
            loader.Apply();

        IsLoading = false;
    }

    public static IEnumerator SaveAll()
    {
        IsLoading = true;
        CompletedTasks = 0;
        TotalTasks = 0;

        yield return new WaitUntil(() => isUploading == 0);

        foreach (var loader in Loaders)
            loader.isLoaded = true;

        foreach (var loader in Loaders)
            loader.PrepareWithDependencies();

        yield return new WaitUntil(() => isUploading == 0);

        var snapshot = Loaders.ToArray();
        TotalTasks += snapshot.Length;

        IsLoading = false;
    }

    public static IEnumerator LoadAllWithoutSaving()
    {
        IsLoading = true;
        CompletedTasks = 0;
        TotalTasks = 0;

        var snapshot = Loaders.ToArray();
        TotalTasks += snapshot.Length;
        foreach (var loader in snapshot)
            loader.isLoaded = false;
        yield return DownloadAll();

        foreach (var loader in snapshot)
            loader.isLoaded = false;

        foreach (var loader in snapshot)
            loader.LoadWithDependencies();

        foreach (var loader in snapshot)
            loader.Apply();

        IsLoading = false;
    }

    private static IEnumerator DownloadAll()
    {
        var snapshot = Loaders.ToArray();

        foreach (var loader in snapshot)
            loader.downloadScope = new CoroutineScope(loader);

        foreach (var loader in snapshot)
        {
            yield return loader.Download(loader.downloadScope);
            yield return loader.downloadScope.Wait();

            CompletedTasks++;
        }
    }

    protected CoroutineScope downloadScope;
    protected CoroutineScope uploadScope;
}

public class CoroutineScope
{
    private readonly MonoBehaviour owner;
    private int running;

    public CoroutineScope(MonoBehaviour owner)
    {
        this.owner = owner;
    }

    public void Run(IEnumerator routine)
    {
        running++;
        owner.StartCoroutine(Wrap(routine));
    }

    private IEnumerator Wrap(IEnumerator routine)
    {
        yield return routine;
        running--;
    }

    public IEnumerator Wait()
    {
        yield return new WaitUntil(() => running == 0);
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
            Loaders.Add(this);
    }

    public sealed override void LoadWithDependencies()
    {
        Debug.Log("loading");

        if (isLoaded)
            return;

        foreach (var depType in Dependencies)
        {
            var dep = Loaders.FirstOrDefault(l => l.GetType() == depType);

            if (dep != null && !dep.isLoaded)
                dep.LoadWithDependencies();
        }

        Load();
        isLoaded = true;
    }

    public sealed override void PrepareWithDependencies()
    {
        if (!isLoaded)
            return;

        foreach (var depType in Dependencies)
        {
            var dep = Loaders.FirstOrDefault(l => l.GetType() == depType);

            if (dep != null && dep.isLoaded)
                dep.PrepareWithDependencies();
        }

        Unload();

        isLoaded = false;
        isUploading++;

        uploadScope = new CoroutineScope(this);
        StartCoroutine(UploadFlow());
    }

    private IEnumerator UploadFlow()
    {
        yield return Upload(uploadScope);
        yield return uploadScope.Wait();

        isUploading--;
    }

    protected override IEnumerator Download(CoroutineScope scope) { yield break; }
    protected override void Load() { }
    protected override void Apply() { }
    protected override void Unload() { }
    protected override IEnumerator Upload(CoroutineScope scope) { yield break; }
}