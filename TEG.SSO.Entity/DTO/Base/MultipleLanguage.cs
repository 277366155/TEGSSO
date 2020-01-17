using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 多语言解析
    /// </summary>
    public class MultipleLanguage
    {
        /// <summary>
        /// 获取指定语言信息
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public string GetContent(Language lang)
        {
            if (this.en_US == null)
            {
                this.en_US = string.Empty;
            }
            switch (lang)
            {
                default:
                case Language.local_Lang:
                    if (string.IsNullOrWhiteSpace(local_Lang))
                    {
                        return this.en_US;
                    }
                    return this.local_Lang;
                case Language.en_US:
                    return this.en_US;
                //case Language.zh_CN:
                //    if (string.IsNullOrWhiteSpace(zh_CN))
                //    {
                //        return this.en_US;
                //    }
                //    return this.zh_CN;
            }
        }

        /// <summary>
        /// 本地语言
        /// </summary>
        public string local_Lang { get; set; }
        ///// <summary>
        ///// 中文
        ///// </summary>
        //public string zh_CN { get; set; }
        /// <summary>
        /// 英语
        /// </summary>
        public string en_US { get; set; }
    }
}
