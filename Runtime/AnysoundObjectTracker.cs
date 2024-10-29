using System;
using UnityEngine;

public class AnysoundObjectTracker
{
    private GameObject _parent;
    private AudioSource _source;
    private AnysoundObject _anysoundObject;

    private bool _isFree;
    public bool IsFree => _isFree;
    public AudioSource Source => _source;
    public GameObject Parent => _parent;
    public AnysoundObject AnysoundObject => _anysoundObject;

    private bool _isFadingVolume;

    private Action _onStopped;


    struct Fade
    {
        public float timer;
        public float initialValue;
        public float targetValue;
        public float duration;

        public Fade(float initialValue, float targetValue, float duration)
        {
            timer = 0;
            this.initialValue = initialValue;
            this.targetValue = targetValue;
            this.duration = duration;
        }
    }

    private Fade _fade;
    private float _parameter = 1;

    public AnysoundObjectTracker(AudioSource source)
    {
        _source = source;
        _source.spatialBlend = 1;
        _source.spread = 10;
        _source.dopplerLevel = 0;
        _isFree = true;
    }

    public void Update()
    {
        if (_isFree) return;
        if (_parent == null)
        {
            _source.Stop();
            _isFree = true;
            return;
        }

        if (!_source.isPlaying)
        {
            _isFree = true;
            return;
        }

        if (_anysoundObject.Is2D)
        {
            _source.panStereo = AnysoundRuntime.GetSound2DPan(_parent);
        }

        HandleVolumeAndPitch();


        if (_isFadingVolume)
        {
            _fade.timer += AnysoundRuntime.DeltaTime;
            _source.volume = Mathf.Lerp(_fade.initialValue, _fade.targetValue, _fade.timer / _fade.duration);

            if (_fade.timer > _fade.duration)
            {
                _isFadingVolume = false;
                if (_fade.targetValue == 0)
                {
                    DoStop();
                    return;
                }
            }
        }

        _source.transform.position = _parent.transform.position;
    }

    public void Play(AnysoundObject soundObject, GameObject parentObject)
    {
        _anysoundObject = soundObject;
        _parent = parentObject;
        _isFree = false;
        _source.clip = soundObject.GetAudioClip();
        float volume = soundObject.GetVolume(_parameter);

        _source.volume = volume;
        _source.pitch = soundObject.GetPitch(_parameter);
        _source.loop = soundObject.GetLooping();
        var positionSettings = soundObject.GetSoundPositionSettings();
        _source.spatialBlend = positionSettings.Spatialize ? 1 : 0;
        _source.spatialize = positionSettings.Spatialize;
        _source.panStereo = positionSettings.GetPan(parentObject);
        _source.Play();

        if (_anysoundObject.GetPlaySettings().useFade)
        {
            _fade = new Fade(0, volume, _anysoundObject.GetPlaySettings().fadeDuration);
            _isFadingVolume = true;
        }
    }

    public float GetPlaybackPercent()
    {
        if (_isFree)
            return 100;
        if (_source.clip == null)
            return 100;

        return (_source.time / _source.clip.length) * 100f;
    }

    void DoStop()
    {
        _anysoundObject = null;
        _parent = null;
        _source.Stop();
        _isFree = true;
        _onStopped?.Invoke();
    }

    public void Stop(Action onStopped = null)
    {
        _onStopped = onStopped;
        if (!_anysoundObject.GetStopSettings().useFade)
        {
            DoStop();
        }
        else
        {
            _fade = new Fade(_source.volume, 0, _anysoundObject.GetStopSettings().fadeDuration);
            _isFadingVolume = true;
        }
    }

    public void SetParameter(float value)
    {
        _parameter = value;
        HandleVolumeAndPitch();
    }

    void HandleVolumeAndPitch()
    {
        if (_anysoundObject.ExternalPitchControl)
            _source.pitch = Mathf.Max(_anysoundObject.GetPitch(_parameter), 0.1f);

        if (_anysoundObject.ExternalVolumeControl)
            _source.volume =  Mathf.Pow(_anysoundObject.GetVolume(_parameter), 2);
    }
}