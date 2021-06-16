using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using SpaceTarget.Runtime;

namespace SpaceTarget.EditorClasses
{
    [CustomEditor(typeof(SpaceTargetBehaviour))]
    [CanEditMultipleObjects]
    public class SpaceTargetBehaviourInspector : Editor
    {
        #region Quick create menuItems

        [MenuItem("GameObject/4DAGE-SpaceTarget/Space Target", false, 1)]
        public static void CreateSpaceTarget()
        {
            GameObject go = new GameObject { name = "SpaceTarget" };
            go.AddComponent<SpaceTargetBehaviour>();
            go.AddComponent<DefaultTrackableEventHandler>();
        }
        #endregion

        #region OnInspectorGUI
        private bool showAdvanced = false;
        public override void OnInspectorGUI()
        {
            SpaceTargetBehaviour spaceTarget = (SpaceTargetBehaviour)target;
            base.OnInspectorGUI();
            serializedObject.Update();

            //world center mode setting
            if (spaceTarget.worldCenterMode == WorldCenterMode.TARGET)
            {
                var prop = serializedObject.FindProperty("m_ARCameraRoot");
                EditorGUILayout.PropertyField(prop, true);
            }

            spaceTarget.databaseList = GetDatabaseList();
            int curIndex = spaceTarget.index;
            spaceTarget.index= EditorGUILayout.Popup("Database", spaceTarget.index, spaceTarget.databaseList.ToArray());
            if (curIndex != spaceTarget.index) 
            {
                spaceTarget.databaseID = spaceTarget.databaseList[spaceTarget.index];
                //destory last database prefab
                if (spaceTarget.databasePrefab != null)
                    DestroyImmediate(spaceTarget.databasePrefab);
                //destory last occlusion prefab
                if (spaceTarget.occlusionPrefab != null) 
                    DestroyImmediate(spaceTarget.occlusionPrefab);
                
                //reset advance setting
                spaceTarget.visiblePrefab = true;
                spaceTarget.addOcclusion = false;
                spaceTarget.showOutline = false;
                spaceTarget.addTransparent = false;

                spaceTarget.databasePrefab = InstantiateDatabase(spaceTarget.databaseID, spaceTarget.transform,(go)=> 
                {
                    //Instantiate preview model for editor
                    go.name = "--Preview Model--";
                    go.hideFlags = HideFlags.HideInHierarchy;
                    for (int i = 0; i < go.transform.childCount; i++)
                    {
                        go.transform.GetChild(i).gameObject.hideFlags = HideFlags.HideInHierarchy;
                    }
                });
            }
            //show advance setting
            if (spaceTarget.databasePrefab != null)
            {
                showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced", true);
                if (showAdvanced)
                {
                    spaceTarget.visiblePrefab = EditorGUILayout.Toggle("Visible Database", spaceTarget.visiblePrefab);
                    spaceTarget.addOcclusion = EditorGUILayout.Toggle("Add Occlusion", spaceTarget.addOcclusion);
                    
                    //编辑器显示模型
                    if (spaceTarget.visiblePrefab)
                    {
                        spaceTarget.databasePrefab.SetActive(true);

                        //是否透明
                        spaceTarget.addTransparent = EditorGUILayout.Toggle("Transparent Database", spaceTarget.addTransparent);
                        if (spaceTarget.addTransparent) 
                        {
                            if (!spaceTarget.isTransparented)
                            {
                                var meshs = spaceTarget.databasePrefab.GetComponentsInChildren<MeshRenderer>();
                                for (int i = 0; i < meshs.Length; i++)
                                {
                                    Material[] tMats = meshs[i].sharedMaterials;
                                    Material[] tempMats = new Material[tMats.Length];
                                    for (int j = 0; j < tMats.Length; j++)
                                    {
                                        Material mat = new Material(tMats[j]);
                                        mat.SetColor("_Color", new Color(1, 1, 1, 100f / 255f));
                                        StandardShaderUtils.ChangeRenderMode(mat, StandardShaderUtils.BlendMode.Transparent);
                                        tempMats[j] = mat;
                                    }
                                    meshs[i].sharedMaterials = tempMats;
                                }
                                spaceTarget.isTransparented = true;
                            }
                            
                        }
                        else
                        {
                            if (spaceTarget.isTransparented)
                            {
                                var meshs = spaceTarget.databasePrefab.GetComponentsInChildren<MeshRenderer>();
                                for (int i = 0; i < meshs.Length; i++)
                                {
                                    Material[] tMats = meshs[i].sharedMaterials;
                                    for (int j = 0; j < tMats.Length; j++)
                                    {
                                        StandardShaderUtils.ChangeRenderMode(tMats[j], StandardShaderUtils.BlendMode.Opaque);
                                    }
                                    meshs[i].sharedMaterials = tMats;
                                }
                                spaceTarget.isTransparented = false;
                            }
                        }
                    }
                    else
                    {
                        spaceTarget.databasePrefab.SetActive(false);
                    }
                    //显示遮罩模型
                    if (spaceTarget.addOcclusion) 
                    {
                        spaceTarget.showOutline = EditorGUILayout.Toggle("Show Outline", spaceTarget.showOutline);
                        if (spaceTarget.occlusionPrefab == null)
                            spaceTarget.occlusionPrefab = InstantiateDatabase(spaceTarget.databaseID, spaceTarget.transform, (go)=> 
                            {
                                //Instantiate Occlusion model
                                go.name = "--Occlusion Model--";
                                go.hideFlags = HideFlags.HideInHierarchy;
                                for (int i = 0; i < go.transform.childCount; i++)
                                {
                                    go.transform.GetChild(i).gameObject.hideFlags = HideFlags.HideInHierarchy;
                                }
                                var meshs = go.GetComponentsInChildren<Renderer>();
                                for (int i = 0; i < meshs.Length; i++)
                                {
                                    Material[] mats = meshs[i].sharedMaterials;
                                    for(int j = 0; j < mats.Length; j++)
                                    {
                                        var maskMat = new Material(Shader.Find("SpaceTarget/DepthMask"));
                                        mats[j] = maskMat;
                                    }
                                    meshs[i].materials = mats;
                                }
                            });
                        if (spaceTarget.showOutline)
                        {
                            if (spaceTarget.occlusionPrefab != null&& spaceTarget.occlusionPrefab.GetComponentInChildren<Renderer>().sharedMaterial.name!= "SpaceTarget/DepthContour")
                            {
                                var meshs= spaceTarget.occlusionPrefab.GetComponentsInChildren<Renderer>();
                                for (int i = 0; i < meshs.Length; i++)
                                {
                                    Material[] mats = meshs[i].sharedMaterials;
                                    for(int j = 0; j < mats.Length; j++)
                                    {
                                        mats[j] = new Material(Shader.Find("SpaceTarget/DepthContour"));
                                    }
                                    meshs[i].materials = mats;
                                }
                            }
                        }
                        else
                        {
                            if (spaceTarget.occlusionPrefab != null && spaceTarget.occlusionPrefab.GetComponentInChildren<Renderer>().sharedMaterial.name != "SpaceTarget/DepthMask") 
                            {
                                var meshs = spaceTarget.occlusionPrefab.GetComponentsInChildren<Renderer>();
                                for (int i = 0; i < meshs.Length; i++)
                                {
                                    Material[] mats = meshs[i].sharedMaterials;
                                    for (int j = 0; j < mats.Length; j++)
                                    {
                                        mats[j] = new Material(Shader.Find("SpaceTarget/DepthMask"));
                                    }
                                    meshs[i].materials = mats;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (spaceTarget.occlusionPrefab != null)
                            DestroyImmediate(spaceTarget.occlusionPrefab);
                    }
                }
            }
            

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Add Database"))
            {
                SpaceTargetEditorUtility.DatabaseLoader();
            }

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(spaceTarget);
        }

        

        private DirectoryInfo dataDirInfo;
        private List<string> GetDatabaseList()
        {
            List<string> tempList = new List<string>();
            tempList.Add("---Empty---");
            if (dataDirInfo == null)
            {
                string dataPath = Path.Combine(Application.streamingAssetsPath, "SpaceTargetAssets", "Database");
                dataDirInfo = new DirectoryInfo(dataPath);
            }
            if (dataDirInfo.Exists)
            {
                for(int i = 0; i < dataDirInfo.GetDirectories().Length; i++)
                {
                    tempList.Add(dataDirInfo.GetDirectories()[i].Name);
                }
            }
            return tempList;
        }

        private GameObject InstantiateDatabase(string ID, Transform parent, System.Action<GameObject> callback = null)
        {
            string obj = Path.Combine("Assets", "Editor", "SpaceTargetAssets", "Database", ID, "sfm_ar.obj");
            string objFullPath = Path.Combine(System.Environment.CurrentDirectory, obj);
            FileInfo fi = new FileInfo(objFullPath);
            if (fi.Exists)
            {
                var go = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(obj), parent);
                callback?.Invoke(go);
                return go;
            }
            else
            {
                if (ID != "---Empty---")
                    Debug.LogErrorFormat("Database:{0},does not exists", ID);
                return null;
            }
        }

        #endregion
    }

    public static class StandardShaderUtils
    {
        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 2450;
                    break;
                case BlendMode.Fade:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
                case BlendMode.Transparent:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
            }

        }
    }
}