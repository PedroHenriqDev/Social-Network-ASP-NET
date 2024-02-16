using Microsoft.AspNetCore.SignalR.Protocol;
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

    }
}
