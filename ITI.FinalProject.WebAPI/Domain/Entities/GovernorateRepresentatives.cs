using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class GovernorateRepresentatives
    {
        [ForeignKey("governorate")]
        public int governorateId { get; set; }
        [ForeignKey("representative")]
        public string representativeId { get; set; }

        public Governorate governorate { get; set; }
        public Representative representative { get; set; }
    }
}
