﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveComeptitionModels
{
    public class LiveCompetitionTest
    {
        public LiveCompetitionTest() { }
        public int Id { get; set; }
        public LiveCompetition LiveCompetition { get; set; }
        [Required]
        public int LiveCompetitionId { get; set; }
        public string TestString { get; set; }
        public string TestAuthor { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
