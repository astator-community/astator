using System;
using System.IO;
using astator.Core.Engine;
using astator.Core.Script;
using Obfuscar;

namespace astator.Core.ThirdParty
{
    /// <summary>
    /// 混淆规则
    /// </summary>
    public struct ObfuscatorRules
    {
        /// <summary>
        /// 待混淆程序集路径
        /// </summary>
        public string DllPath = string.Empty;

        /// <summary>
        /// 引用程序集路径
        /// </summary>
        public string AssemblySearchPath = string.Empty;

        /// <summary>
        /// 生成输出目录
        /// </summary>
        public string OutputDir = string.Empty;
        /// <summary>
        /// 是否重命名属性, 默认true
        /// </summary>
        public bool RenameProperties = true;
        /// <summary>
        /// 是否重命名事件, 默认true
        /// </summary>
        public bool RenameEvents = true;
        /// <summary>
        /// 是否重命名字段, 默认true
        /// </summary>
        public bool RenameFields = true;
        /// <summary>
        /// 是否从模糊处理中排除公共类型和类型成员, 默认false
        /// </summary>
        public bool KeepPublicApi = false;
        /// <summary>
        /// 是否包含模糊处理中的私有类型和类型成员, 默认true
        /// </summary>
        public bool HidePrivateApi = true;
        /// <summary>
        /// 是否重复使用混淆的名称, 默认true
        /// </summary>
        public bool ReuseNames = true;
        /// <summary>
        /// 是否使用 Unicode 字符作为混淆名称, 默认true
        /// </summary>
        public bool UseUnicodeNames = true;
        /// <summary>
        /// 是否使用韩文字作为模糊的名称, 默认false
        /// </summary>
        public bool UseKoreanNames = false;
        /// <summary>
        /// 是否隐藏字符串, 默认true
        /// </summary>
        public bool HideStrings = true;
        /// <summary>
        /// 是否优化方法, 默认true
        /// </summary>
        public bool OptimizeMethods = true;

        public ObfuscatorRules()
        {

        }

    }

    public static class ObfuscatorHelper
    {
        /// <summary>
        /// 执行混淆
        /// </summary>
        /// <param name="rules">混淆规则</param>
        /// <returns></returns>
        public static bool Execute(ObfuscatorRules rules)
        {
            if (rules.DllPath is null)
            {
                throw new Exception("dll路径不可为空");
            }

            if (!rules.DllPath.EndsWith(".dll"))
            {
                throw new Exception("dll路径不合法");
            }

            if (string.IsNullOrEmpty(rules.OutputDir))
            {
                rules.OutputDir = Path.Combine(Path.GetDirectoryName(rules.DllPath), "obfuscator");
            }

            var net6Dir = Path.Combine(SdkReferences.SdkDir, "net6.0");
            var mauiDir = Path.Combine(SdkReferences.SdkDir, "maui");

            var xml =
                $@"<?xml version='1.0'?>
                    <Obfuscator>
                        <Var name = ""OutPath"" value = ""{rules.OutputDir}"" />
                        <Var name = ""RenameProperties"" value = ""{rules.RenameProperties.ToString().ToLower()}"" />
                        <Var name = ""RenameEvents"" value = ""{rules.RenameEvents.ToString().ToLower()}"" />
                        <Var name = ""RenameFields"" value = ""{rules.RenameFields.ToString().ToLower()}"" />
                        <Var name = ""KeepPublicApi"" value = ""{rules.KeepPublicApi.ToString().ToLower()}"" />
                        <Var name = ""HidePrivateApi"" value = ""{rules.HidePrivateApi.ToString().ToLower()}"" />
                        <Var name = ""ReuseNames"" value = ""{rules.ReuseNames.ToString().ToLower()}"" />
                        <Var name = ""UseUnicodeNames"" value = ""{rules.UseUnicodeNames.ToString().ToLower()}"" />
                        <Var name = ""UseKoreanNames"" value = ""{rules.UseKoreanNames.ToString().ToLower()}"" />
                        <Var name = ""HideStrings"" value = ""{rules.HideStrings.ToString().ToLower()}"" />
                        <Var name = ""OptimizeMethods"" value = ""{rules.OptimizeMethods.ToString().ToLower()}"" />
                        <AssemblySearchPath path=""{net6Dir}"" />
                        <AssemblySearchPath path=""{mauiDir}"" />  
                        <AssemblySearchPath path=""{rules.AssemblySearchPath}"" />

                        <Module file = ""{rules.DllPath}""/>
                    </Obfuscator >
                     ";

            try
            {
                var obfuscator = Obfuscator.CreateFromXml(xml);
                obfuscator.RunRules();
                obfuscator.SaveAssemblies();
                return true;
            }
            catch (Exception ex)
            {
                ScriptLogger.Error(ex);
                return false;
            }
        }
    }
}
