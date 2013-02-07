using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Client.Listeners;

namespace TimeTracker.Queries
{
    public abstract class DocumentTestBase
    {
        protected DocumentTestBase()
        {
            DocumentStore = new EmbeddableDocumentStore
                                {
                                    RunInMemory = true
                                }
                .RegisterListener(new NoStaleQueriesListener());

            //  DocumentStore.Conventions.DefaultQueryingConsistency = ConsistencyOptions.QueryYourWrites;

            DocumentStore.Initialize();
        }

        public IDocumentStore DocumentStore { get; set; }

        public IDocumentSession DocumentSession
        {
            get { return DocumentStore.OpenSession(); }
        }

        private class NoStaleQueriesListener : IDocumentQueryListener
        {
            public void BeforeQueryExecuted(IDocumentQueryCustomization queryCustomization)
            {
                queryCustomization.WaitForNonStaleResults();
            }
        }
    }
}