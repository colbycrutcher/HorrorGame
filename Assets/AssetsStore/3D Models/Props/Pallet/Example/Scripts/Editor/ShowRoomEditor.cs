using UnityEditor;
using UnityEngine;

namespace Voo.ShowRoom
{
    [CustomEditor(typeof(ShowRoom))]
    public class ShowRoomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var showRoom = (ShowRoom)target;

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Preview [{showRoom.PreviewIndex}]"))
                showRoom.Preview();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var prevIndex = showRoom.CurrentIndex - 1;
            if (prevIndex < 0)
                prevIndex = showRoom.Objects.Length - 1;
            if (GUILayout.Button($"Prev [{prevIndex}]"))
                showRoom.Prev();
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label($"[{showRoom.CurrentIndex}]", GUILayout.Width(30f));
            var nextIndex = showRoom.CurrentIndex + 1;
            if (nextIndex >= showRoom.Objects.Length)
                nextIndex = 0;
            if (GUILayout.Button($"Next [{nextIndex}]"))
                showRoom.Next();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Recreate"))
                showRoom.Recreate();
            GUILayout.EndHorizontal();

            GUI.enabled = showRoom.CurrentObject != null;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Save Object Settings {(showRoom.CurrentObject == null ? "[No Selected Object!]" : "")}"))
                showRoom.SaveCurrentObjectSetting();
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }
    }
}