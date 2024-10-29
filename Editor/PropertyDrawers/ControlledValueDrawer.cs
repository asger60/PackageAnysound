using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AnysoundObject.ControlledValue))]
    public class ControlledValueDrawer : PropertyDrawer
    {
        private PropertyField _durationField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();
            var label = new Label(property.displayName);
            label.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            label.style.marginTop = 8;
            label.style.marginLeft = 3;
            container.Add(label);
            
            PropertyField valueField = new PropertyField(property.FindPropertyRelative("value"));
            container.Add(valueField);
            
            PropertyField controlField = new PropertyField(property.FindPropertyRelative("control"));
            container.Add(controlField);
//
            //_durationField = new PropertyField(property.FindPropertyRelative("fadeDuration"));
            //container.Add(_durationField);
            //boolField.RegisterValueChangeCallback(evt => { SetDurationVisible(evt.changedProperty.boolValue); });
//
            //SetDurationVisible((property.FindPropertyRelative("useFade").boolValue));

            return container;
        }

        void SetDurationVisible(bool state)
        {
            _durationField.style.display = new StyleEnum<DisplayStyle>(state ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
}