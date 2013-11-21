using Nancy.Conventions;
using Nancy.Hosting.Aspnet;

namespace QuickVoter
{
    public class QuickVoterBootstrapper : DefaultNancyAspNetBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/Scripts"));
            base.ConfigureConventions(nancyConventions);
        }
    }
}