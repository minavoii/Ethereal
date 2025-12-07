using Ethereal.API;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a ScriptableObject from its properties.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract record ScriptableObjectBuilder<T>
    where T : ScriptableObject
{
    public T Build() => ScriptableObjects.IntoScriptableObject<T, ScriptableObjectBuilder<T>>(this);
}
