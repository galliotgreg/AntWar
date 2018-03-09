﻿using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Ionic.Zip;
using System.Text;
using System.IO;

namespace Zip_Tool
{
    public class ZipUtil
    {
#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void unzip (string zipFilePath, string location);

	[DllImport("__Internal")]
	private static extern void zip (string zipFilePath);

	[DllImport("__Internal")]
	private static extern void addZipFile (string addFile);

#endif

        public static void Unzip(string zipFilePath, string location)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            Directory.CreateDirectory(location);

            using (ZipFile zip = ZipFile.Read(zipFilePath))
            {

                zip.ExtractAll(location, ExtractExistingFileAction.OverwriteSilently);
            }

#elif UNITY_ANDROID
		using (AndroidJavaClass zipper = new AndroidJavaClass ("com.tsw.zipper")) {
			zipper.CallStatic ("unzip", zipFilePath, location);
		}
#elif UNITY_IPHONE
		unzip (zipFilePath, location);
#endif
        }

        public static void Zip(string path, params string[] files)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            using (ZipFile zip = new ZipFile())
            {
                foreach (string file in files)
                {
                    zip.AddFile(file, "");
                }
                zip.Save(path + ".zip");
            }
#elif UNITY_ANDROID
		using (AndroidJavaClass zipper = new AndroidJavaClass ("com.tsw.zipper")) {
			{
				zipper.CallStatic ("zip", zipFileName, files);
			}
		}
#elif UNITY_IPHONE
		foreach (string file in files) {
			addZipFile (file);
		}
		zip (zipFileName);
#endif
        }
    }
}

