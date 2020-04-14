using System;
using UnityEditor;
using UnityEngine;

namespace Bolt.Samples.Encryption
{
    [CustomEditor(typeof(SetupEncryptionSystem))]
    public class SetupEncryptionSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate keys"))
            {
                ((SetupEncryptionSystem) target).GenerateKeys();
            }
        }
    }
}