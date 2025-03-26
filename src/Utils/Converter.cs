using System;
using UnityEngine;

namespace Ethereal.Utils;

internal static class Converter
{
    internal static Component WithinGameObject(Component instance, string name = "")
    {
        return IntoGameObject(instance, name).GetComponent<Component>();
    }

    internal static GameObject IntoGameObject(Component component, string name = "")
    {
        GameObject gameObject = new() { name = name };
        CopyToGameObject(ref gameObject, component);

        return gameObject;
    }

    internal static void CopyToGameObject(ref GameObject gameObject, Component component)
    {
        Type type = component.GetType();
        Component innerComponent = gameObject.AddComponent(type);

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
