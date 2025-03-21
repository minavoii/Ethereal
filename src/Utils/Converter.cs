using UnityEngine;

namespace Ethereal.PluginUtils;

internal class Converter
{
    internal static T WithinGameObject<T>(T instance, string name = "")
        where T : Component
    {
        return IntoGameObject(instance, name).GetComponent<T>();
    }

    internal static GameObject IntoGameObject<T>(T component, string name = "")
        where T : Component
    {
        GameObject gameObject = new() { name = name };
        CopyToGameObject(ref gameObject, component);

        return gameObject;
    }

    internal static void CopyToGameObject<T>(ref GameObject gameObject, T component)
        where T : Component
    {
        var type = typeof(T);
        T innerComponent = gameObject.AddComponent<T>();

        foreach (var property in type.GetProperties())
        {
            if (property.CanWrite)
            {
                try
                {
                    property.SetValue(innerComponent, property.GetValue(component));
                }
                catch { }
            }
        }

        foreach (var field in type.GetFields())
        {
            if (field.IsPublic)
            {
                try
                {
                    field.SetValue(innerComponent, field.GetValue(component));
                }
                catch { }
            }
        }
    }
}
