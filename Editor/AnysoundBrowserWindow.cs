using System;
using System.Collections.Generic;
using System.Linq;
using PackageAnysound.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PackageAnysound.Editor
{
    public class AnysoundBrowserWindow : EditorWindow
    {
        private AnysoundPack[] _packs = Array.Empty<AnysoundPack>();
        private VisualElement _root;
        private VisualElement _soundPreviewElement, _tagsElement, _packsElement, _soundsElement;
        private AnysoundPack _currentPack;
        private List<string> _allTags = new List<string>();
        private List<string> _currentTags = new List<string>();
        private List<Anysound> _currentSounds = new List<Anysound>();

        [MenuItem("Examples/My Editor Window")]
        public static void ShowExample()
        {
            AnysoundBrowserWindow browserWindow = GetWindow<AnysoundBrowserWindow>();
            browserWindow.titleContent = new GUIContent("AnysoundBrowser");
            browserWindow._packs = browserWindow.GetPacks();
            browserWindow._allTags = browserWindow.GetTags();
            browserWindow._currentSounds = browserWindow.GetSounds(browserWindow._packs);
            browserWindow.UpdateUI();
        }

        public void CreateGUI()
        {
            _root = rootVisualElement;
            _soundPreviewElement = new VisualElement();
            _tagsElement = new VisualElement();
            _packsElement = new VisualElement();
            _soundsElement = new VisualElement();
            _root.Add(_tagsElement);
            _root.Add(_soundPreviewElement);
            _root.Add(_packsElement);
            _root.Add(_soundsElement);
        }

        void UpdateUI()
        {
            foreach (var tag in _allTags)
            {
                var tagButton = new Button(() => { _currentTags.Add(tag); });
                tagButton.text = tag;
                _tagsElement.Add(tagButton);
            }


            foreach (var anysoundPack in _packs)
            {
                var packButton = new Button(() =>
                {
                    if (anysoundPack == _currentPack)
                        _currentPack = null;
                    else
                        _currentPack = anysoundPack;


                    UpdateCurrentPack();
                })
                {
                    text = anysoundPack.name
                };

                _packsElement.Add(packButton);
            }

            UpdateSoundsButtons();
        }

        void UpdateSoundsButtons()
        {
            _soundsElement.Clear();
            foreach (var anysound in _currentSounds)
            {
                var previewButton = new Button(() => { AnysoundRuntime.StartPreview(anysound); })
                {
                    text = anysound.name
                };
                _soundsElement.Add(previewButton);
            }
        }


        void UpdateCurrentPack()
        {
            _soundPreviewElement.Clear();
            _currentSounds.Clear();
            _currentSounds.AddRange(GetSounds(_packs));
            if (_currentPack)
            {
                for (var i = _currentSounds.Count - 1; i >= 0; i--)
                {
                    var anysound = _currentSounds[i];
                    if (_currentPack.sounds.Contains(anysound))
                        continue;

                    _currentSounds.Remove(anysound);
                }
            }


            UpdateSoundsButtons();
        }

        List<string> GetTags()
        {
            List<string> tags = new List<string>();
            foreach (var pack in _packs)
            {
                foreach (var anysound in pack.sounds)
                {
                    tags.AddRange(anysound.tags);
                }
            }

            return tags;
        }

        List<Anysound> GetSounds(AnysoundPack[] packs)
        {
            List<Anysound> sounds = new List<Anysound>();
            foreach (var pack in packs)
            {
                foreach (var anysound in pack.sounds)
                {
                    sounds.Add(anysound);
                }
            }

            return sounds;
        }

        AnysoundPack[] GetPacks()
        {
            var assets = Resources.FindObjectsOfTypeAll<AnysoundPack>();
            Debug.Log("assets " + assets.Length);
            return assets;
        }
    }
}