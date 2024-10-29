using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(AnysoundObject))]
    public class AnysoundObjectInspector : UnityEditor.Editor
    {
        private Button _previewButton;
        private AnysoundObject _anysoundObject;
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
            _anysoundObject = target as AnysoundObject;


            var spacer = new VisualElement();
            spacer.style.height = new StyleLength(10);
            root.Add(spacer);

            var parameterSlider = new Slider("Test parameter", 0, 1f)
            {
                showInputField = true,
                style =
                {
                    display = _anysoundObject.ExternalPitchControl || _anysoundObject.ExternalVolumeControl
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
                if (_anysoundObject.GetLooping())
                {
                    if (AnysoundRuntime.IsPreviewing(_anysoundObject))
                    {
                        AnysoundRuntime.StopPreview(_anysoundObject, () => { SetPreviewButtonText("Preview"); });
                        SetPreviewButtonText("Stopping");
                    }
                    else
                    {
                        AnysoundRuntime.StartPreview(_anysoundObject);
                        SetPreviewButtonText("Stop");
                    }
                }
                else
                {
                    AnysoundRuntime.StartPreview(_anysoundObject);
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