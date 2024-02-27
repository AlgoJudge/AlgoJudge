namespace AlgoJudge.Server.Services
{
    public class PermissionService(
        ICurrentUserService currentUserService
        ) : IPermissionService
    {
        public bool CanCreateActivity()
        {
            if (!IsAuhenticated())
            {
                return false;
            }
            return true; // TODO
        }

        public bool CanListActivityInfo()
        {
            if (!IsAuhenticated())
            {
                return false;
            }
            return true; // TODO
        }

        private bool IsAuhenticated()
        {
            return currentUserService.Get() != null;
        }
    }
}
