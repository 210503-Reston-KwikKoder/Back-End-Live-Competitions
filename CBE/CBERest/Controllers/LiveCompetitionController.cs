﻿using CBERest.DTO;
using LiveComeptitionModels;
using LiveCompetitionBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LiveCompetitionREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveCompetitionController : ControllerBase
    {
        private readonly ICompBL _compBL;
        private readonly ICategoryBL _categoryBL;
        private readonly IUserBL _userBL;
        private readonly ApiSettings _ApiSettings;

        public LiveCompetitionController(ICompBL compBL, ICategoryBL catBL, IUserBL uBL, IOptions<ApiSettings> settings)
        {
            _compBL = compBL;
            _categoryBL = catBL;
            _userBL = uBL;
            _ApiSettings = settings.Value;

        }
        // GET: api/<LiveCompetitionController>
        [HttpGet]
        public async Task<IEnumerable<LiveCompOutput>> Get()
        {
            List<LiveCompOutput> liveCompOutputs = new List<LiveCompOutput>();
            List<LiveCompetition> liveCompetitions = await _compBL.GetLiveCompetitions();
            foreach (LiveCompetition liveCompetition in liveCompetitions)
            {
                LiveCompOutput liveCompOutput = new LiveCompOutput();
                liveCompOutput.Id = liveCompetition.Id;
                liveCompOutput.Name = liveCompetition.Name;
                liveCompOutputs.Add(liveCompOutput);
            }
            return liveCompOutputs;
        }

        // GET api/<LiveCompetitionController>/5
        [HttpGet("{id}", Name = "GetComp")]
        public async Task<IEnumerable<LiveCompTestOutput>> Get(int id)
        {
            List<LiveCompetitionTest> liveCompetitionTests = await _compBL.GetLiveCompetitionTestsForCompetition(id);
            List<LiveCompTestOutput> liveCompTestOutputs = new List<LiveCompTestOutput>();
            foreach (LiveCompetitionTest liveCompetitionTest in liveCompetitionTests)
            {
                LiveCompTestOutput liveCompTestOutput = new LiveCompTestOutput();
                liveCompTestOutput.Category = (await _categoryBL.GetCategoryById(liveCompetitionTest.CategoryId)).Name;
                liveCompTestOutput.CompId = liveCompetitionTest.LiveCompetitionId;
                liveCompTestOutput.TestAuthor = liveCompetitionTest.TestAuthor;
                liveCompTestOutput.TestString = liveCompetitionTest.TestString;
                liveCompTestOutputs.Add(liveCompTestOutput);
            }
            return liveCompTestOutputs;
        }
        [HttpGet("latest/{id}")]
        public async Task<ActionResult<LiveCompTestOutput>> GetLast(int id)
        {
            List<LiveCompetitionTest> liveCompetitionTests = await _compBL.GetLiveCompetitionTestsForCompetition(id);
            if (liveCompetitionTests.Count == 0) return NotFound();
            LiveCompetitionTest liveCompetitionTest = liveCompetitionTests[liveCompetitionTests.Count - 1];
           
            LiveCompTestOutput liveCompTestOutput = new LiveCompTestOutput();
            liveCompTestOutput.Category = (await _categoryBL.GetCategoryById(liveCompetitionTest.CategoryId)).Name;
            liveCompTestOutput.CompId = liveCompetitionTest.LiveCompetitionId;
            liveCompTestOutput.TestAuthor = liveCompetitionTest.TestAuthor;
            liveCompTestOutput.TestString = liveCompetitionTest.TestString;
            return liveCompTestOutput;
        }
        // POST api/<LiveCompetitionController>

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PostAsync(LiveCompInput liveCompInput)
        {
            LiveCompetition liveCompetition = new LiveCompetition();
            liveCompetition.Name = liveCompInput.Name;
            if (string.IsNullOrEmpty(liveCompetition.Name)||string.IsNullOrWhiteSpace(liveCompetition.Name)) return BadRequest();
            int compId = await _compBL.AddLiveCompetition(liveCompetition);
            if (compId == -1) return BadRequest();
            return CreatedAtRoute(
                                       routeName: "GetComp",
                                       routeValues: new { id = compId },
                                       value: compId);
        }
        [HttpPut("nexttest")]
        public async Task<ActionResult> PutAsync(LiveCompTestInput liveCompTestInput)
        {
            try
            {
                LiveCompetitionTest liveCompetitionTest = new LiveCompetitionTest();
                liveCompetitionTest.TestAuthor = liveCompTestInput.testAuthor;
                liveCompetitionTest.TestString = liveCompTestInput.testString;
                if (await _categoryBL.GetCategory(liveCompTestInput.category) == null)
                {
                    Category category = new Category();
                    category.Name = liveCompTestInput.category;
                    await _categoryBL.AddCategory(category);
                }
                Category category1 = await _categoryBL.GetCategory(liveCompTestInput.category);
                liveCompetitionTest.CategoryId = category1.Id;
                liveCompetitionTest.LiveCompetitionId = liveCompTestInput.compId;
                liveCompetitionTest.DateCreated = DateTime.Now;
                await _compBL.AddLiveCompetitionTest(liveCompetitionTest);
                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                Log.Error("Unexpected error returning 400");
                return NotFound();
            }
        }
        [HttpDelete("LCQ/NextUser/{id}")]
        public async Task<ActionResult<QueueModel>> DeQueue(int id)
        {
            try
            {
                UserQueue userQueue = await _compBL.DeQueueUserQueue(id);
                QueueModel queueModel = new QueueModel();
                queueModel.enterTime = userQueue.EnterTime;
                try
                {
                    User u = await _userBL.GetUser(id);
                    dynamic AppBearerToken = GetApplicationToken();
                    queueModel.userId = u.Auth0Id;
                    var client = new RestClient($"https://kwikkoder.us.auth0.com/api/v2/users/{u.Auth0Id}");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("authorization", "Bearer " + AppBearerToken.access_token);
                    IRestResponse restResponse = await client.ExecuteAsync(request);
                    dynamic deResponse = JsonConvert.DeserializeObject(restResponse.Content);
                    queueModel.name = deResponse.name;
                    queueModel.userName = deResponse.username;
                }
                catch (Exception e)
                {
                    Log.Information(e.Message);
                }
                return queueModel;
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return NotFound();
            }
        }
        [HttpGet("LCQ/{id}")]
        public async Task<ActionResult<List<QueueModel>>> GetLCQ(int id)
        {
            List<UserQueue> userQueues = await _compBL.GetLiveCompetitionUserQueue(id);
            List<QueueModel> queueModels = new List<QueueModel>();
            if (userQueues.Count == 0) return NotFound();
            else foreach (UserQueue userQueue in userQueues)
                {
                    QueueModel queueModel = new QueueModel();
                    queueModel.enterTime = userQueue.EnterTime;
                    try
                    {
                        //getting user information from auth0
                        User u = await _userBL.GetUser(userQueue.UserId);
                        dynamic AppBearerToken = GetApplicationToken();
                        queueModel.userId = u.Auth0Id;
                        var client = new RestClient($"https://kwikkoder.us.auth0.com/api/v2/users/{u.Auth0Id}");
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("authorization", "Bearer " + AppBearerToken.access_token);
                        IRestResponse restResponse = await client.ExecuteAsync(request);
                        dynamic deResponse = JsonConvert.DeserializeObject(restResponse.Content);
                        queueModel.name = deResponse.name;
                        queueModel.userName = deResponse.username;
                    }
                    catch (Exception e)
                    {
                        Log.Information(e.Message);
                    }
                    queueModels.Add(queueModel);
                }
            return queueModels;
        }
        [Authorize]
        [HttpPut("LCQ/{id}")]
        public async Task<ActionResult> EnQueueUser(int id)
        {
            //check if user exists before operations
            string UserID = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (await _userBL.GetUser(UserID) == null)
            {
                User user = new User();
                user.Auth0Id = UserID;
                await _userBL.AddUser(user);
            }
            User u = await _userBL.GetUser(UserID);
            UserQueue userQueue = new UserQueue();
            userQueue.LiveCompetitionId = id;
            userQueue.UserId = u.Id;
            if (await _compBL.AddToQueue(userQueue) == null) return NotFound();
            else return Ok();

        }
        [Authorize]
        [HttpDelete("LCQ/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                string UserID = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                User u = await _userBL.GetUser(UserID);
                if (await _compBL.DeleteUserFromQueue(id, u.Id) == null) return NotFound();
                else return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
                return NotFound();
            }

        }
        [HttpGet("LCS/{id}")]
        public async Task<ActionResult<IEnumerable<LiveCompStatModel>>> GetModels(int id)
        {
            List<LiveCompStatModel> lcsModels = new List<LiveCompStatModel>();
            List<LiveCompStat> liveCompStats = await _compBL.GetLiveCompStats(id);
            //404 if no live comp stats can be found
            if (liveCompStats.Count == 0) return NotFound();
            else foreach (LiveCompStat liveCompStat in liveCompStats)
                {
                    LiveCompStatModel lcsModel = new LiveCompStatModel();
                    lcsModel.Losses = liveCompStat.Losses;
                    lcsModel.Wins = liveCompStat.Wins;
                    lcsModel.WLRatio = liveCompStat.WLRatio;
                    try
                    {
                        //getting user information from auth0
                        User u = await _userBL.GetUser(liveCompStat.UserId);
                        dynamic AppBearerToken = GetApplicationToken();
                        var client = new RestClient($"https://kwikkoder.us.auth0.com/api/v2/users/{u.Auth0Id}");
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("authorization", "Bearer " + AppBearerToken.access_token);
                        IRestResponse restResponse = await client.ExecuteAsync(request);
                        dynamic deResponse = JsonConvert.DeserializeObject(restResponse.Content);
                        lcsModel.name = deResponse.name;
                        lcsModel.userName = deResponse.username;
                    }
                    catch (Exception e)
                    {
                        Log.Information(e.Message);
                    }
                    lcsModels.Add(lcsModel);
                }
            return lcsModels;
        }
        [Authorize]
        [HttpPut("LCS/{id}")]
        public async Task<ActionResult> PutResult(int id, LiveCompTestResultInput liveCompTestResultInput)
        {
            string UserID = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //checking if user exists then adding user if not
            if (await _userBL.GetUser(UserID) == null)
            {
                User user = new User();
                user.Auth0Id = UserID;
                await _userBL.AddUser(user);
            }

            User u = await _userBL.GetUser(UserID);
            if (await _compBL.AddUpdateLiveCompStat(id, u.Id, liveCompTestResultInput.won) == null) return BadRequest();
            else {
                LiveCompTestResultOutput liveCompTestResultOutput = new LiveCompTestResultOutput();
                liveCompTestResultOutput.auth0Id = UserID;
                liveCompTestResultOutput.categoryId = liveCompTestResultInput.categoryId;
                liveCompTestResultOutput.won = liveCompTestResultInput.won;
                liveCompTestResultOutput.wpm = liveCompTestResultInput.wpm;
                liveCompTestResultOutput.timetakenms = liveCompTestResultInput.timetakenms;
                liveCompTestResultOutput.date = liveCompTestResultInput.date;
                liveCompTestResultOutput.numberoferrors = liveCompTestResultInput.numberoferrors;
                liveCompTestResultOutput.winStreak = liveCompTestResultInput.winStreak;
                try
                {
                    string resultJson = JsonConvert.SerializeObject(liveCompTestResultOutput);
                    StringContent content = new StringContent(resultJson, Encoding.UTF8, "application/json");
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage result = await httpClient.PostAsync("http://20.69.69.228/typetest/api/TypeTest/comptest", content);
                }catch(Exception e)
                {
                    Log.Error(e.StackTrace);
                    Log.Error("TypeTest not responding");

                }

                return Ok();
            }
        }
        /// <summary>
        /// Private method to get application token for auth0 management 
        /// </summary>
        /// <returns>dynamic object with token for Auth0 call</returns>
        private dynamic GetApplicationToken()
        {
            var client = new RestClient("https://kwikkoder.us.auth0.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", _ApiSettings.authString, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Log.Information("Response: {0}", response.Content);
            return JsonConvert.DeserializeObject(response.Content);
        }
    }
}