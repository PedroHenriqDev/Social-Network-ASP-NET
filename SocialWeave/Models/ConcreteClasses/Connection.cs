using System.ComponentModel.DataAnnotations;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Connection
    {
        [Required]
        User UserConnected {  get; set; }

        [Required]
        User UserReceivedConnection { get; set; }

        public Connection() 
        {
            UserConnected = new User();
            UserReceivedConnection = new User();
        }
    }
}
