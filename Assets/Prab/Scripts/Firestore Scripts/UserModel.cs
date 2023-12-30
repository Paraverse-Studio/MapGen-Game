using Firebase.Firestore;

namespace ParaverseWebsite.Models
{
  [FirestoreData]
  public class UserModel
  {
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public string? I_Username { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public string? I_Password { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public string? I_Email { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public string? I_StartDate { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public int? S_ParaverseScore { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public int? S_InteractionScore { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public string? S_LengthOfAccount { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public string? P_LogoColor { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public string? P_Caption { get; set; }
#if !UNITY_WEBGL || UNITY_EDITOR
    [FirestoreProperty]
#endif
    public string? P_CaptionColor { get; set; }
  }
}