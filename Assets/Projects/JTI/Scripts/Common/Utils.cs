using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace JTI.Scripts.Common
{
    public static class Utils
    {
#if UNITY_EDITOR
        public static T GetTypeInProject<T>() where T : ScriptableObject
        {
            var guids2 = AssetDatabase.FindAssets($"t:{typeof(T)}");

            if (guids2.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids2[0]);
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return default;
        }
        public static List<T> GetTypesInProject<T>() where T : ScriptableObject
        {
            var guids2 = AssetDatabase.FindAssets($"t:{typeof(T)}");

            var l = new List<T>();

            if (guids2.Length > 0)
            {
                foreach (var s in guids2)
                {
                    var path = AssetDatabase.GUIDToAssetPath(s);
                    l.Add(AssetDatabase.LoadAssetAtPath<T>(path));
                }

            }

            return l;
        }
#endif
        public static bool CheckStringForOnlyAlphabetSymbolsAndNumbers(string str, int maxLength = int.MaxValue)
        {
            var strAv = "1234567890_qwertyuiopasdfghjklzxcvbnm";

            if (str.Length > maxLength) return false;

            foreach (var s in str.ToLower())
            {
                if (!strAv.Contains(s))
                {
                    return false;
                }
            }

            return true;
        }
        public static bool CheckStringForOnlyNumbers(string str, int maxLength = int.MaxValue)
        {
            var strAv = "1234567890";

            if (str.Length > maxLength) return false;

            foreach (var s in str.ToLower())
            {
                if (!strAv.Contains(s))
                {
                    return false;
                }
            }

            return true;
        }
        public enum MoveType
        {
            OnlyX, OnlyY, Free
        }

        public enum RotateType
        {
            OnlyX, OnlyY, Free
        }

        public class PercentDrop<T>
        {
            public float Percent;
            public T Object;
        }
        public enum MoveDirection
        {
            Left,
            Right,
            Down,
            Top
        }

        public enum UpdateType
        {
            FixedUpdate,
            LateUpdate,
            Update,
        }


        public static T GetDrop<T>(List<PercentDrop<T>> drops)
        {
            var totalChance = drops.Sum(x => x.Percent);
            var slotsChances = new Dictionary<PercentDrop<T>, float>();

            for (var index = 0; index < drops.Count; index++)
            {
                var slot = drops[index];
                var percent = slot.Percent;
                var realChance = (percent / totalChance);

                slotsChances.Add(slot, realChance);
            }


            var sorted = slotsChances.OrderBy(x => x.Key.Percent).ToList();

            var chance = UnityEngine.Random.Range(0f, totalChance);


            var count = 0f;
            foreach (var item in sorted)
            {
                if (chance < count + item.Value)
                {
                    var sameChance = sorted.Where(x => x.Value == chance).ToList();
                    if (sameChance.Count > 0)
                    {
                        var random = UnityEngine.Random.Range(0, sorted.Count);

                        return sorted[random].Key.Object;
                    }

                    return item.Key.Object;
                }

                count += item.Value;
            }

            return sorted.RandomElement().Key.Object;

        }

        public static List<GameObject> RayCastAllFrom(Vector3 place, Vector3 pos, int layer)
        {
            var hits = Physics.RaycastAll(place, (pos - place).normalized, Vector3.Distance(place, pos), layer);
            if (hits.Length > 0)
            {
                var a = new List<GameObject>();

                for (var index = 0; index < hits.Length; index++)
                {
                    var raycastHit = hits[index];
                    a.Add(raycastHit.collider.gameObject);
                }

                return a;
            }

            return new List<GameObject>();
        }
        public static List<GameObject> RayCastAllFromCamera(Camera camera, Vector3 pos, int layer)
        {
            var ray = camera.ScreenPointToRay(pos);

            var hits = Physics.RaycastAll(ray, 1000, layer);
            if (hits.Length > 0)
            {
                var a = new List<GameObject>();

                for (var index = 0; index < hits.Length; index++)
                {
                    var raycastHit = hits[index];
                    a.Add(raycastHit.collider.gameObject);
                }

                return a;
            }

            return new List<GameObject>();
        }


        public static GameObject RayCastFromCamera(Camera camera, Vector3 pos, int layer)
        {
            var ray = camera.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out var hit, 1000, layer))
            {
                return hit.collider.gameObject;
            }

            return null;
        }
        public static float ClampAngle(float angle, float min, float max)
        {
            return 0;
            /* angle = NormalizeAngle(angle);
             if (angle > 180)
             {
                 angle -= 360;
             }
             else if (angle < -180)
             {
                 angle += 360;
             }
 
             min = NormalizeAngle(min);
             if (min > 180)
             {
                 min -= 360;
             }
             else if (min < -180)
             {
                 min += 360;
             }
 
             max = NormalizeAngle(max);
             if (max > 180)
             {
                 max -= 360;
             }
             else if (max < -180)
             {
                 max += 360;
             }
 
             // Aim is, convert angles to -180 until 180.
             return Mathf.Clamp(angle, min, max);*/
        }


        public static GameObject RayCastFromCamera(Camera camera, Vector3 pos)
        {
            var ray = camera.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out var hit, 1000))
            {
                return hit.collider.gameObject;
            }

            return null;

        }

        public static GameObject RayCastFromCamera(Camera camera, Vector3 pos, out RaycastHit hitPoint)
        {
            var ray = camera.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out var hit, 1000))
            {
                hitPoint = hit;
                return hit.collider.gameObject;
            }

            hitPoint = new RaycastHit();
            return null;

        }
        public static bool RayCastFromCamera(Camera camera, Vector3 pos, out Vector3 returnPos, int layer)
        {
            var ray = camera.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out var hit, 1000, layer))
            {
                returnPos = hit.point;
                return true;
            }

            returnPos = Vector3.zero;

            return false;

        }
   
        public static bool HasSybmols(string a)
        {
#if UNITY_EDITOR
            var droid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(';').ToList();
            var ios = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';').ToList();

            return droid.Contains(a) && ios.Contains(a);
