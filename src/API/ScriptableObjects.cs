using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Ethereal.API;

public static class ScriptableObjects
{
    /// <summary>
    /// Copy an object to the inside of a ScriptableObject, and return the ScriptableObject.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T IntoScriptableObject<T, U>(U @object)
        where T : ScriptableObject
    {
        T scriptableObject = ScriptableObject.CreateInstance<T>();
        CopyToScriptableObject(ref scriptableObject, @object);

        return scriptableObject;
    }

    /// <summary>
    /// Copy an object's properties into a ScriptableObject's properties and fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="scriptableObject"></param>
    /// <param name="object"></param>
    public static void CopyToScriptableObject<T, U>(ref T scriptableObject, U @object)
        where T : ScriptableObject
    {
        if (@object is null)
            return;

        Type typeSource = @object.GetType();
        Type typeDest = scriptableObject.GetType();

        List<(PropertyInfo, (PropertyInfo, FieldInfo))> properties =
        [
            .. typeSource
                .GetProperties()
                .Select(x =>
                    (
                        source: x,
                        dest: (
                            property: typeDest.GetProperty(x.Name),
                            field: typeDest.GetField(x.Name)
                        )
                    )
                )
                .Where(x => x.dest.field?.IsPublic ?? x.dest.property?.CanWrite ?? false),
        ];

        foreach (
            (
                PropertyInfo propertySource,
                (PropertyInfo propertyDest, FieldInfo fieldDest)
            ) in properties
        )
        {
            if (propertyDest is not null)
            {
                try
                {
                    propertyDest.SetValue(scriptableObject, propertySource.GetValue(@object));
                }
                catch { }
            }
            else if (fieldDest is not null)
            {
                try
                {
                    fieldDest.SetValue(scriptableObject, propertySource.GetValue(@object));
                }
                catch { }
            }
        }
    }
}
