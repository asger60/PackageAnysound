using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;


    [CustomPropertyDrawer(typeof(Anysound))]
    public class AnysoundObjectDrawer : PropertyDrawer
    {
        private Button _previewButton, _createButton;
        private PropertyField _propertyField;
        private SerializedProperty _property;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _property = property;
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


            _propertyField = new PropertyField(property)
            {
                style =
                {
                    flexGrow = 1
                }
            };
            container.Add(_propertyField);
            _propertyField.BindProperty(property);
            _propertyField.RegisterValueChangeCallback(evt => { RefreshButtonStates(property.boxedValue != null); });


            _createButton = new Button
            {
                text = "Create",
                style =
                {
                    width = 80,
                }
            };
            _createButton.clicked += CreateNew;
            container.Add(_createButton);


            _previewButton = new Button
            {
                text = "Preview",
                style =
                {
                    width = 80,
                }
            };
            _previewButton.clicked += () =>
            {
                Anysound target = (Anysound)property.boxedValue;
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
            RefreshButtonStates(property.boxedValue != null);
            container.Add(_previewButton);


            root.Add(container);
            return root;
        }

        void CreateNew()
        {
            AnysoundRuntime.Init();
            Anysound newSound = ScriptableObject.CreateInstance<Anysound>();
            var uniqueFileName = AssetDatabase.GenerateUniqueAssetPath("Assets/"+_property.displayName+".asset");
            bool assetExists = AssetDatabase.GetMainAssetTypeAtPath(uniqueFileName) != null;
            if (assetExists)
            {
                return;
            }
            AssetDatabase.CreateAsset(newSound, uniqueFileName);
            var assetInProject = AssetDatabase.LoadAssetAtPath<Anysound>(AssetDatabase.GetAssetPath(newSound));
            Debug.Log(assetInProject, assetInProject);
            //Selection.activeObject = assetInProject;
            _property.objectReferenceValue = assetInProject;
            _property.serializedObject.ApplyModifiedProperties();
        }

        void RefreshButtonStates(bool hasValue)
        {
            _createButton.style.display = new StyleEnum<DisplayStyle>(hasValue ? DisplayStyle.None : DisplayStyle.Flex);
            _previewButton.style.display = new StyleEnum<DisplayStyle>(!hasValue ? DisplayStyle.None : DisplayStyle.Flex);
        }

        void SetPreviewButtonText(string text)
        {
            _previewButton.text = text;
        }
    }
