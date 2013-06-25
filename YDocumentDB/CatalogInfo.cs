using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YLR.YSystem.Organization;

namespace YLR.YDocumentDB
{
    /// <summary>
    /// 目录信息类。
    /// 作者：董帅 创建时间：2013-6-25 21:19:37
    /// </summary>
    public class CatalogInfo
    {
        /// <summary>
        /// 目录id。
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
        /// 目录名称。
        /// </summary>
        private string _name = "";

        /// <summary>
        /// 目录名称。
        /// </summary>
        public string name
        {
            get { return this._name; }
            set { this._name = value; }
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
        /// 父目录id。
        /// </summary>
        private int _parentId = -1;

        /// <summary>
        /// 父目录id。
        /// </summary>
        public int parentId
        {
            get { return this._parentId; }
            set { this._parentId = value; }
        }
    }
}
