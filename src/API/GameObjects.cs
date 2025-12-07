using System;
using UnityEngine;

namespace Ethereal.API;

public static class GameObjects
{
    /// <summary>
    /// Copy a component to the inside of a GameObject, and return the component.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T WithinGameObject<T>(T instance, string name = "")
        where T : Component
    {
        return IntoGameObject(instance, name).GetComponent<T>();
    }

    /// <summary>
    /// Copy a component to the inside of a GameObject, and return the GameObject.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject IntoGameObject(Component component, string name = "")
    {
        GameObject gameObject = new(name);
        CopyToGameObject(ref gameObject, component);

        return gameObject;
    }

    /// <summary>
    /// Copy a component to the inside of a GameObject.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="component"></param>
    public static void CopyToGameObject(ref GameObject gameObject, Component component)
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
