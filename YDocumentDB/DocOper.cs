using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YLR.YAdoNet;
using System.Data;
using YLR.YSystem.Organization;

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
                                        sql = "INSERT INTO DOC_CATALOG (NAME,USERID,PARENTID) VALUES (@catalogName,@userId,@parentId) SELECT SCOPE_IDENTITY() AS id";
                                        break;
                                    }
                                case DataBaseType.SQLite:
                                    {
                                        sql = "INSERT INTO DOC_CATALOG (NAME,USERID,PARENTID,VALUE) VALUES (@catalogName,@userId,@parentId);SELECT LAST_INSERT_ROWID() AS id;";
                                        break;
                                    }
                                default:
                                    {
                                        sql = "INSERT INTO DOC_CATALOG (NAME,USERID,PARENTID) VALUES (@catalogName,@userId,@parentId) SELECT SCOPE_IDENTITY() AS id";
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

        /// <summary>
        /// 通过DataRow数据构建目录对象。
        /// 作者：董帅 创建时间：2013-6-26 13:56:31
        /// </summary>
        /// <param name="r">数据行。</param>
        /// <returns>成功返回对象，失败返回null。</returns>
        private CatalogInfo getGatalogFromDataRow(DataRow r)
        {
            CatalogInfo catalog = null;

            if (r != null)
            {
                catalog = new CatalogInfo();
                //菜单id不能为null，否则返回失败。
                if (!r.IsNull("ID"))
                {
                    catalog.id = Convert.ToInt32(r["ID"]);
                }
                else
                {
                    return null;
                }

                if (!r.IsNull("NAME"))
                {
                    catalog.name = r["NAME"].ToString();
                }

                if (!r.IsNull("PARENTID"))
                {
                    catalog.parentId = Convert.ToInt32(r["PARENTID"]);
                }
                else
                {
                    catalog.parentId = -1;
                }

                if (!r.IsNull("CREATETIME"))
                {
                    catalog.createTime = Convert.ToDateTime(r["CREATETIME"]);
                }

                if (!r.IsNull("USERID"))
                {
                    int userId = Convert.ToInt32(r["USERID"]);
                    //获取用户信息。
                    if (userId > 0)
                    {
                        OrgOperater orgOper = new OrgOperater();
                        catalog.user = orgOper.getUser(userId, this._docDataBase);
                    }
                }
            }

            return catalog;
        }

        /// <summary>
        /// 获取指定的目录列表。
        /// 作者：董帅 创建时间：2013-6-26 14:17:53
        /// </summary>
        /// <param name="id">目录id。</param>
        /// <returns>成功返回目录列表，出错返回null。</returns>
        public CatalogInfo getGatalog(int id)
        {
            CatalogInfo catalog = null;

            try
            {
                if (this._docDataBase != null)
                {
                    //连接数据库
                    if (this._docDataBase.connectDataBase())
                    {

                        //sql语句，获取所有字典
                        string sql = "";
                        YParameters par = new YParameters();
                        par.add("@id", id);
                        sql = "SELECT * FROM DOC_CATALOG WHERE ID = @id";
                        
                        //获取数据
                        DataTable dt = this._docDataBase.executeSqlReturnDt(sql, par);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            catalog = this.getGatalogFromDataRow(dt.Rows[0]);

                            //获取用户信息。
                            if (catalog != null && catalog.user.id > 0)
                            {
                                OrgOperater orgOper = new OrgOperater();
                                catalog.user = orgOper.getUser(catalog.user.id, this._docDataBase);
                            }
                        }
                        else
                        {
                            this._errorMessage = "获取数据失败！错误信息：[" + this._docDataBase.errorText + "]";
                        }
                    }
                    else
                    {
                        this._errorMessage = "连接数据库失败！错误信息：[" + this._docDataBase.errorText + "]";
                    }
                }
                else
                {
                    this._errorMessage = "未设置数据库实例！";
                }
            }
            catch (Exception ex)
            {
                this._errorMessage = ex.Message;
            }
            finally
            {
                this._docDataBase.disconnectDataBase();
            }

            return catalog;
        }

        /// <summary>
        /// 获取指定父id的目录列表。
        /// 作者：董帅 创建时间：2013-6-26 13:51:59
        /// </summary>
        /// <param name="pId">父id，顶级目录为-1。</param>
        /// <returns>成功返回目录列表，出错返回null。</returns>
        public List<CatalogInfo> getGatalogsByParentId(int pId)
        {
            List<CatalogInfo> catalogs = null;

            try
            {
                if (this._docDataBase != null)
                {
                    //连接数据库
                    if (this._docDataBase.connectDataBase())
                    {

                        //sql语句，获取所有字典
                        string sql = "";
                        YParameters par = new YParameters();
                        par.add("@parentId", pId);
                        if (pId == -1)
                        {
                            sql = "SELECT * FROM DOC_CATALOG WHERE PARENTID IS NULL";
                        }
                        else
                        {
                            sql = "SELECT * FROM DOC_CATALOG WHERE PARENTID = @parentId";
                        }
                        //获取数据
                        DataTable dt = this._docDataBase.executeSqlReturnDt(sql, par);
                        if (dt != null)
                        {
                            catalogs = new List<CatalogInfo>();
                            foreach (DataRow r in dt.Rows)
                            {
                                CatalogInfo c = this.getGatalogFromDataRow(r);

                                

                                if (c != null)
                                {
                                    catalogs.Add(c);
                                }
                            }
                        }
                        else
                        {
                            this._errorMessage = "获取数据失败！错误信息：[" + this._docDataBase.errorText + "]";
                        }
                    }
                    else
                    {
                        this._errorMessage = "连接数据库失败！错误信息：[" + this._docDataBase.errorText + "]";
                    }
                }
                else
                {
                    this._errorMessage = "未设置数据库实例！";
                }
            }
            catch (Exception ex)
            {
                this._errorMessage = ex.Message;
            }
            finally
            {
                this._docDataBase.disconnectDataBase();
            }

            return catalogs;
        }

        /// <summary>
        /// 修改指定目录的信息，通过目录id匹配。
        /// </summary>
        /// <param name="catalog">要修改的字典项。</param>
        /// <returns>成功返回true，否则返回false。</returns>
        public bool changeCatalog(CatalogInfo catalog)
        {
            bool bRet = false; //返回值

            try
            {
                if (this._docDataBase != null)
                {
                    //连接数据库
                    if (this._docDataBase.connectDataBase())
                    {
                        //sql语句
                        string sql = "";
                        YParameters par = new YParameters();
                        par.add("@catalogId", catalog.id);
                        par.add("@catalogName", catalog.name);

                        sql = "UPDATE DOC_CATALOG SET NAME = @catalogName WHERE ID = @catalogId";

                        int retCount = this._docDataBase.executeSqlWithOutDs(sql, par);
                        if (retCount == 1)
                        {
                            bRet = true;
                        }
                        else
                        {
                            this._errorMessage = "更新数据失败！";
                            if (retCount != 1)
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
                else
                {
                    this._errorMessage = "未设置数据库实例！";
                }
            }
            catch (Exception ex)
            {
                this._errorMessage = ex.Message;
            }
            finally
            {
                this._docDataBase.disconnectDataBase();
            }

            return bRet;
        }
    }
}
