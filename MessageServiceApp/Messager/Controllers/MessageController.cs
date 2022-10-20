using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Messager.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Messager.Controllers
{
    

    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private const string messagesFilePath = "Messages.json";
        public static int Seed = 5;
        private static Random rnd = new(5);
        private static List<MessageInfo> _messageInfos = new();


        /// <summary>
        /// Генерирует указанное количество сообщений между существующими пользователями.
        /// </summary>
        /// <param name="ammountOfMessages"></param>
        /// <returns></returns>
        [HttpPost("generate-messages")]
        public IActionResult GenerateMessages(int ammountOfMessages = 4)
        {
            _messageInfos = new();
            for (int i = 0; i < ammountOfMessages; i++)
            {
                _messageInfos.Add(GenerateMessage(UserController.GetUSers[rnd.Next(0, UserController.GetUSers.Count)].Email,
                    UserController.GetUSers[rnd.Next(0, UserController.GetUSers.Count)].Email));
            }
            
            try
            {
                UpdateMessageSerializationFile();
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = $"Не удалось сохранить пользователей в файл из-за ошибки {ex.Message}" });
            }
            return Ok(_messageInfos);
        }

        /// <summary>
        /// Выводит список всех сообщений
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-messages")]
        public IActionResult Get()
        {
            //if (_users.Count == 0)
            //{
            //    //return NotFound(new { Message = $"Пользователей пока что нет" });
            //    return NoContent();
            //}

            return Ok(_messageInfos);
        }

        private void UpdateMessageSerializationFile()
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<MessageInfo>));
            using (FileStream fs = new FileStream(messagesFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                serializer.WriteObject(fs, _messageInfos);
            }
        }

        private static MessageInfo GenerateMessage(string senderId, string recieverId)
        {
            return new MessageInfo()
            {
                SenderId = senderId,
                ReceiverId = recieverId,
                Subject = GenerateSubject(),
                Message = GenerateMessage(),

            };

        }

        private static string GenerateSubject()
        {
            string res = "";
            for (int i = 1; i < rnd.Next(5, 8); i++)
            {
                res += (char)rnd.Next('a', 'z' + 1);
            };
            return res;
        }

        private static string GenerateMessage()
        {
            string res = "";
            for (int i = 1; i < rnd.Next(10, 20); i++)
            {
                res += (char)rnd.Next('a', 'z' + 1);
            };
            return res;
        }

        /// <summary>
        /// Загружает сообщения из json файла в папке Messager
        /// </summary>
        /// <param name="jsonUsersFilePath"></param>
        /// <returns></returns>
        [HttpPost("load-messages")]
        public IActionResult LoadUsers()
        {
            
            try
            {
                using var fs = new FileStream(messagesFilePath, FileMode.Open, FileAccess.Read);
                var formatter = new DataContractJsonSerializer(typeof(List<MessageInfo>));
                _messageInfos = (List<MessageInfo>)formatter.ReadObject(fs);
                return Ok(_messageInfos);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = $"Не загрузить пользователей из файла из-за ошибки {ex.Message}" });
            }

        }

        /// <summary>
        /// Вывод списка сообщений по по идентификатору отправителя.
        /// </summary>
        /// <param name="senderEmail">идентификатор отправителя</param>
        /// <returns></returns>
        [HttpGet("get-messages-sent-from-user")]
        public IActionResult GetFromUser(string senderEmail)
        {
            var requestResult = _messageInfos.Where(x => x.SenderId == senderEmail).ToList();
            if (requestResult.Count == 0)
            {
                return NotFound(new { Message = $"Пользователь с Email {senderEmail} не отправлял сообщений" });
            }
            return Ok(requestResult);

        }

        /// <summary>
        /// Вывод списка сообщений по по идентификатору получателя.
        /// </summary>
        /// <param name="receiverEmail">идентификатор получателя</param>
        /// <returns></returns>
        [HttpGet("get-messages-sent-to-user")]
        public IActionResult GetToUser(string receiverEmail)
        {
            var requestResult = _messageInfos.Where(x => x.ReceiverId == receiverEmail).ToList();
            if (requestResult.Count == 0)
            {
                return NotFound(new { Message = $"Пользователь с Email {receiverEmail} не получал сообщений" });
            }
            return Ok(requestResult);

        }

        /// <summary>
        /// Вывод писка сообщений по идентификатору отправителя и получателя.
        /// </summary>
        /// <param name="senderEmail"></param>
        /// <param name="receiverEmail"></param>
        /// <returns></returns>
        [HttpGet("get-messages-sent-between-users")]
        public IActionResult GetBetweenUsers(string senderEmail, string receiverEmail)
        {
            //var requestResult = _messageInfos.Where(x => (x.SenderId == senderEmail && x.ReceiverId == receiverEmail) ||
            //(x.SenderId == receiverEmail && x.ReceiverId == senderEmail)).ToList();
            var requestResult = _messageInfos.Where(x => (x.SenderId == senderEmail && x.ReceiverId == receiverEmail)).ToList();
            if (requestResult.Count == 0)
            {
                return NotFound(new { Message = $"Пользователь с Email {senderEmail} не отправлял сообщения пользователю {receiverEmail}" });
            }
            return Ok(requestResult);

        }
        //TODO Undone mb Useless
        // GET: api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        // PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
