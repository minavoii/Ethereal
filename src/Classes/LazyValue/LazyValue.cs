using System.Threading.Tasks;

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

    protected int? Id { get; init; }

    protected string? Name { get; init; }

    protected T? Data { get; init; }

    public abstract Task<T?> Get();
}
