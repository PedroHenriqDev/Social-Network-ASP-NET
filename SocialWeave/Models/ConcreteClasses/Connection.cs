using Microsoft.AspNetCore.SignalR.Protocol;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Connection
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("UserConnected")]
        public Guid UserConnectedId { get; set; }
        public User UserConnected { get; set; }

        [ForeignKey("UserReceivedConnection")]
        public Guid UserReceivedConnectionId { get; set; }
        public User UserReceivedConnection { get; set; }

        public Connection() 
        {

        }

        public Connection(User userConnected, User userReceivedConnection, Guid id) 
        {
            UserConnected = userConnected;
            UserReceivedConnection = userReceivedConnection;
            Id = id;
        }

        public async Task<bool> HaveConnectionAsync(string nameUser, string nameCurrentUser) 
        {
            if (nameUser == null || nameCurrentUser == null)
            {
                return false;
            }

            if(UserConnected.Name == nameUser || UserReceivedConnection.Name == nameCurrentUser)
            {
                return true;
            }

            return false;
        }
    }
}