#endif
            return false;
        }

        public static void AddSybmols(string a)
        {
#if UNITY_EDITOR
            var droid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(';').ToList();
            var ios = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';').ToList();

            if (!droid.Contains(a))
            {
                droid.Add(a);

                var finalAndroid = "";
                for (var index = 0; index < droid.Count; index++)
                {
                    var io = droid[index];
                    finalAndroid += io + (index < droid.Count - 1 ? ";" : "");
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, finalAndroid);
            }

            if (!ios.Contains(a))
            {
                ios.Add(a);

                var finalIos = "";
                for (var index = 0; index < ios.Count; index++)
                {
                    var io = ios[index];
                    finalIos += io + (index < ios.Count - 1 ? ";" : "");
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, finalIos);
            }
#endif

        }

        public static void RemoveSymbol(string a)
        {
#if UNITY_EDITOR
            var droid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(';').ToList();
            var ios = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';').ToList();

            if (droid.Contains(a))
            {
                droid.Remove(a);

                var finalAndroid = "";
                for (var index = 0; index < droid.Count; index++)
                {
                    var io = droid[index];
                    finalAndroid += io + (index < droid.Count - 1 ? ";" : "");
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, finalAndroid);
            }

            if (ios.Contains(a))
            {
                ios.Remove(a);

                var finalIos = "";
                for (var index = 0; index < ios.Count; index++)
                {
                    var io = ios[index];
                    finalIos += io + (index < ios.Count - 1 ? ";" : "");
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, finalIos);
            }
