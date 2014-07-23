using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Build.Framework;

namespace RealBuild.CopyInfrustructure
{
    public class UpdateInputs : ITask
    {
        public String SrcPath { get; set; }
        public String DstPath { get; set; }
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
        public bool Execute(IBuildEngine fake)
        {
            BuildEngine = fake;
            return Execute();
        }

        public bool Execute()
        {
            //Set DstPath to default 
            if (DstPath == null)
            {
                string curPath = Directory.GetCurrentDirectory();
                int pon = curPath.ToLower().IndexOf(@"\main\");
                if (pon > 0)
                {
                    String branchPath = curPath.Substring(0, pon);
                    if (!Directory.Exists(branchPath))
                    {
                        BuildEngine.LogMessageEvent(new BuildMessageEventArgs("Branch Path was found :" + branchPath, "Branch Path", "CopyInfrustructure", MessageImportance.High));
                        return false;
                    }
                    DstPath = branchPath + @"\Main\Inputs\RealSuite.Infrastructure\";
                }
            }
            // Set SrcPath to default
            if (SrcPath == null)
            {
                SrcPath = @"C:\RealSuite\TempBin\";
            }

            if (!Directory.Exists(SrcPath))
            {
                return false;
            }

            string[] dirEntries = Directory.GetDirectories(SrcPath);
            foreach (string subDir in dirEntries)
            {
                String dstDir = subDir.Replace(SrcPath, DstPath);
                if (!Directory.Exists(dstDir))
                {
                    Directory.CreateDirectory(dstDir);
                }
            }
            string[] fileEntries = Directory.GetFiles(SrcPath, "*.*", SearchOption.AllDirectories);
            foreach (string srcName in fileEntries)
            {
                String dstName = srcName.Replace(SrcPath, DstPath);
                if (File.Exists(dstName))
                {
                    DateTime srcTimeStamp = File.GetLastWriteTime(srcName);
                    DateTime dstTimeStamp = File.GetLastWriteTime(dstName);
                    if (DateTime.Compare(srcTimeStamp, dstTimeStamp) > 0)
                    {
                        FileAttributes attrs = File.GetAttributes(dstName);
                        if ((attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            File.SetAttributes(dstName, attrs & ~FileAttributes.ReadOnly);
                        }
                        File.Copy(srcName, dstName, true);
                    }
                }
                else
                {
                    File.Copy(srcName, dstName, true);
                }
            }
            return true;
        }
    }
}
