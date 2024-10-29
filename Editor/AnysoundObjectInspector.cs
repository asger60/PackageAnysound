using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(Anysound))]
    public class AnysoundObjectInspector : UnityEditor.Editor
    {
        private Button _previewButton;
        private Anysound _anysound;
        private VisualElement _extendedInspector;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            var foldOut = new Foldout
            {
                value = AnysoundRuntime.ShowExtendedSettings,
                text = "Settings"
            };
            root.Add(foldOut);
            _extendedInspector = new VisualElement();
            InspectorElement.FillDefaultInspector(_extendedInspector, serializedObject, this);
            _extendedInspector.style.display = new StyleEnum<DisplayStyle>(foldOut.value ? DisplayStyle.Flex : DisplayStyle.None);
            root.Add(_extendedInspector);
            foldOut.RegisterValueChangedCallback(e =>
            {
                AnysoundRuntime.ShowExtendedSettings = foldOut.value;
                _extendedInspector.style.display = new StyleEnum<DisplayStyle>(foldOut.value ? DisplayStyle.Flex : DisplayStyle.None);
            });
            _anysound = target as Anysound;


            var spacer = new VisualElement();
            spacer.style.height = new StyleLength(10);
            root.Add(spacer);

            var parameterSlider = new Slider("Test parameter", 0, 1f)
            {
                showInputField = true,
                style =
                {
                    display = _anysound.ExternalPitchControl || _anysound.ExternalVolumeControl
                        ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex)
                        : new StyleEnum<DisplayStyle>(DisplayStyle.None)
                }
            };
            
            parameterSlider.RegisterValueChangedCallback(evt =>
            {
                AnysoundRuntime.SetPreviewParameter(evt.newValue);
            });


            root.Add(parameterSlider);


            _previewButton = new Button(() =>
            {
                if (_anysound.GetLooping())
                {
                    if (AnysoundRuntime.IsPreviewing(_anysound))
                    {
                        AnysoundRuntime.StopPreview(_anysound, () => { SetPreviewButtonText("Preview"); });
                        SetPreviewButtonText("Stopping");
                    }
                    else
                    {
                        AnysoundRuntime.StartPreview(_anysound);
                        SetPreviewButtonText("Stop");
                    }
                }
                else
                {
                    AnysoundRuntime.StartPreview(_anysound);
                    SetPreviewButtonText("Preview");
                }
            });
            SetPreviewButtonText("Preview");

            root.Add(_previewButton);
            return root;
        }

        void SetPreviewButtonText(string text)
        {
            _previewButton.text = text;
        }
    }
}