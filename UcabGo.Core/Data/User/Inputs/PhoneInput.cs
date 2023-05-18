using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UcabGo.Core.Data.User.Inputs
{
    public class PhoneInput : BaseRequest
    {
        [Required]
        public string Phone { get; set; }
    }
}
