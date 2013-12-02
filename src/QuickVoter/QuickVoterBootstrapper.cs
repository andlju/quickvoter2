using System;
using System.Collections.Generic;
using Nancy.Conventions;
using Nancy.Hosting.Aspnet;
using Nancy.TinyIoc;
using QuickVoter.Services;

namespace QuickVoter
{
    public class QuickVoterBootstrapper : DefaultNancyAspNetBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<IQuestionService, MongoQuestionService>();
            base.ConfigureApplicationContainer(container);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/Scripts"));
            base.ConfigureConventions(nancyConventions);
        }
    }
}