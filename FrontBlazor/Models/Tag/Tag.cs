using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontBlazor.Models;
public class Tag : IEntity
{
    public int TagId { get; set; }
    
    public string? LibelleTag { get; set; } = null;

    public int GetId()
    {
        return TagId;
    }
}