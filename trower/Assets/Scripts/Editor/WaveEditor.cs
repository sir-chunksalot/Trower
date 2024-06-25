using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(Wave))]
//public class WaveEditor : Editor
//{
//    Wave wave;

//    private void OnEnable()
//    {
//        wave = target as Wave;
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        if(wave.Sprite == null) {
//            return;
//        }

//        //get reference to sprite
//        Texture2D sprite = AssetPreview.GetAssetPreview(wave.Sprite);

//        //define image size
//        GUILayout.Label("", GUILayout.Height(120), GUILayout.Width(120));

//        //draw it
//        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), sprite);
//    }
//}
