namespace Shared.DTO.MotInterdit;

public class MotInterditDTO
{
    public int MotInterditId { get; set; }
    public string LibelleMot { get; set; }
    public int GetId()
    {
        return MotInterditId;
    }
}