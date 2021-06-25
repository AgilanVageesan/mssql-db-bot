namespace ssms_db_bot.Entity
{
    public class JobRequest
    {
        public string DbServer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string JobName { get; set; }
    }
}
