using System.Security.Principal;
using Newtonsoft.Json;

namespace TimeTracker.Models
{
    public class User
    {
        public User()
        {
            DateTimeZone = "Europe/Stockholm";
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string DateTimeZone { get; set; }
        public string ClaimedIdentifier { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(new SerializePrincipal
                                                   {
                                                       Email = Email,
                                                       Id = Id,
                                                       Fullname = FullName,
                                                       DateTimeZone = DateTimeZone
                                                   });
        }
    }

    public interface ICustomPrincipal : IPrincipal
    {
        int Id { get; set; }
        string Fullname { get; set; }
        string Email { get; set; }
        string DateTimeZone { get; set; }
    }

    public class CustomPrincipal : ICustomPrincipal
    {
        private CustomPrincipal(SerializePrincipal serializedPrincipal)
        {
            Identity = new GenericIdentity(serializedPrincipal.Email);
            Id = serializedPrincipal.Id;
            Fullname = serializedPrincipal.Fullname;
            Email = serializedPrincipal.Email;
            DateTimeZone = serializedPrincipal.DateTimeZone;
        }

        public IIdentity Identity { get; private set; }

        public bool IsInRole(string role)
        {
            return false;
        }

        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string DateTimeZone { get; set; }

        public static IPrincipal Deserialize(string json)
        {
            var serializePrincipal = JsonConvert.DeserializeObject<SerializePrincipal>(json);
            return new CustomPrincipal(serializePrincipal);
        }
    }

    public class SerializePrincipal
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string DateTimeZone { get; set; }
    }
}