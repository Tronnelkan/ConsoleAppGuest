using BusinessLogic.Interfaces;
using Domain.Models;

namespace BusinessLogic.Services
{
    public class SessionService : ISessionService
    {
        public User CurrentUser { get; set; }
    }
}