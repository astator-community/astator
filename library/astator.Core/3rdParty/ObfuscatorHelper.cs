using Obfuscar;
using System;
using System.IO;

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
        /// 生成输出目录
        /// </summary>
        public string OutputDir = string.Empty;
        /// <summary>
        /// 入口类名称
        /// </summary>
        public string EntryType = string.Empty;
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

            if (rules.OutputDir is null)
            {
                rules.OutputDir = Path.Combine(Path.GetDirectoryName(rules.DllPath), "obfuscator");
            }

            if (rules.EntryType is null)
            {
                ScriptLogger.Warn("未指定入口类名称!");
            }

            var xml =
                $@"<?xml version='1.0'?>
                    <Obfuscator>
                        <Var name = ""OutPath"" value = ""{rules.OutputDir}"" />
                        <Var name = ""RenameProperties"" value = ""{rules.RenameProperties}"" />
                        <Var name = ""RenameEvents"" value = ""{rules.RenameEvents}"" />
                        <Var name = ""RenameFields"" value = ""{rules.RenameFields}"" />
                        <Var name = ""KeepPublicApi"" value = ""{rules.KeepPublicApi}"" />
                        <Var name = ""HidePrivateApi"" value = ""{rules.HidePrivateApi}"" />
                        <Var name = ""ReuseNames"" value = ""{rules.ReuseNames}"" />
                        <Var name = ""UseUnicodeNames"" value = ""{rules.UseUnicodeNames}"" />
                        <Var name = ""UseKoreanNames"" value = ""{rules.UseKoreanNames}"" />
                        <Var name = ""HideStrings"" value = ""{rules.HideStrings}"" />
                        <Var name = ""OptimizeMethods"" value = ""{rules.OptimizeMethods}"" />
                        <AssemblySearchPath path=""{Android.App.Application.Context.GetExternalFilesDir("Sdk")}"" />
   
                        <Module file = ""{rules.DllPath}"">
                            <SkipMethod type=""{rules.EntryType}"" attrib = ""public"" rx = "".*"" />
                        </Module>
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
                ScriptLogger.Error(ex.Message);
                return false;
            }
        }
    }
}
