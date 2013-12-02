using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickVoter.Services
{
    public class InMemoryQuestionService //B: IQuestionService
    {
        private Dictionary<string, Question> _questions = new Dictionary<string, Question>();
        private int _lastId = 0;
 
        public Question AddQuestion(string text)
        {
            _lastId++;
            var question = new Question() {QuestionId = _lastId.ToString(), Text = text, Answers = new List<Answer>()};
            _questions[question.QuestionId] = question;

            return question;
        }

        public Question GetQuestion(string questionId)
        {
            Question q;
            _questions.TryGetValue(questionId, out q);
            return q;
        }

        public IEnumerable<Question> GetQuestions()
        {
            return _questions.Values.ToArray();
        }

        public Answer AddAnswer(string questionId, string text)
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

        public Answer AddVote(string questionId, int answerId)
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