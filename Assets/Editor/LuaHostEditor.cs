﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Assets.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LuaHost.LuaScriptHost))]
    class LuaHostEditor:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var luaHost = target as LuaHost.LuaScriptHost;
            EditorGUILayout.Space();
            if (GUILayout.Button("Run"))
            {
                luaHost.CoroutineManager.Reset();
                luaHost.RunScript(luaHost.Script);

            }
        }
    }
}
