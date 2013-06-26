using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YLR.YAdoNet;
using System.Data;

namespace YLR.YDocumentDB
{
    /// <summary>
    /// 文档数据库操作类。
    /// 作者：董帅 创建时间：2013-6-26 10:37:47
    /// </summary>
    public class DocOper
    {
        /// <summary>
        /// 文档数据库。
        /// </summary>
        private YDataBase _docDataBase = null;

        /// <summary>
        /// 文档数据库。
        /// </summary>
        public YDataBase docDataBase
        {
            get { return this._docDataBase; }
            set { this._docDataBase = value; }
        }

        /// <summary>
        /// 错误信息。
        /// </summary>
        protected string _errorMessage = "";

        /// <summary>
        /// 错误信息。
        /// </summary>
        public string errorMessage
        {
            get
            {
                return this._errorMessage;
            }
        }

        /// <summary>
        /// 创建文档数据库操作对象。
        /// 作者：董帅 创建时间：2013-6-26 10:46:28
        /// </summary>
        /// <param name="configFilePath">配置文件路径。</param>
        /// <param name="nodeName">节点名。</param>
        /// <param name="key">解密密码。</param>
        /// <returns>成功返回操作对象，否则返回null。</returns>
        public static DocOper createDocOper(string configFilePath, string nodeName, string key)
        {
            DocOper docOper = null;

            //获取数据库实例。
            YDataBase orgDb = YDataBaseConfigFile.createDataBase(configFilePath, nodeName, key);

            if (orgDb != null)
            {
                docOper = new DocOper();
                docOper.docDataBase = orgDb;
            }
            else
            {
                Exception ex = new Exception("创建数据库实例失败！");
                throw ex;
            }

            return docOper;
        }

        /// <summary>
        /// 创建新目录。
        /// </summary>
        /// <param name="catalog">目录信息，目录名称长度在1~200，顶层目录父id为-1。</param>
        /// <returns>成功返回目录id，失败返回-1。</returns>
        public int createNewCatalog(CatalogInfo catalog)
        {
            int catalogId = -1; //创建的组织机构id。

            try
            {
                if (catalog == null)
                {
                    this._errorMessage = "不能插入空目录！";
                }
                else if (string.IsNullOrEmpty(catalog.name) || catalog.name.Length > 200)
                {
                    this._errorMessage = "目录名称不合法！";
                }
                else
                {
                    //存入数据库
                    if (this._docDataBase.connectDataBase())
                    {
                        //新增数据
                        string sql = "";
                        YParameters par = new YParameters();
                        par.add("@catalogName", catalog.name);
                        par.add("@userId", catalog.user.id);
                        par.add("@parentId", catalog.parentId);
                        if (catalog.parentId == -1)
                        {
                            switch (this._docDataBase.databaseType)
                            {
                                case DataBaseType.MSSQL:
                                case DataBaseType.SQL2000:
                                case DataBaseType.SQL2005:
                                case DataBaseType.SQL2008:
                                    {
                                        sql = "INSERT INTO DOC_CATALOG (NAME,USERID) VALUES (@catalogName,@userId) SELECT SCOPE_IDENTITY() AS id";
                                        break;
                                    }
                                case DataBaseType.SQLite:
                                    {
                                        sql = "INSERT INTO DOC_CATALOG (NAME,USERID) VALUES (@catalogName,@userId);SELECT LAST_INSERT_ROWID() AS id;";
                                        break;
                                    }
                                default:
                                    {
                                        sql = "INSERT INTO DOC_CATALOG (NAME,USERID) VALUES (@catalogName,@userId) SELECT SCOPE_IDENTITY() AS id";
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (this._docDataBase.databaseType)
                            {
                                case DataBaseType.MSSQL:
                                case DataBaseType.SQL2000:
                                case DataBaseType.SQL2005:
                                case DataBaseType.SQL2008:
                                    {
                                        sql = "INSERT INTO DOC_CATALOG (NAME,USERID,PARENTID) VALUES (@catalogName,@parentId,@userId) SELECT SCOPE_IDENTITY() AS id";
                                        break;
                                    }
                                case DataBaseType.SQLite:
                                    {
                                        sql = "INSERT INTO DOC_CATALOG (NAME,USERID,PARENTID,VALUE) VALUES (@catalogName,@parentId,@userId);SELECT LAST_INSERT_ROWID() AS id;";
                                        break;
                                    }
                                default:
                                    {
                                        sql = "INSERT INTO DOC_CATALOG (NAME,USERID,PARENTID) VALUES (@catalogName,@parentId,@userId) SELECT SCOPE_IDENTITY() AS id";
                                        break;
                                    }
                            }
                        }

                        DataTable retDt = this._docDataBase.executeSqlReturnDt(sql, par);
                        if (retDt != null && retDt.Rows.Count > 0)
                        {
                            //获取组织机构id
                            catalogId = Convert.ToInt32(retDt.Rows[0]["id"]);
                        }
                        else
                        {
                            this._errorMessage = "创建目录失败！";
                            if (retDt == null)
                            {
                                this._errorMessage += "错误信息[" + this._docDataBase.errorText + "]";
                            }
                        }
                    }
                    else
                    {
                        this._errorMessage = "连接数据库出错！错误信息[" + this._docDataBase.errorText + "]";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //断开数据库连接。
                this._docDataBase.disconnectDataBase();
            }

            return catalogId;
        }
    }
}
