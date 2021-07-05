namespace API.Helpers
{
    public class UsersParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public int pageSize{
            get => _pageSize;
            set => _pageSize = (value >MaxPageSize)? MaxPageSize : value;
        }

        public string CurrentUserName { get; set; }
        public string Gender { get; set; }
        
    }
}