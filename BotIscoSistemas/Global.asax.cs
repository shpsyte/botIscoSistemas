using System;
using Autofac;
using System.Web.Http;
using System.Configuration;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using BotBlog.Models;
using System.Threading;
using System.Globalization;

namespace BotBlog
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            //var uri = new System.Uri(KeyPassAndPhrase._cosmoDburi);
            //var key = KeyPassAndPhrase._cosmokey;
            //var store = new DocumentDbBotDataStore(uri, key);

            var store = new InMemoryDataStore();

            Conversation.UpdateContainer(
                        builder =>
                        {
                            builder.Register(c => store)
                                .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                                .AsSelf()
                                .SingleInstance();

                            builder.Register(c => new CachingBotDataStore(store, CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
                                .As<IBotDataStore<BotData>>()
                                .AsSelf()
                                .InstancePerLifetimeScope();

                        });


            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
            GlobalConfiguration.Configure(WebApiConfig.Register);


        }
    }
}
