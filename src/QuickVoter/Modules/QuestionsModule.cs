using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.UI.HtmlControls;
using Microsoft.AspNet.SignalR;
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
        public string QuestionId { get; set; }
        public string Text { get; set; }
        public long Votes { get; set; }
        public long NumberOfAnswers { get; set; }
    }

    public class QuestionResource
    {
        public string QuestionId { get; set; }
        public string Text { get; set; }
        public List<AnswerItemResource> Answers { get; set; }  
    }

    public class AnswerItemResource
    {
        public string QuestionId { get; set; }
        public long AnswerId { get; set; }
        public string Text { get; set; }
        public long Votes { get; set; }
    }

    public class QuestionsModule : NancyModule
    {

        public QuestionsModule(IQuestionService questionService) : base("/api")
        {
            Get["/questions"] = _ =>
            {
                var questions = questionService.GetQuestions().Select(BuildQuestionItemResource);
                return Response.AsJson(
                    new QuestionsResource() {Questions = questions.ToList()}
                    ).WithHeader("Cache-Control", "no-cache");
            };

            Get["/questions/{questionId}"] = pars =>
            {
                string questionId = pars.questionId;
                var question = questionService.GetQuestion(questionId);
                if (question == null)
                    return HttpStatusCode.NotFound;

                return Response.AsJson(
                    BuildQuestionResource(question)).
                    WithHeader("Cache-Control", "no-cache");
            };

            Post["/questions"] = pars =>
            {
                string text = Request.Form.text;
                var question = questionService.AddQuestion(text);

                var resource = BuildQuestionItemResource(question);

                GlobalHost.ConnectionManager.GetHubContext<QuestionsHub>().Clients.All.questionAdded(resource);

                return Response.AsJson(resource);
            };

            Post["/questions/{questionId}/answers"] = pars =>
            {
                string text = Request.Form.text;
                var answer = questionService.AddAnswer((string)pars.questionId, text);

                var resource = BuildAnswerItemResource(answer);

                GlobalHost.ConnectionManager.GetHubContext<QuestionsHub>().Clients.Group(resource.QuestionId).answerAdded(resource);

                return Response.AsJson(resource);
            };

            Post["/questions/{questionId}/answers/{answerId}"] = pars =>
            {
                var answer = questionService.AddVote((string)pars.questionId, (int)pars.answerId);

                var resource = BuildAnswerItemResource(answer);

                GlobalHost.ConnectionManager.GetHubContext<QuestionsHub>().Clients.Group(resource.QuestionId).answerUpdated(resource);

                return Response.AsJson(resource);
            };

            Post["/questions/seed"] = _ =>
            {
                questionService.DeleteAll();

                var question1 = questionService.AddQuestion("Vilken musik kodar du helst till?");
                questionService.AddAnswer(question1.QuestionId, "Dubstep");
                questionService.AddAnswer(question1.QuestionId, "Metal");
                questionService.AddAnswer(question1.QuestionId, "Klassiskt");
                questionService.AddAnswer(question1.QuestionId, "Eurodisco");

                var question2 = questionService.AddQuestion("Vad är det bästa med CSS?");
                questionService.AddAnswer(question2.QuestionId, "Inget");
                questionService.AddAnswer(question2.QuestionId, "Skojjar du?");
                questionService.AddAnswer(question2.QuestionId, "Bootstrap");

                return HttpStatusCode.OK;
            };

        }

        private static QuestionItemResource BuildQuestionItemResource(Question q)
        {
            return new QuestionItemResource()
            {
                QuestionId = q.QuestionId,
                Text = q.Text,
                NumberOfAnswers = q.Answers.Count,
                Votes = q.Answers.Sum(a => a.Votes)
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
                QuestionId = a.QuestionId,
                AnswerId = a.AnswerId,
                Text = a.Text,
                Votes = a.Votes
            };
        }
    }


}