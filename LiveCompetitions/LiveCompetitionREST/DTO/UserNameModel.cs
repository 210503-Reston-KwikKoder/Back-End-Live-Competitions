using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveCompetitionREST.DTO
{
    public class UserNameModel
    {
        public UserNameModel () { }
        
        public string UserName { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
    }
}
