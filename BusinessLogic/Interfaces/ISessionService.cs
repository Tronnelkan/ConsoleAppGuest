using Domain.Models;

namespace BusinessLogic.Interfaces
{
    public interface ISessionService
    {
        User CurrentUser { get; set; }
    }
}