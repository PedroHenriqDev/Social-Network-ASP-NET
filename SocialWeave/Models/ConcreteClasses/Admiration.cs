using Microsoft.AspNetCore.SignalR.Protocol;
using SocialWeave.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialWeave.Models.ConcreteClasses
{
    public class Admiration
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("UserAdmired")]
        public Guid UserAdmirerId { get; set; }
        public User UserAdmirer { get; set; }

        [ForeignKey("UserReceivedAdmired")]
        public Guid UserAdmiredId { get; set; }
        public User UserAdmired { get; set; }

        public Admiration() 
        {
        }

        public Admiration(User userConnected, User userAdmired, Guid id) 
        {
            UserAdmirer = userConnected;
            UserAdmired = userAdmired;
            Id = id;
        }

        public async Task<bool> HaveAdmiredAsync(string nameUser, string nameCurrentUser) 
        {
            if (nameUser == null || nameCurrentUser == null)
            {
                return false;
            }

            if(UserAdmirer.Name == nameUser || UserAdmired.Name == nameCurrentUser)
            {
                return true;
            }

            return false;
        }
    }
}
