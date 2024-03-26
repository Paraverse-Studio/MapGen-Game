using ParaverseWebsite.Models;
using UnityEngine;

public class SaveOptionsData : MonoBehaviour
{
    public void SaveOptions()
    {
        UserDatabaseHandler(AudioManager.Instance.MusicVolume, AudioManager.Instance.SoundVolume);
    }

    private void UserDatabaseHandler(int musicVolume, int soundVolume)
    {
        // get user id and use it to get leaderboards of that user
        UserModel userModel = new UserModel();
        userModel.MusicVolume = musicVolume;
        userModel.SoundVolume = soundVolume;
        UserModel updatedUserModel = new UserModel();

        FirebaseDatabaseManager.Instance.GetUser(MainMenuController.Instance.Username,
          //  IF USER IS FOUND!!
          (updatedUserModel) => UpdateUser(updatedUserModel, userModel),
          //  IF USER IS NOT FOUND!!
          () => Debug.Log($"User not found! Options were not saved"));
    }

    /// <summary>
    /// Runs if leaderboards already exists for user
    /// </summary>
    /// <param name="oldAchievementsModel"></param>
    /// <param name="sessionDataModel"></param>
    private void UpdateUser(UserModel oldModel, UserModel model)
    {
        // Create updated leaderboards
        UserModel updatedUserModel = new UserModel(oldModel, model);

        // Updates previous leaderboards entry into database
        FirebaseDatabaseManager.Instance.PostUser(updatedUserModel, (updatedUserModel) => Debug.Log("User Updated Successfully!"));
    }
}
