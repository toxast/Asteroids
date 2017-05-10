using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Logger : MonoBehaviour {

    public static void Log(string msg) {
		Singleton<Logger>.inst.LogInternal(msg);
    }

    StreamWriter file;
    private void LogInternal(string msg) {
        if (file == null) {
            string path;
            if (Application.isEditor) {
                path = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\..\"));
                path = Path.Combine(path, "mylog.txt");
            } else {
                path = Path.Combine(Application.dataPath, "mylog.txt");
            }
			Debug.LogWarning ("logging at: " + path);
            file = new StreamWriter(path, true);
        }
		msg = DateTime.Now.ToLongTimeString() + " " + msg;
        Debug.Log(msg);
        file.WriteLine(msg);
    }

    private void OnApplicationPause(bool pause) {
        if (pause) {
            LogInternal("pause");
            CloseFileIfOpened();
        }
    }

    private void OnApplicationQuit() {
        LogInternal("quit");
        CloseFileIfOpened();
    }

    void CloseFileIfOpened() {
        if (file != null) {
            file.Close();
            file = null;
        }
    }

}
