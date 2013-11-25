using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace QuickVoter.Services
{
    public interface IQuestionService
    {
        Question AddQuestion(string text);
        Question GetQuestion(int questionId);
        IEnumerable<Question> GetQuestions();
        Answer AddAnswer(int questionId, string text);
        Answer AddVote(int questionId, int answerId);
    }

    public class InMemoryQuestionService : IQuestionService
    {
        private Dictionary<int, Question> _questions = new Dictionary<int, Question>();
        private int _lastId = 0;
 
        public Question AddQuestion(string text)
        {
            _lastId++;
            var question = new Question() {QuestionId = _lastId, Text = text, Answers = new List<Answer>()};
            _questions[_lastId] = question;

            return question;
        }

        public Question GetQuestion(int questionId)
        {
            Question q;
            _questions.TryGetValue(questionId, out q);
            return q;
        }

        public IEnumerable<Question> GetQuestions()
        {
            return _questions.Values.ToArray();
        }

        public Answer AddAnswer(int questionId, string text)
        {
            Question q;
            if (!_questions.TryGetValue(questionId, out q))
                throw new InvalidOperationException(string.Format("No question with id {0} found", questionId));

            var lastAnswerId = q.Answers.Max(a => (int?) a.AnswerId).GetValueOrDefault();
            lastAnswerId++;
            var answer = new Answer()
            {
                AnswerId = lastAnswerId,
                NumberOfVotes = 1,
                QuestionId = questionId,
                Text = text
            };
            q.Answers.Add(answer);

            return answer;
        }

        public Answer AddVote(int questionId, int answerId)
        {
            Question q;
            if (!_questions.TryGetValue(questionId, out q))
                throw new InvalidOperationException(string.Format("No question with id {0} found", questionId));

            var answer = q.Answers.Find(a => a.AnswerId == answerId);
            answer.NumberOfVotes++;

            return answer;
        }
    }
}