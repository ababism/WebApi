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
    public class UserController : Controller
    {
        private const string jsonUsersFilePath = "Users.json";

        public static int seed = 5;
        /// <summary>
        /// Для генерации
        /// </summary>
        internal static Random rnd = new(seed);
        
        private static readonly string[] namePull = { "Adam", "John", "Julia", "Mark", "Efrem",
                                                        "Leon", "Bob", "Cavin", "David", "George"};
        private static readonly string[] emailDomainPull = { "mail.ru", "gmail.com", "edu.hse.ru", "tkoff.com" };

        private static List<UserInfo> _users = new List<UserInfo>();
        public static List<UserInfo> GetUSers => _users;

        public List<UserInfo> GetSortedUsers
        {
            get
            {
                _users.Sort((x, y) => x.Email.CompareTo(y.Email));
                return _users;
            }
        }



        //private static int GetNewId => _users.Count == 0 ? 0 : _users.Max(x => x.Id) + 1;

        // TODO Проверка на Email

        /// <summary>
        /// Заменяет пользователей в файле на сгенерированное указанное количество пользователей
        /// </summary>
        /// <param name="ammountOfUsers">Количество пользователей</param>
        /// <returns></returns>
        [HttpPost("generate-users")]
        public IActionResult GenerateUsers(int ammountOfUsers = 10)
        {
            _users = new();
            for (int i = 0; i < ammountOfUsers; i++)
            {
                var tmp = GenerateUser();
                if (_users.Any(x => x.Email == tmp.Email))
                {
                    int count = 1;
                    var newTmp = new UserInfo()
                    {
                        UserName = tmp.UserName,
                        Email = tmp.Email + "_" + count,
                    };
                    do
                    {
                        newTmp = new UserInfo()
                        {
                            UserName = tmp.UserName,
                            Email = tmp.Email + "_" + count,
                        };
                        count++;
                    } while (_users.Any(x => x.Email == newTmp.Email));
                    _users.Add(newTmp);

                }
                else
                {
                    _users.Add(tmp);
                }
            }
            try
            {
                UpdateSerializationFile();
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = $"Не удалось сохранить пользователей в файл из-за ошибки {ex.Message}" });
            }
            

            return Ok(_users);
        }

        /// <summary>
        /// Добавляет пользователя
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Mail"></param>
        /// <returns></returns>
        [HttpPost("add-user")]
        public IActionResult AddUser(string Name, string Mail)
        {
            var newUser = new UserInfo()
            {
                UserName = Name,
                Email = Mail,
            };
            if (_users.Any(x => x.Email == Mail))
            {
                return BadRequest(new { Message = $"Пользователь с Email {Mail} уже есть" });

            }
            else
            {
                _users.Add(newUser);
                UpdateSerializationFile();

                return Ok(newUser);
            }
        }

            /// <summary>
            /// Загружает пользователей из json файла в папке Messager
            /// </summary>
            /// <param name="jsonUsersFilePath"></param>
            /// <returns></returns>
            [HttpPost("load-users")]
        public IActionResult LoadUsers()
        {
           
            try
            {
                using var fs = new FileStream(jsonUsersFilePath, FileMode.Open, FileAccess.Read);
                var formatter = new DataContractJsonSerializer(typeof(List<UserInfo>));
                _users = (List<UserInfo>)formatter.ReadObject(fs);
                return Ok(_users);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = $"Не загрузить пользователей из файла из-за ошибки {ex.Message}" });
            }
            
        }


        private void UpdateSerializationFile()
        {
            
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<UserInfo>));
            using (FileStream fs = new FileStream(jsonUsersFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                serializer.WriteObject(fs, GetSortedUsers);
            }
        }

        /// <summary>
        /// Генерирует пользователя
        /// </summary>
        /// <returns>Обьект класса UserInfo</returns>
        private static UserInfo GenerateUser()
        {
            string tmpName = namePull[rnd.Next(0, namePull.Length)];
            return new UserInfo()
            {
                UserName = tmpName,
                Email = tmpName + "@" + emailDomainPull[rnd.Next(0, emailDomainPull.Length)],

            };

        }

        /// <summary>
        /// Получает пользователя по его идентификатору
        /// </summary>
        /// <param name="email">Идентификатор</param>
        /// <returns>Пользователя</returns>
        [HttpGet("get-user-by-email")]
        public IActionResult Get(string email)
        {
            var requestResult = _users.Where(x => x.Email == email).ToList();
            if (requestResult.Count == 0)
            {
                return NotFound(new { Message = $"Пользователь с Email {email} не найден" });
            }
            return Ok(requestResult.First());

        }

        /// <summary>
        /// Выводит список всех пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        [HttpGet("get-users")]
        public IActionResult Get()
        {
            //if (_users.Count == 0)
            //{
            //    //return NotFound(new { Message = $"Пользователей пока что нет" });
            //    return NoContent();
            //}

            return Ok(GetSortedUsers);

        }

        // GET: api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //TODO Delete
        // GET api/values/5
        //[HttpGet("get-user-by-id")]
        //public IActionResult Get(int id)
        //{
        //    var requestResult = _users.Where(x => x.Id == id).ToList();
        //    if (requestResult.Count == 0)
        //    {
        //        return NotFound(new { Message = $"Пользователь с Id = {id} не найден" });
        //    }
        //    return Ok(requestResult.First());

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
