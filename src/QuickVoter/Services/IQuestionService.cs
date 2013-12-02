using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using QuickVoter.Modules;

namespace QuickVoter.Services
{
    public interface IQuestionService
    {
        Question AddQuestion(string text);
        Question GetQuestion(string questionId);
        IEnumerable<Question> GetQuestions();
        Answer AddAnswer(string questionId, string text);
        Answer AddVote(string questionId, int answerId);
    }

    public interface IMongoQuestionServiceConfiguration
    {
        string ConnectionString { get; }    
    }

    public class MongoQuestionServiceConfiguration : IMongoQuestionServiceConfiguration
    {
        public string ConnectionString { get { return ConfigurationManager.AppSettings["mongoConnection"]; } }
    }

    public class MongoQuestionService : IQuestionService
    {
        private readonly IMongoQuestionServiceConfiguration _config;
        private readonly MongoDatabase _database;

        public class QuestionModel 
        {
            public ObjectId Id { get; set; }
            public string Text { get; set; }
            public List<AnswerModel> Answers { get; set; }
        }

        public class AnswerModel
        {
            public long AnswerId { get; set; }
            public string Text { get; set; }
            public int Votes { get; set; }
        }

        public MongoQuestionService(IMongoQuestionServiceConfiguration config)
        {
            _config = config;
            var client = new MongoClient(_config.ConnectionString);
            var server = client.GetServer();
            _database = server.GetDatabase("quickvoter-data");
        }

        public Question AddQuestion(string text)
        {
            var questionsCollection = _database.GetCollection<QuestionModel>("questions");
            var question = new QuestionModel()
            {
                Text = text,
                Answers = new List<AnswerModel>()
            };
            questionsCollection.Insert(question);
            return BuildQuestion(question);
        }

        public Question GetQuestion(string questionId)
        {
            var questionsCollection = _database.GetCollection<QuestionModel>("questions");
            var q = Query.EQ("_id", questionId);

            var question = questionsCollection.FindOne(q);

            return BuildQuestion(question);
        }

        private static Question BuildQuestion(QuestionModel q)
        {
            var questionId = q.Id.ToString();
            var question = new Question()
            {
                QuestionId = questionId, 
                Text = q.Text, 
            };
            if (q.Answers != null)
            {
                question.Answers =
                    q.Answers.Select(
                        a =>
                            new Answer()
                            {
                                AnswerId = a.AnswerId,
                                Text = a.Text,
                                NumberOfVotes = a.Votes,
                                QuestionId = questionId
                            }).ToList();
            }
            else
            {
                question.Answers = new List<Answer>();
            }
            return question;
        }

        public IEnumerable<Question> GetQuestions()
        {
            var questionsCollection = _database.GetCollection<QuestionModel>("questions");
            var questions = questionsCollection.FindAll();

            return questions.Select(BuildQuestion).ToArray();
        }

        public Answer AddAnswer(string questionId, string text)
        {
            throw new NotImplementedException();
        }

        public Answer AddVote(string questionId, int answerId)
        {
            throw new NotImplementedException();
        }
    }
}