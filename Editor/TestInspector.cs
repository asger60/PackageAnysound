using UnityEditor;
using UnityEngine.UIElements;


[CustomEditor(typeof(Test))]
    public class TestInspector : Editor
    {

        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our Inspector UI.
            VisualElement myInspector = new VisualElement();

            // Add a simple label.
            myInspector.Add(new Label("This is a custom Inspector"));

            // Return the finished Inspector UI.
            return myInspector;
            
            //yo
        }
    }
