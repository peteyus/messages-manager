namespace Core.Interfaces
{
    public interface IModelMapper<TIn, TOut>
    {
        TOut Map(TIn input);
    }
}
