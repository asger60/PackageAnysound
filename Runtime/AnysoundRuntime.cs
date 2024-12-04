using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class AnysoundRuntime : MonoBehaviour
{
    private static AnysoundRuntime _instance;

    private static AnysoundRuntime Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<AnysoundRuntime>();
            }

            return _instance;
        }
    }

    private AudioSource[] _sources;
    private List<AnysoundObjectTracker> _trackers = new List<AnysoundObjectTracker>();
    [Range(1, 200)] [SerializeField] private int voices = 100;
    private Camera _camera;
    private bool _isInit;
    private bool _executeInEditMode;
    private double _prevTime;
    public static bool ShowExtendedSettings;
    public static AudioClip DebugClip;

    public static float DeltaTime
    {
        get
        {
            if (Application.isPlaying)
            {
                return Time.deltaTime;
            }

            return (float)(EditorApplication.timeSinceStartup - Instance._prevTime);
        }
    }

    private void Start()
    {
        _isInit = false;
        Init();

        _executeInEditMode = false;
    }

    public static void Init() => Instance.DoInit();

    void DoInit()
    {
        if (_isInit) return;
        foreach (var audioSource in GetComponentsInChildren<AudioSource>())
        {
            if (audioSource.gameObject == gameObject) continue;
            DestroyImmediate(audioSource.gameObject);
        }

        _camera = Camera.main;
        _trackers = new List<AnysoundObjectTracker>(voices);
        for (int i = 0; i < voices; i++)
        {
            var sourceObject = new GameObject("AnysoundSource");
            var source = sourceObject.AddComponent<AudioSource>();
            sourceObject.transform.SetParent(transform);
            _trackers.Add(new AnysoundObjectTracker(source));
        }
#if UNITY_EDITOR
        DebugClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Packages/com.floppyclub.anysound/Runtime/Resources/DebugPling.wav");
#endif
        _isInit = true;
    }

    private void OnValidate()
    {
        _isInit = false;
    }

#if UNITY_EDITOR
    static AnysoundRuntime()
    {
        EditorApplication.update += EditorUpdate;
    }
#endif
    static void EditorUpdate()
    {
        if (Application.isPlaying) return;

        //if (Instance._executeInEditMode)
        {
            Instance.Update();
        }
    }


    public static void StartPreview(Anysound sound)
    {
        if (!Instance._isInit) Init();
        if (!Application.isPlaying) Instance._executeInEditMode = true;
        Instance.GetFreeTracker()?.Play(sound, Instance.gameObject);
    }

    public static void StopPreview(Anysound sound, Action onStopped = null)
    {
        if (!Instance._isInit) Init();
        if (!Application.isPlaying) Instance._executeInEditMode = true;
        foreach (var tracker in Instance.GetTrackers(sound, Instance.gameObject))
        {
            tracker.Stop(onStopped);
        }
    }

    public static void Play(Anysound sound, GameObject gameObject) => Instance?.DoPlay(sound, gameObject);


    public static void Stop(Anysound sound, GameObject gameObject) => Instance?.DoStop(sound, gameObject);


    public static void SetParameter(Anysound sound, GameObject parentObject, float value) => Instance?.DoSetParameter(sound, parentObject, value);


    public static void SetPreviewParameter(float value)
    {
        foreach (var tracker in Instance.GetTrackers(Instance.gameObject))
        {
            tracker.SetParameter(value);
        }
    }

    void DoSetParameter(Anysound sound, GameObject parentObject, float value)
    {
        var trackers = GetTrackers(sound, parentObject);
        foreach (var tracker in trackers)
        {
            tracker.SetParameter(value);
        }
    }


    public static bool IsPreviewing(Anysound sound)
    {
        if (!Instance._isInit) Init();
        return Instance.GetTrackers(sound, Instance.gameObject).Length > 0;
    }


    void DoPlay(Anysound sound, GameObject parentObject)
    {
        if (!_isInit) Init();
        if (parentObject == null)
            parentObject = gameObject;
        GetFreeTracker()?.Play(sound, parentObject);
    }

    void DoStop(Anysound sound, GameObject parentObject)
    {
        if (!_isInit) Init();
        if (parentObject == null) return;

        foreach (var tracker in GetTrackers(sound, parentObject))
        {
            tracker.Stop();
        }
    }

    AnysoundObjectTracker GetFreeTracker()
    {
        foreach (var tracker in _trackers)
        {
            if (tracker.IsFree) return tracker;
        }

        float bestPercent = 0;
        AnysoundObjectTracker furthestTracker = null;
        foreach (var tracker in _trackers)
        {
            var thisPercent = tracker.GetPlaybackPercent();
            if (thisPercent > bestPercent)
            {
                bestPercent = thisPercent;
                furthestTracker = tracker;
            }
        }

        return furthestTracker;
    }


    AnysoundObjectTracker[] GetTrackers(GameObject parentObject)
    {
        List<AnysoundObjectTracker> trackers = new List<AnysoundObjectTracker>();
        foreach (var tracker in _trackers)
        {
            if (tracker.Parent == parentObject) trackers.Add(tracker);
        }

        return trackers.ToArray();
    }

    AnysoundObjectTracker[] GetTrackers(Anysound sound, GameObject parentObject)
    {
        List<AnysoundObjectTracker> trackers = new List<AnysoundObjectTracker>();
        foreach (var tracker in _trackers)
        {
            if (tracker.Anysound == sound && tracker.Parent == parentObject) trackers.Add(tracker);
        }

        return trackers.ToArray();
    }


    private void Update()
    {
        foreach (var tracker in Instance._trackers)
        {
            if (tracker.IsFree) continue;
            tracker.Update();
        }

        _prevTime = EditorApplication.timeSinceStartup;
    }

    public static float GetSound2DPan(GameObject gameObject)
    {
        var pos = Instance._camera.WorldToViewportPoint(gameObject.transform.position);
        pos.x -= 0.5f;
        pos.x *= 2;
        return pos.x;
    }
}