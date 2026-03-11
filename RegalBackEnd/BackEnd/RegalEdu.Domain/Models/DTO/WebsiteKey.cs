using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegalEdu.Domain.Models.DTO
{
    public class WebsiteKey
    {
        public WebsiteKey( ) { }

        public required string Key { get; set; }
        public required int Frequency { get; set; }
    }
}
