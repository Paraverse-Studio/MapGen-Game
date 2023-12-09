#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

// This script's purpose is to update data when building
// The function below updates the version name, and also updates it on the firestore db

class BuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        // This only updates the last part of the 3-part version name
        // It takes current month, day, hour, and minute, stripped of all leading 0s

        // First 2 parts of the version should still be manually overridden when 
        // deploying a major (first part) update, or a fix/update/balance update (2nd part)

        string existingVersion = Application.version;
        string newVersion = "0";

        int idx = existingVersion.LastIndexOf('.');

        if (idx != -1)
        {
            string ext = (DateTime.Now.Month % 10).ToString().TrimStart('0') + DateTime.Now.Day.ToString().TrimStart('0') +
                DateTime.Now.Hour.ToString().TrimStart('0');
            newVersion = ((DateTime.Now.Year % 10)-2) + "." + ((DateTime.Now.Month >= 10)? "1":"0") + "." + ext;
        }

        Debug.Log($"Existing version: {existingVersion}, moving to version: {newVersion}");

        PlayerSettings.bundleVersion = newVersion;
        PlayerSettings.Android.bundleVersionCode += 1;

        Debug.Log("MyCustomBuildProcessor.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
    }


}
#endif