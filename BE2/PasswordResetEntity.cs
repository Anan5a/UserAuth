using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BE
{
    public class PasswordResetEntity
    {
        public long? Id { get; set; }
        public string Token { get; set; }
        public long UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        

    }
}
