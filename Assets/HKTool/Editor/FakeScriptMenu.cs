using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using HKTool.Unity;

[CustomEditor(typeof(FakeScript))]
public class FakeScriptMenu : Editor
{
    
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            OnInspectorGUIPlaying();
        }
        else
        {
            OnInspectorGUIEditor();
        }
    }
    private void OnInspectorGUIPlaying()
    {
        FakeScript fs = target as FakeScript;
        if (fs == null) return;
        var sd = fs.scriptData;
        if (sd == null)
        {
            return;
        }
        Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        foreach (var v in sd.fields)
        {
            if (v.fieldType == FakeScriptData.FieldType.GameObject)
            {
                fs.SetData(v.name, EditorGUILayout.ObjectField(v.name,
                    (UnityEngine.Object)fs.GetData(v.name), typeof(GameObject), true));
            }
            else if (v.fieldType == FakeScriptData.FieldType.Component || v.fieldType == FakeScriptData.FieldType.UnityObject)
            {
                if (!typeCache.TryGetValue(v.type, out var type))
                {
                    foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        var t = ass.GetType(v.type);
                        if (t != null)
                        {
                            type = t;
                            typeCache.Add(v.type, t);
                            break;
                        }
                    }
                }
                if (type == null) continue;
                fs.SetData(v.name, EditorGUILayout.ObjectField(v.name,
                    (UnityEngine.Object)fs.GetData(v.name), type, true));
            }
            else if (v.fieldType == FakeScriptData.FieldType.Int)
            {
                fs.SetData(v.name, EditorGUILayout.IntField(v.name, (int)(fs.GetData(v.name) ?? 0)));
            }
            else if (v.fieldType == FakeScriptData.FieldType.Bool)
            {
                fs.SetData(v.name, EditorGUILayout.Toggle(v.name, (bool)(fs.GetData(v.name) ?? false)));
            }
            else if (v.fieldType == FakeScriptData.FieldType.Float)
            {
                fs.SetData(v.name, EditorGUILayout.FloatField(v.name, (float)(fs.GetData(v.name) ?? 0f)));
            }
            else if (v.fieldType == FakeScriptData.FieldType.String)
            {
                fs.SetData(v.name, EditorGUILayout.TextField(v.name, (string)fs.GetData(v.name)));
            }
            else if (v.fieldType == FakeScriptData.FieldType.Vector2)
            {
                fs.SetData(v.name,
                    EditorGUILayout.Vector2Field(v.name, (Vector2)(fs.GetData(v.name) ?? Vector2.zero)));
            }
            else if (v.fieldType == FakeScriptData.FieldType.Vector3)
            {
                fs.SetData(v.name,
                    EditorGUILayout.Vector3Field(v.name, (Vector3)(fs.GetData(v.name) ?? Vector3.zero)));
            }
            else if (v.fieldType == FakeScriptData.FieldType.Vector4)
            {
                fs.SetData(v.name,
                    EditorGUILayout.Vector4Field(v.name, (Vector4)(fs.GetData(v.name) ?? Vector4.zero)));
            }
            else if (v.fieldType == FakeScriptData.FieldType.Quaternion)
            {
                Vector4 vec = EditorGUILayout.Vector4Field(v.name, (Vector4)(fs.GetData(v.name) ?? Vector2.zero));
                fs.SetData(v.name, new Quaternion(vec.x, vec.y, vec.z, vec.w));
            }
        }
    }
    private void OnInspectorGUIEditor()
    {
        FakeScript fs = target as FakeScript;
        if (fs == null) return;
        var osd = fs.scriptData;
        var sd = fs.scriptData = EditorGUILayout.ObjectField("Script Data", fs.scriptData, typeof(FakeScriptData), false)
            as FakeScriptData;
        if (sd != osd)
        {
            EditorUtility.SetDirty(fs);
        }
        if (sd == null)
        {
            return;
        }
        Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        foreach (var v in sd.fields)
        {
            if (v.fieldType == FakeScriptData.FieldType.GameObject)
            {
                var go = fs.gameObjects.FirstOrDefault(x => x.name == v.name);
                if (go == null)
                {
                    go = new FakeScriptDataPairGameObject();
                    go.name = v.name;
                    fs.gameObjects.Add(go);
                }
                var old = go.go;
                go.go = EditorGUILayout.ObjectField(v.name, go.go, typeof(GameObject), true) as GameObject;
                if (old != go.go)
                {
                    EditorUtility.SetDirty(fs);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.Component)
            {
                if (!typeCache.TryGetValue(v.type, out var type))
                {
                    foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        var t = ass.GetType(v.type);
                        if (t != null)
                        {
                            type = t;
                            typeCache.Add(v.type, t);
                            break;
                        }
                    }
                }
                if (type == null) continue;
                var c = fs.components.FirstOrDefault(x => x.name == v.name);
                if (c == null)
                {
                    c = new FakeScriptDataPairComponent();
                    c.name = v.name;
                    fs.components.Add(c);
                }
                var old = c.component;
                c.component = EditorGUILayout.ObjectField(v.name, c.component, type, true) as Component;
                if (old != c.component)
                {
                    EditorUtility.SetDirty(fs);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.ExternComponent)
            {
                var f = fs.scripts.FirstOrDefault(x => x.name == v.name);
                if (f == null)
                {
                    f = new FakeScriptDataPairFakeScript();
                    f.name = v.name;
                    fs.scripts.Add(f);
                }
                var old = f.script;
                f.script = EditorGUILayout.ObjectField(v.name, f.script, typeof(FakeScript), true) as FakeScript;
                if (f.script != null)
                {
                    if (f.script.scriptData?.scriptFullName != v.type)
                    {
                        f.script = f.script.GetComponents<FakeScript>()
                            .FirstOrDefault(x => x.scriptData?.scriptFullName == v.type);
                    }
                }
                if (f.script != old)
                {
                    EditorUtility.SetDirty(fs);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.UnityObject)
            {
                if (!typeCache.TryGetValue(v.type, out var type))
                {
                    foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        var t = ass.GetType(v.type);
                        if (t != null)
                        {
                            type = t;
                            typeCache.Add(v.type, t);
                            break;
                        }
                    }
                }
                if (type == null) continue;
                var o = fs.objects.FirstOrDefault(x => x.name == v.name);
                if (o == null)
                {
                    o = new FakeScriptDataPairUnityObject();
                    o.name = v.name;
                    fs.objects.Add(o);
                }
                var old = o.obj;
                o.obj = EditorGUILayout.ObjectField(v.name, o.obj, type, true);
                if (o.obj != old)
                {
                    EditorUtility.SetDirty(fs);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.String)
            {
                var s = fs.data.FirstOrDefault(x => x.name == v.name);
                if (s == null)
                {
                    s = new FakeScriptDataPairN();
                    s.name = v.name;
                    s.data = Encoding.UTF8.GetBytes("");
                    fs.data.Add(s);
                }
                var old = Encoding.UTF8.GetString(s.data);
                var n = EditorGUILayout.TextField(v.name, old);
                if (old != n)
                {
                    EditorUtility.SetDirty(fs);
                    s.data = Encoding.UTF8.GetBytes(n);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.Int)
            {
                var i = fs.data.FirstOrDefault(x => x.name == v.name);
                if (i == null)
                {
                    i = new FakeScriptDataPairN();
                    i.name = v.name;
                    i.data = BitConverter.GetBytes((int)0);
                    fs.data.Add(i);
                }
                var old = BitConverter.ToInt32(i.data, 0);
                var n = EditorGUILayout.IntField(v.name, old);
                if (old != n)
                {
                    EditorUtility.SetDirty(fs);
                    i.data = BitConverter.GetBytes(n);
                }

            }
            else if (v.fieldType == FakeScriptData.FieldType.Float)
            {
                var f = fs.data.FirstOrDefault(x => x.name == v.name);
                if (f == null)
                {
                    f = new FakeScriptDataPairN();
                    f.name = v.name;
                    f.data = BitConverter.GetBytes((float)0);
                    fs.data.Add(f);
                }
                var old = BitConverter.ToSingle(f.data, 0);
                var n = EditorGUILayout.FloatField(v.name, old);
                if (old != n)
                {
                    EditorUtility.SetDirty(fs);
                    f.data = BitConverter.GetBytes(n);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.Bool)
            {
                var b = fs.data.FirstOrDefault(x => x.name == v.name);
                if (b == null)
                {
                    b = new FakeScriptDataPairN();
                    b.name = v.name;
                    b.data = BitConverter.GetBytes(false);
                    fs.data.Add(b);
                }
                var old = BitConverter.ToBoolean(b.data, 0);
                var n = EditorGUILayout.Toggle(v.name, old);
                if (old != n)
                {
                    EditorUtility.SetDirty(fs);
                    b.data = BitConverter.GetBytes(n);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.Quaternion)
            {
                var q = fs.quaternions.FirstOrDefault(x => x.name == v.name);
                if (q == null)
                {
                    q = new FakeScriptDataPairQuaternion();
                    q.name = v.name;
                    fs.quaternions.Add(q);
                }
                var old = q.data;
                q.data = EditorGUILayout.Vector4Field(v.name,
                    new Vector4(q.data.x, q.data.y, q.data.z, q.data.w));
                if (q.data != old)
                {
                    EditorUtility.SetDirty(fs);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.Vector2)
            {
                var vec = fs.vectors.FirstOrDefault(x => x.name == v.name);
                if (vec == null)
                {
                    vec = new FakeScriptDataPairVector();
                    vec.name = v.name;
                    fs.vectors.Add(vec);
                }
                var old = vec.data;
                vec.data = EditorGUILayout.Vector2Field(v.name, vec.data);
                if (vec.data != old)
                {
                    EditorUtility.SetDirty(fs);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.Vector3)
            {
                var vec = fs.vectors.FirstOrDefault(x => x.name == v.name);
                if (vec == null)
                {
                    vec = new FakeScriptDataPairVector();
                    vec.name = v.name;
                    fs.vectors.Add(vec);
                }
                var old = vec.data;
                vec.data = EditorGUILayout.Vector3Field(v.name, vec.data);
                if (vec.data != old)
                {
                    EditorUtility.SetDirty(fs);
                }
            }
            else if (v.fieldType == FakeScriptData.FieldType.Vector4)
            {
                var vec = fs.vectors.FirstOrDefault(x => x.name == v.name);
                if (vec == null)
                {
                    vec = new FakeScriptDataPairVector();
                    vec.name = v.name;
                    fs.vectors.Add(vec);
                }
                var old = vec.data;
                vec.data = EditorGUILayout.Vector4Field(v.name, vec.data);
                if (vec.data != old)
                {
                    EditorUtility.SetDirty(fs);
                }
            }
        }
    }
}
