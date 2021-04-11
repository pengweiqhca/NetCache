using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetCache
{
    /// <summary>
    /// 表示插入代理IL任务
    /// </summary>
    public class ProxyTask : Task
    {
        /// <summary>
        /// 插入代理的程序集
        /// </summary>
        [Required]
        public string TargetAssembly { get; set; } = default!;

        /// <summary>
        /// 引用的程序集
        /// 逗号分隔
        /// </summary>
        public ITaskItem[]? References { get; set; }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var logger = new Logger(Log);
            if (File.Exists(TargetAssembly) == false)
            {
                logger.Message($"找不到文件编译输出的程序集{TargetAssembly}");

                return true;
            }

            logger.Message(GetType().AssemblyQualifiedName);

            try
            {
                using var assembly = new CacheAssembly(TargetAssembly, GetSearchDirectories().Distinct(), logger, true, true);

                assembly.WriteProxyTypes();

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());

                return false;
            }
        }

        /// <summary>
        /// 返回依赖搜索目录
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetSearchDirectories()
        {
            if (References == null) yield break;

            foreach (var item in References)
            {
                if (string.IsNullOrEmpty(item.ItemSpec)) continue;

                var path = Path.GetDirectoryName(item.ItemSpec);

                if (path != null && Directory.Exists(path)) yield return path;
            }
        }
    }
}
