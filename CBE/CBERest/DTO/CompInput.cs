using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveCompetitionREST.DTO
{
    public class CompInput: TypeTestInput
    {
        public CompInput() { }
        public int compId { get; set; }
    }
}
