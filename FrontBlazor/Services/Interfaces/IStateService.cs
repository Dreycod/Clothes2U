namespace FrontBlazor.Services.Interfaces;

public interface IStateService<T>
{
    public T CurrentEntity { get; set; } 

    public T GetEntity();
    public void SetEntity(T entity);
    public void ClearEntity();
}