using com.armatur.common.save;
using com.armatur.common.serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace com.armatur.common.util
{
    public interface IWeighted
    {
        float Weight { get; }
    }

    public abstract class KeyValueObj<T, U>
    {
        [SerializeAsAttribute("Key")]
        public T key;

        [SerializeAsAttribute("Value")]
        public U value;
    }

    public interface IKeyValueItem<K, V>
    {
        K First { get; set; }
        V Second { get; set; }
    }

    public class Weighted : IWeighted
    {
        [SerializeAsAttribute("Probability")]
        private readonly float probability = 0;

        public float Weight => probability;

        public Weighted() { }

        public Weighted(float value)
        {
            probability = value;
        }
    }

    public class IndexedWeight : IWeighted
    {
        public int Index { get; } = 0;
        public float Weight { get; } = 0;

        public IndexedWeight(int index, float weight)
        {
            Index = index;
            Weight = weight;
        }
    }

    public interface IIndexedItem
    {
        int Id { get; }
    }

    public static class MiscUtil
    {
        public static T LoadConfig<T>(string configPath, bool skipSavables = false) where T: class
        {
            var xmltext = File.ReadAllText(configPath);

            if (!string.IsNullOrEmpty(xmltext))
            {
                var saveProc = new SaveProcessorXml(xmltext,
                    new SaveLoadParams() {
                        skipSavables = skipSavables
                    });

                T obj = saveProc.LoadObject<T>();

                return obj;
            }

            return default(T);
        }

        public static T LoadConfig<T>(T destinationObj, string configPath, bool skipSavables = false) where T : class
        {
            var xmltext = File.ReadAllText(configPath);

            if (!string.IsNullOrEmpty(xmltext))
            {
                var saveProc = new SaveProcessorXml(xmltext,
                    new SaveLoadParams()
                    {
                        skipSavables = skipSavables
                    });

                T obj = saveProc.LoadObject<T>(destinationObj);
                return obj;
            }

            return default(T);
        }

        public static T TryLoadSavedState<T>(string saveFolderPath, string fileName = "game_save.xml", bool savableOnly = false) where T : class
        {
            string savedStatePath = string.Format("{0}/{1}", saveFolderPath, fileName);

            if (!File.Exists(savedStatePath))
            {
                return default(T);
            }

            string xmltext = File.ReadAllText(savedStatePath);


            if (!string.IsNullOrEmpty(xmltext))
            {
                var saveProcessorXml = new SaveProcessorXml(xmltext, new SaveLoadParams()
                {
                    savableOnly = savableOnly
                });

                T obj = saveProcessorXml.LoadObject<T>(default(T));
                return obj;
            }

            return default(T);
        }


        public static bool TryLoadSavedState<T>(T obj, string saveFolderPath, string fileName = "game_save.xml", bool savableOnly = false) where T : class
        {
            string savedStatePath = string.Format("{0}/{1}", saveFolderPath, fileName);

            if (!File.Exists(savedStatePath))
            {
                return false;
            }


            var xmltext = File.ReadAllText(savedStatePath);

            if (!string.IsNullOrEmpty(xmltext))
            {
                var saveProcessorXml = new SaveProcessorXml(xmltext, new SaveLoadParams()
                {
                    savableOnly = savableOnly
                });

                saveProcessorXml.LoadObject<T>(obj);
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool StateIsSaved(string saveFolderPath, string fileName = "game_save.xml")
        {
            string savedStatePath = string.Format("{0}/{1}", saveFolderPath, fileName);

            return File.Exists(savedStatePath);
        }

        public static void DeleteSavedState(string saveFolderPath, string fileName = "game_save.xml")
        {
            string savedStatePath = string.Format("{0}/{1}", saveFolderPath, fileName);

            if (File.Exists(savedStatePath))
            {
                File.Delete(savedStatePath);
            }
        }

        public static void SaveState(object obj, string saveFolderPath, string fileName = "game_save.xml", bool savableOnly = false)
        {
            var saveProcessorXml = new SaveProcessorXml(new SaveLoadParams()
            {
                savableOnly = savableOnly
            });

            saveProcessorXml.SaveObject(obj);

            string folderPath = string.Format("{0}", saveFolderPath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = string.Format("{0}/{1}", folderPath, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);

            File.WriteAllText(filePath, saveProcessorXml.XDoc.ToString());
        }



        public static T MapObject<T>(T source, T destination, bool skipSavable = false, bool savablesOnly = false, bool debug = false)
        {
            // fields
            List<FieldInfo> fieldsInfo = source.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(member => member.GetCustomAttribute(typeof(SerializeDataAttribute), true) != null)
                .ToList();
            
            // skip simple savable types
            // complex types (collections) still are going through mapping
            if (skipSavable)
            {
                fieldsInfo.ToArray().ForEach((i) =>
                {
                    SavableAttribute sAttr = i.GetCustomAttribute(typeof(SavableAttribute)) as SavableAttribute;
                    if (sAttr != null)
                    {
                        if (sAttr.isComplex == ComplexType.False)
                        {
                            fieldsInfo.Remove(i);
                        }
                    }
                });
            }

            // savables only
            if (savablesOnly)
            {
                fieldsInfo = fieldsInfo.Where(i => ((SavableAttribute)i.GetCustomAttribute(typeof(SavableAttribute))) != null).ToList();
            }

            //if (fieldsInfo.Count() == 0) UberDebug.Log("fields not found");

            // iterate all fields
            foreach (var member in fieldsInfo)
            {
                object sourceValue = member.GetValue(source);
                if (sourceValue == null) continue;

                if (sourceValue.GetType().IsCollectionType())
                {
                    IList sourceCollection = (IList)sourceValue;
                    IList destCollection = (IList)destination.GetType()
                        .GetField(member.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .GetValue(destination);

                    int destCollectionStartSize = destCollection.Count;
                    foreach (var sourceItem in sourceCollection)
                    {
                        if (sourceItem.GetType().IsValueType)
                        {
                            destCollection?.Add(sourceItem);
                        }
                        else if (sourceCollection.Count > 0 && destCollectionStartSize > 0)
                        {
                            int? sourceItem_id = (sourceItem as IIndexedItem)?.Id;
                            if (sourceItem_id != null)
                            {
                                object targItem = null;
                                foreach (var destItem in destCollection)
                                {
                                    int? destItem_id = (destItem as IIndexedItem)?.Id;
                                    if (destItem_id != null)
                                    {
                                        if (destItem_id.Value == sourceItem_id.Value)
                                        {
                                            targItem = destItem;
                                            break;
                                        }
                                    }
                                }

                                if (targItem != null)
                                    targItem = MapObject(sourceItem, targItem, skipSavable, savablesOnly, debug);

                            }
                        }
                        else if (sourceCollection.Count > 0 && destCollectionStartSize == 0) //&& ((sourceItem as ICloneable) != null)
                        {
                            Type itemType = sourceItem.GetType();

                            object newItem;
                            if (itemType == typeof(string))
                                newItem = string.Empty;
                            else
                                newItem = Activator.CreateInstance(itemType);

                            if (newItem != null)
                                newItem = MapObject(sourceItem, newItem, skipSavable, savablesOnly, debug);

                            destCollection.Add(newItem);
                        }
                    }
                }
                else if (!sourceValue.GetType().IsBasicType())
                {
                    //throw new NotImplementedException("class type mapping not implemented" + member.Name);
                    object destValue = destination.GetType()
                        .GetField(member.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .GetValue(destination);

                    if (destValue == null)
                    {
                        var cloneable = sourceValue as ICloneable;
                        if (cloneable != null)
                        {
                            destValue = cloneable.Clone();
                        }
                        else
                        {
                            destValue = Activator.CreateInstance(sourceValue.GetType());
                        }
                    }

                    destValue = MapObject(Convert.ChangeType(sourceValue, sourceValue.GetType()), Convert.ChangeType(destValue, destValue.GetType()), skipSavable, savablesOnly, debug);
                    destination.GetType()
                        .GetField(member.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .SetValue(destination, destValue);
                }
                else
                {
                    destination
                        .GetType()
                        .GetField(member.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .SetValue(destination, sourceValue);
                }
            }


            // props
            List<PropertyInfo> propsInfo = source.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(member => member.GetCustomAttribute(typeof(SerializeDataAttribute), true) != null)
                .ToList();

            // skip simple savable types
            // complex types (collections) still are going through mapping
            if (skipSavable)
            {
                propsInfo.ToArray().ForEach((i) =>
                {
                    SavableAttribute sAttr = i.GetCustomAttribute(typeof(SavableAttribute)) as SavableAttribute;
                    if (sAttr != null)
                    {
                        if (sAttr.isComplex == ComplexType.False)
                        {
                            propsInfo.Remove(i);
                        }
                    }
                });
            }

            // savables only
            if (savablesOnly)
            {
                fieldsInfo = fieldsInfo.Where(i => ((SavableAttribute)i.GetCustomAttribute(typeof(SavableAttribute))) != null).ToList();
            }

            //if (propsInfo.Count() == 0) UberDebug.Log("properties not found");

            // iterate all properties
            foreach (var member in propsInfo)
            {
                object sourceValue = member.GetValue(source);
                if (sourceValue == null) continue;

                if (sourceValue.GetType().IsCollectionType())
                {
                    IList sourceCollection = (IList)sourceValue;
                    IList destCollection = (IList)destination.GetType()
                        .GetProperty(member.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .GetValue(destination);

                    int destCollectionStartSize = destCollection.Count;
                    foreach (var sourceItem in sourceCollection)
                    {
                        if (sourceItem.GetType().IsValueType)
                        {
                            destCollection?.Add(sourceItem);
                        }
                        else if (sourceCollection.Count > 0 && destCollectionStartSize > 0)
                        {


                            int? sourceItem_id = (sourceItem as IIndexedItem)?.Id;
                            if (sourceItem_id != null)
                            {
                                object targItem = null;
                                foreach (var destItem in destCollection)
                                {
                                    int? destItem_id = (destItem as IIndexedItem)?.Id;
                                    if (destItem_id != null)
                                    {
                                        if (destItem_id.Value == sourceItem_id.Value)
                                        {
                                            targItem = destItem;
                                            break;
                                        }
                                    }
                                }

                                if (targItem != null)
                                    targItem = MapObject(sourceItem, targItem, skipSavable, savablesOnly, debug);
                            }
                        }
                        else if (sourceCollection.Count > 0 && destCollectionStartSize == 0) // && ((sourceItem as ICloneable) != null)
                        {
                            Type itemType = sourceItem.GetType();

                            object newItem;
                            if (itemType == typeof(string))
                                newItem = string.Empty;
                            else
                                newItem = Activator.CreateInstance(itemType);

                            if (newItem != null)
                                newItem = MapObject(sourceItem, newItem, skipSavable, savablesOnly, debug);

                            destCollection.Add(newItem);
                        }
                    }
                }
                else if (!sourceValue.GetType().IsBasicType())
                {
                    //throw new NotImplementedException("class type mapping not implemented: " + member.Name);
                    object destValue = destination.GetType()
                        .GetProperty(member.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .GetValue(destination);

                    if (destValue == null)
                    {
                        var cloneable = sourceValue as ICloneable;
                        if (cloneable != null)
                        {
                            destValue = cloneable.Clone();
                        }
                        else
                        {
                            destValue = Activator.CreateInstance(sourceValue.GetType());
                        }
                    }

                    destValue = MapObject(Convert.ChangeType(sourceValue, sourceValue.GetType()), Convert.ChangeType(destValue, destValue.GetType()), skipSavable, savablesOnly, debug);
                    destination.GetType()
                        .GetProperty(member.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .SetValue(destination, destValue);
                }
                else
                {
                    destination
                        .GetType()
                        .GetProperty(member.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .SetValue(destination, sourceValue);
                }
            }

            return destination;
        }

        public static Color SetAlpha(this Color self, float a)
        {
            return new Color(self.r, self.g, self.b, a);
        }

        public static float NextFloat(this System.Random random, float min, float max)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }
    }

    public static class StringExt
    {
        public static string Random(this string str, int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            

            for (int i = 0; i < stringChars.Length; i++)
            {
                var random = new System.Random(Guid.NewGuid().GetHashCode());
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}