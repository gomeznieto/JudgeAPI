namespace JudgeAPI.Models.User{
  public class UsersResponseDTO{
    public int TotalAmount { get; set; }
    public int TotalPerPage { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public List<UserDTO> Users { get; set; } = new();
  }
}
