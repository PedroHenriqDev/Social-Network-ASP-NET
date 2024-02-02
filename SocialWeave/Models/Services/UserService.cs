using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SocialWeave.Data;
using SocialWeave.Exceptions;
using SocialWeave.Models.ConcreteClasses;

namespace SocialWeave.Models.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

      

    }
}
