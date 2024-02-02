using SocialWeave.Data;

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
