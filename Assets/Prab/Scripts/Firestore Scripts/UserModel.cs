using System;
using UnityEngine.UIElements;

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
        public string LatestLoggedDevice;
        public string LatestVisitedGame;
        public int GameVisits;
        public string LatestLoggedInDate;
        public int InteractionScore;
        public string LogoColor;
        public string Caption;
        public string CaptionColor;
        public string ChatEmbed;
        public LikesModel Likes;
        public string Avatar;
        public int ChatMessageSent;
        public TagModel Tag;
        // Game Options
        public int MusicVolume;
        public int SoundVolume;

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
#if UNITY_EDITOR
            LatestLoggedDevice = DeviceType.Test.ToString();
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
      LatestLoggedDevice = DeviceType.WebGL.ToString();
#endif
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR || UNITY_STANDALONE_WIN && !UNITY_EDITOR || UNITY_STANDALONE_LINUX && !UNITY_EDITOR
      LatestLoggedDevice = DeviceType.Desktop.ToString();
#endif
#if UNITY_IOS && !UNITY_EDITOR || UNITY_ANDROID && !UNITY_EDITOR
      LatestLoggedDevice = DeviceType.Mobile.ToString();
#endif
            LatestVisitedGame = "0";
            GameVisits = 0;
            LatestMessageReadDate = "0";
            InteractionScore = 0;
            Likes = new LikesModel();
            LogoColor = "#d17213";
            Caption = "";
            CaptionColor = "#ffffff";
            ChatEmbed = "true";
            Avatar = "Male/avatarm0";
            ChatMessageSent = 0;
            Tag = new TagModel();
            MusicVolume = 100;
            SoundVolume = 100;
        }


        public UserModel(UserModel oldModel, UserModel model)
        {
            Id = oldModel.Id;
            Username = oldModel.Username.ToLower();
            Email = oldModel.Email.ToLower();
            StartDate = oldModel.StartDate;
            LatestLoggedInDate = oldModel.LatestLoggedInDate;
#if UNITY_EDITOR
            LatestLoggedDevice = oldModel.LatestLoggedDevice;
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
      LatestLoggedDevice = DeviceType.WebGL.ToString();
#endif
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR || UNITY_STANDALONE_WIN && !UNITY_EDITOR || UNITY_STANDALONE_LINUX && !UNITY_EDITOR
      LatestLoggedDevice = DeviceType.Desktop.ToString();
#endif
#if UNITY_IOS && !UNITY_EDITOR || UNITY_ANDROID && !UNITY_EDITOR
      LatestLoggedDevice = DeviceType.Mobile.ToString();
#endif
            LatestVisitedGame = oldModel.LatestVisitedGame;
            GameVisits = oldModel.GameVisits;
            LatestMessageReadDate = oldModel.LatestMessageReadDate;
            InteractionScore = oldModel.InteractionScore;
            Likes = oldModel.Likes;
            LogoColor = oldModel.LogoColor;
            Caption = oldModel.Caption;
            CaptionColor = oldModel.CaptionColor;
            ChatEmbed = oldModel.ChatEmbed;
            Avatar = oldModel.Avatar;
            ChatMessageSent = oldModel.ChatMessageSent;
            Tag = oldModel.Tag;
            MusicVolume = model.MusicVolume;
            SoundVolume = model.SoundVolume;
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


    [Serializable]
    public class TagModel
    {
        public string TagEnabled;
        public string TagColor;
        public string TagImage;
        public string TagCaption;

        public TagModel()
        {
            TagEnabled = "false";
            TagCaption = "";
            TagColor = "";
            TagImage = "";
        }
    }
}