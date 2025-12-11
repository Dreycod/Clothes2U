
using Microsoft.AspNetCore.Components.Forms;
namespace FrontBlazor.Models;
public class Photo: IEntity
{
    public int? PhotoId { get; set; }
    public IBrowserFile? File { get; set; }
    public int GetId()
    {
        return PhotoId ?? 0;
    }
}