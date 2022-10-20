using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Messager.Models
{
    /// <summary>
    /// Реализует сообщение согласно тз
    /// </summary>
    [DataContract]
    public class MessageInfo
    {

        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        [Required]
        public string SenderId { get; set; }
        [DataMember]
        [Required]
        public string ReceiverId { get; set; }
        [DataMember]
        [Required]
        public string Message { get; set; }

        //public int Id { get; set; }
    }
}
