using UnityEngine;
using System.Collections;
using System.IO;

namespace LSFunctions
{
	public class LSText {
		public static string Space(string str) {
			string returnStr = "";
			char[] strs = str.ToCharArray();
			for(var i = 0; i < str.Length; i++){
				if(strs[i] == '\\' && strs[i+1] == 'n') {
					returnStr += "\n";
					i++;
				} else if(strs[i] == '<' && strs[i+1] == 'b') {
					returnStr += "<b>";
					i+=2;
				} else if(strs[i] == '<' && strs[i+1] == '/' && strs[i+2] == 'b') {
					returnStr += "</b>";
					i+=3;
				} else {
					string tempStr = strs[i] + " ";
					tempStr = tempStr.ToUpper();
					returnStr += tempStr;
				}
			}
			returnStr = returnStr.Remove(returnStr.Length - 1);
			return returnStr;
		}
		
		public static string Space2(string str) {
			var returnStr = "";
			var strs = str.ToCharArray();
			for(var i = 0; i < str.Length; i++){
				returnStr += strs[i] + ' ';     
			}
			returnStr = returnStr.Remove(returnStr.Length - 2).ToUpper();
			return returnStr;
		}
		
		public static string ConvertToTimecode(float time) {
			float minutes = (time % (60 * 60)) / 60;
			float seconds = time % 60;
			string timeFormated = string.Format("{0:00}:{1:00.000}", minutes, seconds);
			return timeFormated;
		}
		
		public static string ConvertToPercent(float time, float total) {
			var timePrecent = (time/total * 100);
			string timePrecentFormated = string.Format("{0:000}%", timePrecent);
			return timePrecentFormated;
		}
	}

    static class LSMath {
	    public static Vector3 SphericalToCartesian(int radius, int polar) {
		    Vector3 outCart = new Vector3();
    	    outCart.x = radius * Mathf.Cos(Mathf.Deg2Rad * polar);
    	    outCart.y = radius * Mathf.Sin(Mathf.Deg2Rad * polar);
    	    return outCart;
	    }

	    public static float SuperLerp (float from, float to, float from2, float to2, float value) {
            if (from2 < to2) {
                if (value < from2)
                    value = from2;
                else if (value > to2)
                    value = to2;
            }
            else {
                if (value < to2)
                    value = to2;
                else if (value > from2)
                    value = from2; 
            }
            return (to - from) * ((value - from2) / (to2 - from2)) + from;
        }

        public static Rect ClampToScreen(Rect r)
        {
            r.x = Mathf.Clamp(r.x, 0, Screen.width - r.width);
            r.y = Mathf.Clamp(r.y, 0, Screen.height - r.height);
            return r;
        }
    }

    static class Screenshot
    {
        public static void TakeScreenshot() {
            string screenshotPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/screenshots";
            if (!Directory.Exists(screenshotPath))
            {
                Directory.CreateDirectory(screenshotPath);
            }
            Application.CaptureScreenshot(screenshotPath + "/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".png");

            Debug.Log("Took Screenshot! - " + screenshotPath + "/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".png");
        }
    }

    static class LSFile {

	    public static string GetFileURL (string filename) {
		    WWW wwwurl;
		    wwwurl = new WWW (filename);
		    return wwwurl.url;
	    }

        public static bool IsValidFile(string filename)
        {
		    bool IsValid = true;
		    string filePath= GetFileURL(filename);

		    if (!File.Exists(filePath))
			    IsValid = false;
		    else if (filePath == null || filePath == "")
			    IsValid = false;

		    return IsValid;
	    }

        public static string GetBeatmapMiscURL(string beatmapFileName, string filename)
        {
		    WWW wwwurl;
		    wwwurl = new WWW (Application.dataPath.Substring (0, Application.dataPath.LastIndexOf ("/")) + "/Beatmaps/" + beatmapFileName + "/" + filename);
		    return wwwurl.url;
	    }

        public static string GetBeatmapURL(string filename)
        {
		    WWW wwwurl;
		    wwwurl = new WWW (Application.dataPath.Substring (0, Application.dataPath.LastIndexOf ("/")) + "/Beatmaps/" + filename + "/" + filename + ".ls");
		    return wwwurl.url;
	    }

        public static bool IsValidBeatmapFile(string filename)
        {
		    bool IsValid = true;
		    string beatmapPath;

		    if(filename == "")
			    beatmapPath = GetBeatmapURL(PlayerPrefs.GetString("Selected Beatmap"));
		    else
			    beatmapPath = filename;

		    if (!File.Exists(beatmapPath))
			    IsValid = false;
		    else if (beatmapPath == null || beatmapPath == "")
			    IsValid = false;

		    return IsValid;
	    }

        public static string GetAudioURL(string filename)
        {
		    WWW wwwurl;
		    wwwurl = new WWW (Application.dataPath.Substring (0, Application.dataPath.LastIndexOf ("/")) + "/Beatmaps/" + filename + "/" + filename + ".wav");
		    return wwwurl.url;
	    }

        public static bool IsValidAudioFile(string filename)
        {
		    bool IsValid = true;
		    string audioPath;

		    if(filename == "")
			    audioPath = GetAudioURL(PlayerPrefs.GetString("Selected Beatmap"));
		    else
			    audioPath = filename;

		    if (!File.Exists(audioPath))
			    IsValid = false;
		    else if (audioPath == null || audioPath == "")
			    IsValid = false;

		    return IsValid;
	    }

        public static string GetSettingsURL(string filename)
        {
		    WWW wwwurl;
		    wwwurl = new WWW (Application.dataPath.Substring (0, Application.dataPath.LastIndexOf ("/")) + "/Misc Files/" + filename + ".ls");
		    return wwwurl.url;
	    }

        public static bool IsValidSettingsFile(string filename)
        {
		    bool IsValid = true;
		    string settingsPath;

		    if(filename == "")
			    settingsPath = GetSettingsURL("Settings");
		    else
			    settingsPath = GetSettingsURL(filename);

		    if (!File.Exists(settingsPath))
			    IsValid = false;
		    else if (settingsPath == null || settingsPath == "")
			    IsValid = false;

		    return IsValid;
	    }

        public static void WriteToFile(string path, string json)
        {
		    var sw = new System.IO.StreamWriter(path);
		    sw.Write(json);
		    sw.Flush();
		    sw.Close();
	    }
    }
}
