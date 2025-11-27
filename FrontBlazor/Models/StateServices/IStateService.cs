namespace FrontBlazor.Models.StateServices;

public interface IStateService<T>
{
    public T CurrentEntity { get; set; } 
}