namespace DeleteConsumer.Repositories
{
    public interface IUsuarioRepository
    {
        Task RemoveAsync(int id);
    }
}
