using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextShare.Domain.Models.EntityModels.FriendsModels
{
    public class FriendsSearchModel
    {
        [StringLength(100, ErrorMessage = "Слишком длинный запрос")]
        public string? Search { get; set; } = null;     
    }
}
