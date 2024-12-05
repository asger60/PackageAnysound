using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace com.floppyclub.anysound.Runtime
{
    [CreateAssetMenu(fileName = "Anysound", menuName = "Anysound/Anysound", order = 1)]
    public class Anysound : ScriptableObject
    {
        [SerializeField] private AudioClip[] audioClips;

        enum ClipSelectMode
        {
            Random,
            Sequential
        }

        [SerializeField] private ClipSelectMode clipSelectMode;

        private enum PlayMode
        {
            OneShot,
            Looping
        }

        [SerializeField] private PlayMode playMode;


        [Serializable]
        public struct SoundPositionMode
        {
            public enum SoundPositionType
            {
                WorldSpace,
                ScreenSpace,
                None
            }

            public SoundPositionType soundPositionType;
            public float maxDistance;
            public float minDistance;

            public SoundPositionMode(SoundPositionType soundPositionMode, float maxDistance)
            {
                this.soundPositionType = soundPositionMode;
                this.maxDistance = maxDistance;
                this.minDistance = 10;
            }
        }


        [SerializeField] private SoundPositionMode soundPositionMode;


        [Serializable]
        public struct ControlledValue
        {
            [Range(0, 1f)] public float value;
            [SerializeField] private ControlSource control;

            public float GetValue(float externalValue)
            {
                if (controlActive)
                    return control.GetControlValue(value, externalValue);
                return value;
            }

            public bool IsExternallyControlled => control.SourceType == ControlSource.ControlSourceTypes.GameParameter;
            [FormerlySerializedAs("_controlActive")] public bool controlActive;

            public void Init(float initialValue)
            {
                value = initialValue;
                control.Init();
                controlActive = false;
            }
        }

        [Serializable]
        public struct ControlSource
        {
            public enum ControlSourceTypes
            {
                Random,
                GameParameter
            }

            [SerializeField] private ControlSourceTypes sourceType;

            [SerializeField] private float randomControlWidth;

            [Range(-1, 1)] [SerializeField] private float randomShift;

            [FormerlySerializedAs("externalControlMin")] [SerializeField]
            private float valueMin;

            [FormerlySerializedAs("externalControlMax")] [SerializeField]
            private float valueMax;


            public float GetControlValue(float initialValue, float externalValue)
            {
                switch (sourceType)
                {
                    case ControlSourceTypes.Random:
                        float width = randomControlWidth * 0.5f;
                        float rangeMin = (width * -1 + (width * randomShift));
                        float rangeMax = width + (width * randomShift);
                        return initialValue + Random.Range(rangeMin, rangeMax);
                    case ControlSourceTypes.GameParameter:
                        return initialValue + Mathf.Lerp(valueMin, valueMax, externalValue);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public ControlSourceTypes SourceType => sourceType;

            public void Init()
            {
                sourceType = ControlSourceTypes.Random;
                randomControlWidth = 0.25f;
                randomShift = 0;
                valueMin = 0;
                valueMax = 1;
            }
        }


        [SerializeField] private ControlledValue volume;
        [SerializeField] private ControlledValue pitch;


        private int _currentPlayIndex;


        public FadeSettings playSettings;
        public FadeSettings stopSettings;

        [Serializable]
        public struct FadeSettings
        {
            public bool useFade;
            public float fadeDuration ;
        
        }

        public struct SoundPositionSettings
        {
            private SoundPositionMode.SoundPositionType _positionMode;
            public readonly bool Spatialize;
            public readonly float MaxDistance;
            public readonly float MinDistance;
        

            public SoundPositionSettings(SoundPositionMode positionMode) : this()
            {
                MaxDistance = positionMode.maxDistance;
                MinDistance = positionMode.minDistance;
                _positionMode = positionMode.soundPositionType;
                Spatialize = _positionMode == SoundPositionMode.SoundPositionType.WorldSpace;
            }

            public float GetPan(GameObject gameObject)
            {
                switch (_positionMode)
                {
                    case SoundPositionMode.SoundPositionType.WorldSpace:
                        return 0;
                    case SoundPositionMode.SoundPositionType.ScreenSpace:
                        return AnysoundRuntime.GetSound2DPan(gameObject);
                    case SoundPositionMode.SoundPositionType.None:
                        return 0;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }

        public bool ExternalPitchControl => pitch.controlActive && pitch.IsExternallyControlled;
        public bool ExternalVolumeControl => volume.controlActive &&  volume.IsExternallyControlled;

        public bool Is2D => soundPositionMode.soundPositionType == SoundPositionMode.SoundPositionType.ScreenSpace;

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
            if (audioClips == null || audioClips.Length == 0) return null;

            switch (clipSelectMode)
            {
                case ClipSelectMode.Random:
                    return audioClips[Random.Range(0, audioClips.Length)];

                case ClipSelectMode.Sequential:
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

        public Anysound()
        {
            volume.value = 1;
            pitch.value = 1;
            soundPositionMode = new SoundPositionMode(SoundPositionMode.SoundPositionType.None, 100);
            audioClips = new AudioClip[1];
            audioClips[0] = AnysoundRuntime.DebugClip;
            pitch.Init(1);
            volume.Init(0.8f);
            playSettings.fadeDuration = 0.2f;
            stopSettings.fadeDuration = 0.2f;
        }
    }
}