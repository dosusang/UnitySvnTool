using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
 
namespace Assets.Editor
{
    public class EditorUtil
    {
        private static string _svnPath = "";

        // 填写svn安装路径
        private static string svnPath = @"d:\commonTools\svn\bin\";
        private static string svnProc = @"TortoiseProc.exe";
        
        private static string workingPath = Application.dataPath.Replace("/Assets", "");
 
        private static string GetSvnProcPath()
        {
            if (_svnPath != string.Empty)
            {
                return _svnPath;
            }

            var path = string.Concat(svnPath, svnProc);
            if (File.Exists(path))
            {
                _svnPath = path;
            }

            if (_svnPath == string.Empty)
            {
                _svnPath = EditorUtility.OpenFilePanel("选择SVN工具： TortoiseProc.exe", "c:\\", "exe");
            }
            return _svnPath;
        }

        public static void ExecuteProcess(string filePath, string command, string workPath = "", int seconds = 0)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            Process process = new Process();//创建进程对象
            process.StartInfo.WorkingDirectory = workPath;
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = command;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = false;//不重定向输出
            try
            {
                if (process.Start())
                {
                    if (seconds == 0)
                    {
                        process.WaitForExit(); //无限等待进程结束
                    }
                    else
                    {
                        process.WaitForExit(seconds); //等待毫秒
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                process.Close();
            }
        }
        
        public static void ProcessSvnCommand(string command)
        {
            Debug.Log($"command: {command}");
            ExecuteProcess(GetSvnProcPath(), command, workingPath);
        }
        
        [MenuItem("Assets/SVN/删除并拉取svn")]
        public static void DelAndUpdateSvnFile()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            ProcessSvnCommand(string.Format("/command:update /path:{0} /closeonend:3", path));
        }
 
        [MenuItem("Assets/SVN/回滚")]
        public static void RevertSvnDirectory()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            ProcessSvnCommand(string.Format("/command:revert /path:{0} /closeonend:3", path));
        }
        
        [MenuItem("Assets/SVN/递归回滚")]
        public static void RevertRDirectory()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            ProcessSvnCommand(string.Format("/command:revert -r /path:{0} /closeonend:3", path));
        }
        
        [MenuItem("Assets/SVN/更新", false, priority = 0)]
        public static void UpdateSvnDirectory()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            ProcessSvnCommand( string.Format("/command:update /path:{0} /closeonend:3", path));
        }
        
        [MenuItem("Assets/SVN/提交", false, priority = 1)]
        public static void CommitSvnDirectory()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            ProcessSvnCommand(string.Format("/command:commit /path:{0} /closeonend:3", path));
        }
    }
}