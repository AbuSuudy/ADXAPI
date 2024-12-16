using System.ComponentModel;

namespace ADXAPI.Model
{
    public class User
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool ADXUser { get; set; }    
    }
}
