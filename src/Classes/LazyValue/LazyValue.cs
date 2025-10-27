namespace Ethereal.Classes.LazyValues;

public abstract record LazyValue<T>
{
    protected LazyValue(int id)
    {
        Id = id;
    }

    protected LazyValue(string name)
    {
        Name = name;
    }

    protected LazyValue(T? data)
    {
        Data = data;
    }

    public int? Id { get; init; }

    protected string? Name { get; init; }

    public T? Data { get; init; }

    public virtual T? Get() => Data;
}
