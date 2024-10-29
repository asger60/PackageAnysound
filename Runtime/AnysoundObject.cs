using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "AnysoundObject", menuName = "Anysound/AnysoundObject", order = 1)]
public class AnysoundObject : ScriptableObject
{
    [SerializeField] private AudioClip[] audioClips;

    enum ClipSelectMode
    {
        Random,
        Cycle
    }

    [SerializeField] private ClipSelectMode clipSelectMode;

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

    [Serializable]
    public struct ControlledValue
    {
        [Range(0, 1f)] public float value;
        [SerializeField] private ControlSource control;

        public float GetValue(float externalValue) => control.GetControlValue(value, externalValue);

        public bool IsExternallyControlled => control.SourceType == ControlSource.ControlSourceTypes.ExternalParameter;
    }

    [Serializable]
    public struct ControlSource
    {
        public enum ControlSourceTypes
        {
            InternalRandom,
            ExternalParameter
        }

        [SerializeField] private ControlSourceTypes sourceType;

        [SerializeField] private float randomControlWidth;

        [Range(-1, 1)] [SerializeField] private float randomShift;

        [FormerlySerializedAs("externalControlMin")] [SerializeField] private float valueMin;
        [FormerlySerializedAs("externalControlMax")] [SerializeField] private float valueMax;

        public float GetControlValue(float initialValue, float externalValue)
        {
            switch (sourceType)
            {
                case ControlSourceTypes.InternalRandom:
                    float width = randomControlWidth * 0.5f;
                    float rangeMin = (width * -1 + (width * randomShift));
                    float rangeMax = width + (width * randomShift);
                    return initialValue + Random.Range(rangeMin, rangeMax);
                case ControlSourceTypes.ExternalParameter:
                    return initialValue + Mathf.Lerp(valueMin, valueMax, externalValue);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ControlSourceTypes SourceType => sourceType;
    }


    //[SerializeField] private float volume = 1;
    [FormerlySerializedAs("volumeValue")] [SerializeField]
    private ControlledValue volume;


    [SerializeField] private ControlledValue pitch;


    private int _currentPlayIndex;


    public FadeSettings playSettings;
    public FadeSettings stopSettings;

    [Serializable]
    public struct FadeSettings
    {
        public bool useFade;
        public float fadeDuration;
    }

    public struct SoundPositionSettings
    {
        private SoundPositionMode _positionMode;
        public readonly bool Spatialize;

        public SoundPositionSettings(SoundPositionMode positionMode) : this()
        {
            _positionMode = positionMode;
            Spatialize = positionMode == SoundPositionMode.WorldSpace;
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

    public bool ExternalPitchControl => pitch.IsExternallyControlled;
    public bool ExternalVolumeControl => volume.IsExternallyControlled;

    public bool Is2D => soundPositionMode == SoundPositionMode.ScreenSpace;

    public float GetPitch(float externalValue)
    {
        return pitch.GetValue(externalValue);
    }

    public float GetVolume(float externalValue)
    {
        var val = volume.GetValue(externalValue);
        return val;
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