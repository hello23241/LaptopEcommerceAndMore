using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Services
{
    public class HeaderBadgeService(IHttpContextAccessor httpContextAccessor) : IHeaderBadgeService
    {
        private const string WishlistCountKey = "WishlistCount";
        private const string CartCountKey = "CartCount";
        private const string InitializedKey = "BadgeCountsInitialized";
        private const int DefaultWishlistCount = 6;
        private const int DefaultCartCount = 3;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public Task<int> GetWishlistCountAsync() => Task.FromResult(GetCount(WishlistCountKey, DefaultWishlistCount));
        public Task<int> GetCartCountAsync() => Task.FromResult(GetCount(CartCountKey, DefaultCartCount));

        public Task IncrementWishlistCountAsync()
        {
            IncrementCount(WishlistCountKey, DefaultWishlistCount);
            return Task.CompletedTask;
        }

        public Task IncrementCartCountAsync()
        {
            IncrementCount(CartCountKey, DefaultCartCount);
            return Task.CompletedTask;
        }

        public Task DecrementWishlistCountAsync()
        {
            DecrementCount(WishlistCountKey, DefaultWishlistCount);
            return Task.CompletedTask;
        }

        public Task DecrementCartCountAsync()
        {
            DecrementCount(CartCountKey, DefaultCartCount);
            return Task.CompletedTask;
        }

        private int GetCount(string key, int defaultValue)
        {
            var session = GetSession();
            if (session == null)
            {
                return defaultValue;
            }

            EnsureInitialized(session);
            return session.GetInt32(key) ?? defaultValue;
        }

        private void IncrementCount(string key, int defaultValue)
        {
            var session = GetSession();
            if (session == null)
            {
                return;
            }

            EnsureInitialized(session);
            var current = session.GetInt32(key) ?? defaultValue;
            session.SetInt32(key, current + 1);
        }

        private void DecrementCount(string key, int defaultValue)
        {
            var session = GetSession();
            if (session == null)
            {
                return;
            }

            EnsureInitialized(session);
            var current = session.GetInt32(key) ?? defaultValue;
            var next = Math.Max(0, current - 1);
            session.SetInt32(key, next);
        }

        private void EnsureInitialized(ISession session)
        {
            if (session.GetInt32(InitializedKey) == 1)
            {
                return;
            }

            session.SetInt32(WishlistCountKey, DefaultWishlistCount);
            session.SetInt32(CartCountKey, DefaultCartCount);
            session.SetInt32(InitializedKey, 1);
        }

        private ISession? GetSession() => _httpContextAccessor.HttpContext?.Session;
    }
}
