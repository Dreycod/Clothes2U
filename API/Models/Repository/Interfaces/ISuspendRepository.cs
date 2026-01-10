namespace API.Models.Repository;

public interface ISuspendRepository
{
    Task SuspendElement(int id);
}