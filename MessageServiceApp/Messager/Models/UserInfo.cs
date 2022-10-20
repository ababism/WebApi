using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Messager.Models
{
    /// <summary>
    /// Реализует информацию о пользователе согласно тз
    /// </summary>
    [DataContract]
    public class UserInfo
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [DataMember]
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [DataMember]
        [Required]
        public string Email { get; set; }
        //public int Id { get; set; }
    }
}

