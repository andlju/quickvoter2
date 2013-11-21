using System.Runtime.Remoting.Messaging;
using Nancy;

namespace QuickVoter.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => View["index"];

            Get["/question/{questionId}"] = pars => View["question"];

        }
    }
}