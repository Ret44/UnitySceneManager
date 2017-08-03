using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : SingletonBehaviour<ScreenManager> {

    [SerializeField]
    private ScreenData[] _screens;
    public ScreenData[] screens
    {
        get { return _screens; }
    }

    public delegate void ScreenLoadingEvent();

    public ScreenLoadingEvent OnLoadingStart;
    public ScreenLoadingEvent OnLoadingEnd;

    private List<string> _currentlyLoaded;
    public static List<string> currentlyLoaded
    {
        get
        {
            if(instance._currentlyLoaded == null)
            {
                instance._currentlyLoaded = new List<string>();
            }
            return instance._currentlyLoaded;
        }
    }

    private static ScreenData GetScreen(string key)
    {
        for (int i = 0; i < instance._screens.Length; i++)
        {
            if (string.Compare(key, instance._screens[i].key) == 0)
            {
                return instance._screens[i];
            }
        }
        return null;
    }

    public IEnumerator LoadScreenCouroutine(ScreenData screenToLoad, string[] extra)
    {
        if (OnLoadingStart != null)
            OnLoadingStart();

        //Unloading previously loaded scenes
        for (int i = 0; i < currentlyLoaded.Count; i++)
        {
            AsyncOperation asyncOp;
            asyncOp = SceneManager.UnloadSceneAsync(currentlyLoaded[i]);
            while (!asyncOp.isDone)
                yield return null;
        }

        currentlyLoaded.Clear();

        //Loading scenes saved in ScreenData asset
        for (int i = 0; i < screenToLoad.scenes.Length; i++)
        {
            AsyncOperation asyncOp;
            asyncOp = SceneManager.LoadSceneAsync(screenToLoad.scenes[i], LoadSceneMode.Additive);
            while (!asyncOp.isDone)
                yield return null;

            instance._currentlyLoaded.Add(screenToLoad.scenes[i]);
        }

        //If needed - loading extra scenes (Basically everything that is not determined in ScreenData asset, but its required (like level assets, geometry, etc.)
        if (extra != null)
        {
            for (int i = 0; i < extra.Length; i++)
            {
                AsyncOperation asyncOp;
                asyncOp = SceneManager.LoadSceneAsync(extra[i], LoadSceneMode.Additive);
                while (!asyncOp.isDone)
                    yield return null;

                instance._currentlyLoaded.Add(extra[i]);
            }
        }

        if (OnLoadingEnd!=null)
            OnLoadingEnd();
    }

    public static void LoadScreen(ScreenData screen, string[] extra = null)
    {
        if (screen == null)
            Debug.LogErrorFormat("Error while trying to load selected screen");
        else
        {
            instance.StartCoroutine(instance.LoadScreenCouroutine(screen, extra));
        }
    }

    public static void LoadScreen(string key, string[] extra = null)
    {
        LoadScreen(GetScreen(key), extra);
    }
}
                                                      