using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Nancy;
using QuickVoter.Services;

namespace QuickVoter.Modules
{

    public class QuestionsResource
    {
        public List<QuestionItemResource> Questions { get; set; } 
    }

    public class QuestionItemResource
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public int NumberOfVotes { get; set; }
        public int NumberOfAnswers { get; set; }
    }

    public class QuestionResource
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public List<AnswerItemResource> Answers { get; set; }  
    }

    public class AnswerItemResource
    {
        public int AnswerId { get; set; }
        public string Text { get; set; }
        public int NumberOfVotes { get; set; }
    }

    public class QuestionsModule : NancyModule
    {

        public QuestionsModule(IQuestionService questionService)
        {
            Get["/questions"] = _ =>
            {
                var questions = questionService.GetQuestions().Select(BuildQuestionItemResource);
                return Response.AsJson(
                    new QuestionsResource() {Questions = questions.ToList()}
                    );
            };

            Get["/questions/{questionId}"] = pars =>
            {
                var question = questionService.GetQuestion((int)pars.questionId);
                if (question == null)
                    return HttpStatusCode.NotFound;

                return Response.AsJson(
                    BuildQuestionResource(question));
            };

            Post["/questions"] = pars =>
            {
                string text = Request.Form.text;
                var question = questionService.AddQuestion(text);

                return Response.AsJson(BuildQuestionItemResource(question));
            };

            Post["/questions/{questionId}/answers"] = pars =>
            {
                string text = Request.Form.text;
                var answer = questionService.AddAnswer((int)pars.questionId, text);

                return Response.AsJson(BuildAnswerItemResource(answer));
            };

            Post["/questions/{questionId}/answers/{answerId}"] = pars =>
            {
                var answer = questionService.AddVote((int)pars.questionId, (int)pars.answerId);

                return Response.AsJson(BuildAnswerItemResource(answer));
            };
        }

        private static QuestionItemResource BuildQuestionItemResource(Question q)
        {
            return new QuestionItemResource()
            {
                QuestionId = q.QuestionId,
                Text = q.Text,
                NumberOfAnswers = q.Answers.Count,
                NumberOfVotes = q.Answers.Sum(a => a.NumberOfVotes)
            };
        }
        private static QuestionResource BuildQuestionResource(Question q)
        {
            return new QuestionResource()
            {
                QuestionId = q.QuestionId,
                Text = q.Text,
                Answers = q.Answers.Select(BuildAnswerItemResource).ToList()
            };
        }

        private static AnswerItemResource BuildAnswerItemResource(Answer a)
        {
            return new AnswerItemResource()
            {
                AnswerId = a.AnswerId,
                Text = a.Text,
                NumberOfVotes = a.NumberOfVotes
            };
        }
    }


}