namespace Ethereal.Classes.LazyValues;

public abstract record LazyReferenceable<T> : LazyValue<T>
    where T : Referenceable
{
    public LazyReferenceable(int id)
        : base(id) { }

    public LazyReferenceable(string name)
        : base(name) { }

    public LazyReferenceable(T data)
        // If data is not inside a GameObject, the `UnityEngine.Object` equality check
        // will return null despite the data not actually being null
        : base(data.Equals(null) ? null : data)
    {
        // If the component is not in a GameObject, we store its ID, otherwise we store the component itself
        if (data == null)
            Id = data?.ID;
    }
}
