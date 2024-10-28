using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AnysoundObject))]
    public class AnysoundObjectDrawer : PropertyDrawer
    {
        private Button _button;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement
            {
                style =
                {
                    flexGrow = 1
                }
            };

            VisualElement container = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row
                }
            };


            PropertyField propertyField = new PropertyField(property)
            {
                style =
                {
                    flexGrow = 1
                }
            };
            container.Add(propertyField);
            propertyField.BindProperty(property);


            _button = new Button
            {
                text = "Preview",
                style =
                {
                    width = 80,
                }
            };
            _button.clicked += () =>
            {
                AnysoundObject target = (AnysoundObject)property.boxedValue;
                if (target.GetLooping())
                {
                    if (AnysoundRuntime.IsPreviewing(target))
                    {
                        AnysoundRuntime.StopPreview(target, () => { SetPreviewButtonText("Preview"); });
                        SetPreviewButtonText("Stopping");
                    }
                    else
                    {
                        AnysoundRuntime.StartPreview(target);
                        SetPreviewButtonText("Stop");
                    }
                }
                else
                {
                    AnysoundRuntime.StartPreview(target);
                    SetPreviewButtonText("Preview");
                }
            };
            container.Add(_button);


            root.Add(container);
            return root;
        }

        void SetPreviewButtonText(string text)
        {
            _button.text = text;
        }
    }
}