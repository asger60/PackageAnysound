#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(Anysound))]
public class AnysoundObjectInspector : UnityEditor.Editor
{
    private Button _previewButton;
    private Anysound _anysound;
    private VisualElement _extendedInspector;
    private Slider _parameterSlider;


    public override VisualElement CreateInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
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

        _parameterSlider = new Slider("Test parameter", 0, 1f)
        {
            showInputField = true,
        };
        RefreshParameterActive();
        _parameterSlider.RegisterValueChangedCallback(evt => { AnysoundRuntime.SetPreviewParameter(evt.newValue); });


        root.TrackSerializedObjectValue(serializedObject, property =>
        {
            RefreshParameterActive();
        });


        root.Add(_parameterSlider);

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

    void RefreshParameterActive()
    {
        _parameterSlider.style.display = _anysound.ExternalPitchControl || _anysound.ExternalVolumeControl
            ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex)
            : new StyleEnum<DisplayStyle>(DisplayStyle.None);
    }

    void SetPreviewButtonText(string text)
    {
        _previewButton.text = text;
    }
}
#endif