using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "AnysoundObject", menuName = "Anysound/AnysoundObject", order = 1)]
public class AnysoundObject : ScriptableObject
{
    [SerializeField] private AudioClip[] audioClips;
    [Range(0, 1f)] [SerializeField] private float volume = 1;
    [Range(0, 10f)] [SerializeField] private float pitch = 1;


    enum ClipSelectMode
    {
        Random,
        Cycle
    }

    [SerializeField] private ClipSelectMode clipSelectMode;
    private int _currentPlayIndex;

    private enum PlayMode
    {
        OneShot,
        Looping
    }

    [SerializeField] private PlayMode playMode;

    public enum SoundPositionMode
    {
        WorldSpace,
        ScreenSpace,
        None
    }

    [SerializeField] private SoundPositionMode soundPositionMode;


    public FadeSettings playSettings;
    public FadeSettings stopSettings;

    [Serializable]
    public struct FadeSettings
    {
        [FormerlySerializedAs("fadeOutDuration")] public float fadeDuration;
        public AnimationCurve fadeCurve;
    }

    public struct SoundPositionSettings
    {
        private SoundPositionMode _positionMode;
        public bool spatialize;

        public SoundPositionSettings(SoundPositionMode positionMode) : this()
        {
            _positionMode = positionMode;
            spatialize = positionMode == SoundPositionMode.WorldSpace;
        }

        public float GetPan(GameObject gameObject)
        {
            switch (_positionMode)
            {
                case SoundPositionMode.WorldSpace:
                    return 0;
                case SoundPositionMode.ScreenSpace:
                    return AnysoundRuntime.GetSound2DPan(gameObject);
                case SoundPositionMode.None:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return 0;
        }
    }

    public bool Is2D => soundPositionMode == SoundPositionMode.ScreenSpace;

    public float GetPitch()
    {
        return pitch;
    }

    public float GetVolume()
    {
        return volume;
    }

    public AudioClip GetAudioClip()
    {
        switch (clipSelectMode)
        {
            case ClipSelectMode.Random:
                return audioClips[Random.Range(0, audioClips.Length)];

            case ClipSelectMode.Cycle:
                _currentPlayIndex++;
                return audioClips[(int)Mathf.Repeat(_currentPlayIndex, audioClips.Length)];

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool GetLooping()
    {
        return playMode == PlayMode.Looping;
    }

    public SoundPositionSettings GetSoundPositionSettings()
    {
        return new SoundPositionSettings(soundPositionMode);
    }

    public FadeSettings GetStopSettings() => stopSettings;
    public FadeSettings GetPlaySettings() => playSettings;
    
}