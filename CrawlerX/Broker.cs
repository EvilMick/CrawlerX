using Nest;
using System;

namespace CrawlerX
{

    class Broker
    {
        public Broker()
        {

        }
        public static ElasticClient EsClient()
        {
            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;


            //Connection string for Elasticsearch
            connectionSettings = new ConnectionSettings(new Uri("http://localhost:9200/")); //local PC
            elasticClient = new ElasticClient(connectionSettings);

            var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };

            var indexConfig = new IndexState
            {
                Settings = settings
            };
            return elasticClient;
        }

    }
}