using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.AspNet.SignalR;

namespace QuickVoter.Modules
{

    public interface IQuestionNotificationContext
    {
        void UpdateQuestion(QuestionItemResource questionItemResource);
        void UpdateAnswer(AnswerItemResource answerItemResource);
    }

    public class QuestionNotificationContext : IQuestionNotificationContext
    {
        private IHubContext _hubContext;

        public QuestionNotificationContext()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<QuestionsHub>();
        }

        public void UpdateQuestion(QuestionItemResource questionItemResource)
        {
            _hubContext.Clients.Group("questions").QuestionUpdated(questionItemResource);
        }

        public void UpdateAnswer(AnswerItemResource answerItemResource)
        {
            _hubContext.Clients.Group(answerItemResource.QuestionId).AnswerUpdated(answerItemResource);
        }
    }

    public class QuestionsHub :Hub
    {
        public void Register(string questionId)
        {
            Groups.Add(Context.ConnectionId, questionId);
        }
    }
}