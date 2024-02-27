namespace AlgoJudge.Server.Utils
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException() : base("Access denied") { }
    }
}