#endif
        }
        public static T[] ToArraySaveReferences<T>(this IList<T> list)
        {
            var array = new T[list.Count];
            var count = array.Length;
            for (var index = 0; index < count; index++)
            {
                array[index] = list[index];
            }

            return array;
        }

        public static void ShuffleList<T>(this IList<T> list)
        {
            var rng = new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }


        public static T RandomElement<T>(this IList<T> array)
        {
            return array[UnityEngine.Random.Range(0, array.Count)];
        }

        public static T DefaultRandomElement<T>(this IList<T> array, T val)
        {
            if (array == null || array.Count == 0)
                return val;

            return array[UnityEngine.Random.Range(0, array.Count)];
        }

        public static List<string> GetAllFilesInInnerDictionary(string path)
        {
            if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                var fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                return fileInfos.ToList().ConvertAll(x => Path.GetFileNameWithoutExtension($"{path}/{x.Name}"));
            }
            return new List<string>();
        }


        public static void ClearChildObjects(this Transform transform)
        {
            foreach (Transform child in transform)
                Object.Destroy(child.gameObject);
        }
        public static void SetLayerToAll(this GameObject obj, Enum layer)
        {
            var c = obj.GetComponentsInChildren<Transform>(true);
            foreach (var transform in c)
            {
                transform.gameObject.layer = Convert.ToInt32(layer);
            }
        }


        public static void SetLayerToAll(this GameObject obj, int layer)
        {
            var c = obj.GetComponentsInChildren<Transform>(true);
            foreach (var transform in c)
            {
                transform.gameObject.layer = layer;
            }
        }


        public static void ClearChildObjects(this Transform transform, List<Transform> exception)
        {
            if (exception == null)
                exception = new List<Transform>();

            foreach (Transform child in transform)
            {
                if (!exception.Contains(child))
                    Object.Destroy(child.gameObject);
            }
        }


        public static void ClearChildObjectsNow(this Transform transform)
        {
            var l = new List<GameObject>();

            foreach (Transform child in transform)
            {
                l.Add(child.gameObject);
            }

            foreach (var obj in l)
            {
                Object.DestroyImmediate(obj);
            }

        }
        public static double RoundToHalf(double val, int n = 2)
        {
            var res = val / 0.5f;
            var round = Math.Round(res);

            return round * 0.5f;
        }

        public static T FromJson<T>(this string str)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject<T>(str, settings);
        }

        public static string ToJson<T>(this T obj, Formatting formatting = Formatting.None)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public static T Copy<T>(this object a)
        {
            return ToJson(a).FromJson<T>();
        }


        public static T SpawnPrefabOfInstance<T>(T ob) where T : MonoBehaviour
        {
            Object obj = null;
#if  UNITY_EDITOR
            if (!Application.isPlaying)
            {
                obj = PrefabUtility.InstantiatePrefab(ob);
            }
#endif

            if (obj == null)
            {
                obj = Object.Instantiate(ob);
            }

            return obj as T;
        }
        public static Quaternion RotateZReturn(Transform t, Vector3 pos)
        {
            var difference = pos - t.transform.position;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0.0f, 0.0f, rotationZ - 90);
        }

        public static Quaternion RotateZ(Vector3 from, Vector3 pos)
        {
            var difference = pos - from;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0.0f, 0.0f, rotationZ - 90);
        }

        public static Vector3 RotateZVec(Vector3 from, Vector3 pos)
        {
            var difference = pos - from;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0.0f, 0.0f, rotationZ - 90).eulerAngles;
        }

        public static void RotateZ(Transform t, Vector3 pos)
        {
            var difference = pos - t.transform.position;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            t.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ + 0);
        }
        public static void RotateReverseZ(Transform t, Vector3 pos)
        {
            var difference = t.transform.position - pos;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            t.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ + 0);
        }
        public static void RotateYExtraOffset(Transform t, Vector3 pos, float offset)
        {
            var difference = pos - t.transform.position;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            t.transform.rotation = Quaternion.Euler(0.0f, rotationZ + offset, 0f);
        }

        public static void RotateZExtraOffset(Transform t, Vector3 pos, float offset)
        {
            var difference = pos - t.transform.position;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            t.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ + offset);
        }
        public static void RotateZExtraOffset(Transform t, Vector3 pos, float offset, float time)
        {
            var difference = pos - t.transform.position;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            t.transform.rotation = Quaternion.Lerp(t.transform.rotation, Quaternion.Euler(0.0f, 0.0f, rotationZ + offset), time);
        }
        public static void RotateReverseZ(Transform t, Vector3 pos, float percent)
        {
            var difference = t.transform.position - pos;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            t.transform.rotation = Quaternion.Lerp(t.rotation, Quaternion.Euler(0.0f, 0.0f, rotationZ - 90), percent);
        }
        public static void RotateZ(Transform t, Vector3 pos, float percent)
        {
            var difference = pos - t.transform.position;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            t.transform.rotation = Quaternion.Lerp(t.rotation, Quaternion.Euler(0.0f, 0.0f, rotationZ - 90), percent);
        }
        public static Quaternion RotateZReturn(Transform t, Vector3 pos, float percent)
        {
            var difference = pos - t.transform.position;
            var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            return Quaternion.Lerp(t.rotation, Quaternion.Euler(0.0f, 0.0f, rotationZ - 90), percent);
        }
    }
}
