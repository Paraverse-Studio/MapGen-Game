using System;

namespace ParaverseWebsite.Models
{
  [Serializable]
  public class UserModel
  {
    public string Id;
    public string Username;
    public string Email;
    public string StartDate;
    public string LatestMessageReadDate;
    public string LatestLoggedInDate;
    public int InteractionScore;
    public string LogoColor;
    public string Caption;
    public string CaptionColor;
    public string ChatEmbed;
    public LikesModel Likes;
    public string Avatar;
    public int ChatMessageSent;

    public UserModel() { }


    public UserModel(
      string userId,
      string username,
      string email
      )
    {
      Id = userId;
      Username = username.ToLower();
      Email = email.ToLower();
      StartDate = DateTime.Today.ToString("MMMM dd, yy");
      LatestLoggedInDate = DateTime.Today.ToString("MMMM dd, yy");
      LatestMessageReadDate = "0";
      InteractionScore = 0;
      Likes = new LikesModel();
      LogoColor = "#d17213";
      Caption = "";
      CaptionColor = "#ffffff";
      ChatEmbed = "true";
      Avatar = "Male/avatarm0";
      ChatMessageSent = 0;
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