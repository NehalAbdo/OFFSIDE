namespace OFF.BLL.Interfaces
{
    public interface IUnitOfWork: IAsyncDisposable
    {
        public IPlayerRepository Players  { get; }
        public IAgentRepository Agents { get; }
        public Task<int> completeAsync();
        IPostRepository Posts { get; }
        public IPaymentRepository Payments { get; }
        public ISubscriptionRepository Subscriptions { get; }
        public IContactRepository Contacts { get; }


    }
}
