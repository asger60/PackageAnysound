using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Anysound.SoundPositionMode))]
    public class SoundPositionModeDrawer : PropertyDrawer
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

            var typeProperty = property.FindPropertyRelative("soundPositionType");
            PropertyField typeField = new PropertyField(typeProperty);
            container.Add(typeField);

            PropertyField distanceField = new PropertyField(property.FindPropertyRelative("maxDistance"));
            distanceField.style.display = new StyleEnum<DisplayStyle>(typeProperty.enumValueIndex == 0 ? DisplayStyle.Flex : DisplayStyle.None);
            container.Add(distanceField);


            typeField.RegisterValueChangeCallback(evt =>
            {
                distanceField.style.display = new StyleEnum<DisplayStyle>(typeProperty.enumValueIndex == 0 ? DisplayStyle.Flex : DisplayStyle.None);

            });

            return container;
        }
    }
}