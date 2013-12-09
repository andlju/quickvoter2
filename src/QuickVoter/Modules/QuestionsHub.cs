using Microsoft.AspNet.SignalR;

namespace QuickVoter.Modules
{
    public class QuestionsHub : Hub
    {
        public void RegisterClient(string questionId)
        {
            Groups.Add(Context.ConnectionId, questionId);
        }
    }
}