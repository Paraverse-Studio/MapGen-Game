using System;

namespace ParaverseWebsite.Models
{
  [Serializable]
  public class UserModel
  {
    public string I_Username;
    public string I_Password;
    public string I_Email;
    public string I_StartDate;
    public int S_ParaverseScore;
    public int S_InteractionScore;
    public string S_LengthOfAccount;
    public string P_LogoColor;
    public string P_Caption;
    public string P_CaptionColor;

    public UserModel() { }
  }
}