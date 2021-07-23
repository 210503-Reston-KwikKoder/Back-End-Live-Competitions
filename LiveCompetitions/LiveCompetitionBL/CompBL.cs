﻿
using LiveComeptitionModels;
using LiveCompetitionDL;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCompetitionBL
{
    public class CompBL:ICompBL
    {
        private readonly Repo _repo;
        public CompBL(CBEDbContext context)
        {
            _repo = new Repo(context);

        }
        public async Task<int> AddCompetition(DateTime startDate, DateTime endDate, int categoryId, string competitionName, int user, string teststring)
        {
            Competition competition = new Competition();
            competition.StartDate = startDate;
            competition.EndDate = endDate;
            competition.CategoryId = categoryId;
            competition.CompetitionName = competitionName;
            competition.TestString = teststring;
            competition.UserCreatedId = user;
            return await _repo.AddCompetition(competition);
        }
        public async Task<int> AddCompetition(DateTime startDate, DateTime endDate, int categoryId, string competitionName, int user, string teststring, string author)
        {
            Competition competition = new Competition();
            competition.StartDate = startDate;
            competition.EndDate = endDate;
            competition.CategoryId = categoryId;
            competition.CompetitionName = competitionName;
            competition.TestString = teststring;
            competition.UserCreatedId = user;
            competition.TestAuthor = author;
            return await _repo.AddCompetition(competition);
        }

        public async Task<int> AddLiveCompetition(LiveCompetition liveCompetition)
        {
            return await _repo.AddLiveCompetition(liveCompetition);
        }

        public async Task<LiveCompetitionTest> AddLiveCompetitionTest(LiveCompetitionTest liveCompetitionTest)
        {
            return await _repo.AddLiveCompetitionTest(liveCompetitionTest);
        }

        public async Task<UserQueue> AddToQueue(UserQueue userQueue)
        {
            userQueue.EnterTime = DateTime.Now;
            return await _repo.AddToQueue(userQueue);

        }

        public async Task<LiveCompStat> AddUpdateLiveCompStat(int liveCompId, int userId, bool won)
        {
            try
            {
                return await _repo.AddUpdateLiveCompStat(liveCompId, userId, won);
            }catch(Exception e)
            {
                Log.Error("Unusual error, check to make sure database is correctly persisting users and competitions");
                Log.Error(e.StackTrace);
                return null;
            }
        }

        public async Task<UserQueue> DeleteUserFromQueue(int liveCompId, int userId)
        {
            return await _repo.DeleteUserFromQueue(liveCompId, userId);
        }

        public async Task<UserQueue> DeQueueUserQueue(int liveCompId)
        {
            return await _repo.DeQueueUserQueue(liveCompId);
        }

        public async Task<List<Competition>> GetAllCompetitions()
        {
            return await _repo.GetAllCompetitions();
        }

        public async Task<Competition> GetCompetition(int compId)
        {
            return await _repo.GetCompetition(compId);
        }

        public async Task<List<CompetitionStat>> GetCompetitionStats(int competitionId)
        {
            return await _repo.GetCompStats(competitionId);
        }

        public async Task<string> GetCompString(int compId)
        {
            return await _repo.GetCompetitionString(compId);
        }

        public async Task<Tuple<string, string, int>> GetCompStuff(int compId)
        {
            return await _repo.GetCompStuff(compId);
        }

        public async Task<LiveCompetition> GetLiveCompetition(int id)
        {
            return await _repo.GetLiveCompetition(id);
        }

        public async Task<List<LiveCompetition>> GetLiveCompetitions()
        {
            return await _repo.GetLiveCompetitions();
        }

        public async Task<List<LiveCompetitionTest>> GetLiveCompetitionTestsForCompetition(int compId)
        {
            return await _repo.GetLiveCompetitionTestsForCompetition(compId);
        }

        public async Task<List<UserQueue>> GetLiveCompetitionUserQueue(int liveCompId)
        {
            return await _repo.GetLiveCompetitionUserQueue(liveCompId);
        }

        public async Task<List<LiveCompStat>> GetLiveCompStats(int liveCompId)
        {
            return await _repo.GetLiveCompStats(liveCompId);
        }

        public async Task<int> InsertCompStatUpdate(CompetitionStat competitionStat, int numberWords, int numberErrors)
        {
            try
            {
                double numWords = (double)numberWords;
                numWords = numWords / 5;
                double numErrors = (double)numberErrors;
                numErrors = numErrors / 5;
                competitionStat.Accuracy = (numWords - numErrors) / numWords;
                if (await _repo.AddCompStat(competitionStat) == null)
                {
                    Log.Error("Repo could not add competitionStat");
                    return -1;
                }
                List<CompetitionStat> competitionStats = await _repo.GetCompStats(competitionStat.CompetitionId);
                int i = 0;
                foreach (CompetitionStat c in competitionStats)
                {
                    i += 1;
                    c.rank = i;
                    await _repo.UpdateCompStat(c);
                }

                return competitionStats.First(comp => comp.UserId == competitionStat.UserId).rank;
            }
            catch (Exception)
            {
                Log.Error("error in insertCompStat returning null");
                return -1;
            }
        }

    }
}
