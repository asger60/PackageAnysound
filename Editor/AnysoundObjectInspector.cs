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

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement defaultInspector = new VisualElement();
            InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);
            _anysoundObject = target as AnysoundObject;

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

            defaultInspector.Add(_previewButton);
            return defaultInspector;
        }

        void SetPreviewButtonText(string text)
        {
            _previewButton.text = text;
        }
    }
}