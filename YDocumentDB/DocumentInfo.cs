using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YLR.YSystem.Organization;

namespace YLR.YDocumentDB
{
    /// <summary>
    /// 文档信息。
    /// </summary>
    public class DocumentInfo
    {
        /// <summary>
        /// 文档id。
        /// </summary>
        private int _id = -1;

        /// <summary>
        /// 目录id。
        /// </summary>
        public int id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <summary>
        /// 文档标题。
        /// </summary>
        private string _title = "";

        /// <summary>
        /// 文档标题。
        /// </summary>
        public string title
        {
            get { return this._title; }
            set { this._title = value; }
        }

        /// <summary>
        /// 创建时间。
        /// </summary>
        private DateTime _createTime;

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime createTime
        {
            get { return this._createTime; }
            set { this._createTime = value; }
        }

        /// <summary>
        /// 创建用户。
        /// </summary>
        private UserInfo _user = null;

        /// <summary>
        /// 创建用户。
        /// </summary>
        public UserInfo user
        {
            get { return this._user; }
            set { this._user = value; }
        }

        /// <summary>
        /// 目录id。
        /// </summary>
        private int _catalogId = -1;

        /// <summary>
        /// 目录id。
        /// </summary>
        public int catalogId
        {
            get { return this._catalogId; }
            set { this._catalogId = value; }
        }

        /// <summary>
        /// 文档纯文本内容。
        /// </summary>
        private string _text = "";

        /// <summary>
        /// 文档纯文本内容。
        /// </summary>
        public string text
        {
            get { return this._text; }
            set { this._text = value; }
        }

        /// <summary>
        /// 带格式的文档内容。
        /// </summary>
        private string _html = "";

        /// <summary>
        /// 带格式的文档内容。
        /// </summary>
        public string html
        {
            get { return this._html; }
            set { this._html = value; }
        }
    }
}
