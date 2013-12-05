using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace QuickVoter.Services
{
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
            var q = Query.EQ("_id", new ObjectId(questionId));

            var question = questionsCollection.FindOne(q);

            return BuildQuestion(question);
        }

        public void DeleteAll()
        {
            _database.DropCollection("questions");
        }

        private static Question BuildQuestion(QuestionModel q)
        {
            if (q == null)
                return null;

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
                        a => BuildAnswer(a, questionId)).ToList();
            }
            else
            {
                question.Answers = new List<Answer>();
            }
            return question;
        }

        private static Answer BuildAnswer(AnswerModel a, string questionId)
        {
            return new Answer()
            {
                AnswerId = a.AnswerId,
                Text = a.Text,
                Votes = a.Votes,
                QuestionId = questionId
            };
        }

        public IEnumerable<Question> GetQuestions()
        {
            var questionsCollection = _database.GetCollection<QuestionModel>("questions");
            var questions = questionsCollection.FindAll();

            return questions.Select(BuildQuestion).ToArray();
        }

        public Answer AddAnswer(string questionId, string text)
        {
            var questionsCollection = _database.GetCollection<QuestionModel>("questions");
            var q = Query.EQ("_id", new ObjectId(questionId));

            var question = questionsCollection.FindOne(q);
            var id = question.Answers.Max(m => (long?) m.AnswerId).GetValueOrDefault(0) + 1;

            var answer = new AnswerModel()
            {
                AnswerId = id,
                Text = text,
                Votes = 1
            };
            question.Answers.Add(answer);

            questionsCollection.Save(question);

            return BuildAnswer(answer, questionId);
        }

        public Answer AddVote(string questionId, long answerId)
        {
            var questionsCollection = _database.GetCollection<QuestionModel>("questions");
            var q = Query.EQ("_id", new ObjectId(questionId));

            var question = questionsCollection.FindOne(q);
            var answer = question.Answers.FirstOrDefault(a => a.AnswerId == answerId);
            if (answer == null)
                return null;

            answer.Votes++;
            questionsCollection.Save(question);
            return BuildAnswer(answer, questionId);
        }
    }
}