using System;

namespace ParaverseWebsite.Models
{
  [Serializable]
  public class UserModel
  {
    public string Username;
    public string Password;
    public string Email;
    public string StartDate;
    public int InteractionScore;
    public string LogoColor;
    public string Caption;
    public string CaptionColor;
    public string ChatEmbed;
    public LikesModel Likes;

    public UserModel() { }


    public UserModel(
      string username,
      string email,
      string password
      )
    {
      Username = username;
      Email = email;
      Password = password;
      StartDate = DateTime.Today.ToString("MMMM dd, yy");
      InteractionScore = 0;
      Likes = new LikesModel();
      LogoColor = "#d17213";
      Caption = "";
      CaptionColor = "#ffffff";
      ChatEmbed = "true";
    }
  }

  [Serializable]
  public class LikesModel
  {
    public int TotalLiked;
    public int LikedToday;
    public string TodayDate;
    public LikesModel() 
    {
      TotalLiked = 0;
      LikedToday = 0;
      TodayDate = DateTime.Today.ToString("MMMM dd, yy");
    }
  }
}