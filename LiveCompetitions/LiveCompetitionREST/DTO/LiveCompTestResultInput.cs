﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveCompetitionREST.DTO
{
    public class LiveCompTestResultInput:TypeTestInput
    {
        public LiveCompTestResultInput() { }
        public bool won { get; set; }
        public int winStreak { get; set; }
    }
}
