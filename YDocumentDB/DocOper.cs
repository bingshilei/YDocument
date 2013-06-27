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
                        catalogs = this.getGatalogsByParentId(pId, this._docDataBase);
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
        /// 获取指定父id的目录列表。
        /// 作者：董帅 创建时间：2013-6-26 13:51:59
        /// </summary>
        /// <param name="pId">父id，顶级目录为-1。</param>
        /// <param name="db">数据库连接</param>
        /// <returns>成功返回目录列表，出错返回null。</returns>
        public List<CatalogInfo> getGatalogsByParentId(int pId,YDataBase db)
        {
            List<CatalogInfo> catalogs = null;

            try
            {
                if (db != null)
                {
                    //sql语句，获取所有字典
                    string sql = "";
                    YParameters par = new YParameters();
                    par.add("@parentId", pId);
                    if (pId == -1)
                    {
                        sql = "SELECT * FROM DOC_CATALOG WHERE PARENTID IS NULL ORDER BY CREATETIME ASC";
                    }
                    else
                    {
                        sql = "SELECT * FROM DOC_CATALOG WHERE PARENTID = @parentId ORDER BY CREATETIME ASC";
                    }
                    //获取数据
                    DataTable dt = db.executeSqlReturnDt(sql, par);
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
                        this._errorMessage = "获取数据失败！错误信息：[" + db.errorText + "]";
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

        /// <summary>
        /// 删除目录，删除时，连同子目录和文档一并删除。
        /// 作者：董帅 创建时间：2012-8-28 22:20:05
        /// </summary>
        /// <param name="catalogIds">字典项id</param>
        /// <returns>成功返回true，否则返回false。</returns>
        public bool deleteCatalogs(int[] catalogIds)
        {
            bool bRet = true;
            try
            {
                //连接数据库
                if (this._docDataBase.connectDataBase())
                {
                    this._docDataBase.beginTransaction(); //开启事务

                    //删除字典项
                    foreach (int i in catalogIds)
                    {
                        if (this.deleteCatalogsByParentId(i,this._docDataBase))
                        {
                            //删除当前机构
                            string sql = "DELETE FROM DOC_CATALOG WHERE ID = @id";
                            YParameters par = new YParameters();
                            par.add("@id", i);
                            if (this._docDataBase.executeSqlWithOutDs(sql, par) < 0)
                            {
                                bRet = false;
                                break;
                            }
                        }
                        else
                        {
                            bRet = false;
                            break;
                        }
                    }
                }
                else
                {
                    this._errorMessage = "连接数据库出错！错误信息[" + this._docDataBase.errorText + "]";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (bRet)
                {
                    //提交
                    this._docDataBase.commitTransaction();
                }
                else
                {
                    //回滚
                    this._docDataBase.rollbackTransaction();
                }
                this._docDataBase.disconnectDataBase();
            }

            return bRet;
        }

        /// <summary>
        /// 删除指定父id的目录。
        /// 作者：董帅 创建时间：2013-6-27 23:32:32
        /// </summary>
        /// <param name="catalogId">目录id</param>
        /// <param name="db">数据库连接</param>
        /// <returns>成功返回true，否则返回false。</returns>
        public bool deleteCatalogsByParentId(int catalogId, YDataBase db)
        {
            bool bRet = true;
            try
            {
                //删除文档
                if (this.deleteDocumentsByCatalogId(catalogId, db))
                {
                    //删除子目录
                    List<CatalogInfo> catalogs = this.getGatalogsByParentId(catalogId, db);
                    int i = 0;
                    for (i = 0;i < catalogs.Count;i++)
                    {
                        if (!this.deleteCatalogsByParentId(catalogs[i].id, db))
                        {
                            break;
                        }
                    }

                    if (i == catalogs.Count)
                    {
                        YParameters par = new YParameters();
                        par.add("@catalogId", catalogId);
                        string sql = "DELETE FROM DOC_CATALOG WHERE PARENTID = @catalogId";

                        if (db.executeSqlWithOutDs(sql, par) < 0)
                        {
                            bRet = false;
                        }
                    }
                    else
                    {
                        bRet = false;
                    }
                }
                else
                {
                    bRet = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bRet;
        }

        /// <summary>
        /// 创建新文档。
        /// </summary>
        /// <param name="document">文档信息，文档名称长度在1~200，顶层文档目录id为-1。</param>
        /// <returns>成功返回文档id，失败返回-1。</returns>
        public int createNewDocument(DocumentInfo document)
        {
            int documentId = -1; //创建的文档id。

            try
            {
                if (document == null)
                {
                    this._errorMessage = "必须输入文档标题！";
                }
                else if (string.IsNullOrEmpty(document.title) || document.title.Length > 200)
                {
                    this._errorMessage = "文档标题不合法！";
                }
                else
                {
                    //存入数据库
                    if (this._docDataBase.connectDataBase())
                    {
                        //新增数据
                        string sql = "";
                        YParameters par = new YParameters();
                        par.add("@documentTitle", document.title);
                        par.add("@userId", document.user.id);
                        par.add("@catalogId", document.catalogId);
                        if (document.catalogId == -1)
                        {
                            switch (this._docDataBase.databaseType)
                            {
                                case DataBaseType.MSSQL:
                                case DataBaseType.SQL2000:
                                case DataBaseType.SQL2005:
                                case DataBaseType.SQL2008:
                                    {
                                        sql = "INSERT INTO DOC_DOCUMENT (TITLE,USERID) VALUES (@documentTitle,@userId) SELECT SCOPE_IDENTITY() AS id";
                                        break;
                                    }
                                case DataBaseType.SQLite:
                                    {
                                        sql = "INSERT INTO DOC_DOCUMENT (TITLE,USERID) VALUES (@documentTitle,@userId);SELECT LAST_INSERT_ROWID() AS id;";
                                        break;
                                    }
                                default:
                                    {
                                        sql = "INSERT INTO DOC_DOCUMENT (TITLE,USERID) VALUES (@documentTitle,@userId) SELECT SCOPE_IDENTITY() AS id";
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
                                        sql = "INSERT INTO DOC_DOCUMENT (TITLE,USERID,CATALOGID) VALUES (@documentTitle,@userId,@catalogId) SELECT SCOPE_IDENTITY() AS id";
                                        break;
                                    }
                                case DataBaseType.SQLite:
                                    {
                                        sql = "INSERT INTO DOC_DOCUMENT (TITLE,USERID,CATALOGID,VALUE) VALUES (@documentTitle,@userId,@catalogId);SELECT LAST_INSERT_ROWID() AS id;";
                                        break;
                                    }
                                default:
                                    {
                                        sql = "INSERT INTO DOC_DOCUMENT (TITLE,USERID,CATALOGID) VALUES (@documentTitle,@userId,@catalogId) SELECT SCOPE_IDENTITY() AS id";
                                        break;
                                    }
                            }
                        }

                        DataTable retDt = this._docDataBase.executeSqlReturnDt(sql, par);
                        if (retDt != null && retDt.Rows.Count > 0)
                        {
                            //获文档id
                            documentId = Convert.ToInt32(retDt.Rows[0]["id"]);
                        }
                        else
                        {
                            this._errorMessage = "创建文档失败！";
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

            return documentId;
        }

        /// <summary>
        /// 通过DataRow数据构建文档对象。
        /// 作者：董帅 创建时间：2013-6-27 15:27:35
        /// </summary>
        /// <param name="r">数据行。</param>
        /// <returns>成功返回对象，失败返回null。</returns>
        private DocumentInfo getDocumentFromDataRow(DataRow r)
        {
            DocumentInfo document = null;

            if (r != null)
            {
                document = new DocumentInfo();
                //菜单id不能为null，否则返回失败。
                if (!r.IsNull("ID"))
                {
                    document.id = Convert.ToInt32(r["ID"]);
                }
                else
                {
                    return null;
                }

                if (!r.IsNull("TITLE"))
                {
                    document.title = r["TITLE"].ToString();
                }

                if (!r.IsNull("CATALOGID"))
                {
                    document.catalogId = Convert.ToInt32(r["CATALOGID"]);
                }
                else
                {
                    document.catalogId = -1;
                }

                if (!r.IsNull("CREATETIME"))
                {
                    document.createTime = Convert.ToDateTime(r["CREATETIME"]);
                }

                if (!r.IsNull("USERID"))
                {
                    int userId = Convert.ToInt32(r["USERID"]);
                    //获取用户信息。
                    if (userId > 0)
                    {
                        OrgOperater orgOper = new OrgOperater();
                        document.user = orgOper.getUser(userId, this._docDataBase);
                    }
                }
            }

            return document;
        }

        /// <summary>
        /// 获取指定的文档信息。
        /// 作者：董帅 创建时间：2013-6-27 17:10:32
        /// </summary>
        /// <param name="id">文档id。</param>
        /// <returns>成功返回文档信息，出错返回null。</returns>
        public DocumentInfo getDocument(int id)
        {
            DocumentInfo document = null;

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
                        par.add("@id", id);
                        sql = "SELECT * FROM DOC_DOCUMENT WHERE ID = @id";

                        //获取数据
                        DataTable dt = this._docDataBase.executeSqlReturnDt(sql, par);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            document = this.getDocumentFromDataRow(dt.Rows[0]);
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

            return document;
        }

        /// <summary>
        /// 获取指定目录id的文档列表。
        /// 作者：董帅 创建时间：2013-6-27 15:29:29
        /// </summary>
        /// <param name="catalogId">目录id，顶级目录为-1。</param>
        /// <returns>成功返回文档列表，出错返回null。</returns>
        public List<DocumentInfo> getDocumentsByParentId(int catalogId)
        {
            List<DocumentInfo> documents = null;

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
                        par.add("@catalogId", catalogId);
                        if (catalogId == -1)
                        {
                            sql = "SELECT * FROM DOC_DOCUMENT WHERE CATALOGID IS NULL ORDER BY CREATETIME DESC";
                        }
                        else
                        {
                            sql = "SELECT * FROM DOC_DOCUMENT WHERE CATALOGID = @catalogId ORDER BY CREATETIME DESC";
                        }
                        //获取数据
                        DataTable dt = this._docDataBase.executeSqlReturnDt(sql, par);
                        if (dt != null)
                        {
                            documents = new List<DocumentInfo>();
                            foreach (DataRow r in dt.Rows)
                            {
                                DocumentInfo c = this.getDocumentFromDataRow(r);



                                if (c != null)
                                {
                                    documents.Add(c);
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

            return documents;
        }

        /// <summary>
        /// 修改指定的文档信息，通过文档id匹配。
        /// </summary>
        /// <param name="document">要修改的文档。</param>
        /// <returns>成功返回true，否则返回false。</returns>
        public bool changeDocument(DocumentInfo document)
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
                        par.add("@documentId", document.id);
                        par.add("@documentTitle", document.title);

                        sql = "UPDATE DOC_DOCUMENT SET TITLE = @documentTitle WHERE ID = @documentId";

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

        /// <summary>
        /// 删除指定的文档。
        /// 作者：董帅 创建时间：2013-6-27 22:31:54
        /// </summary>
        /// <param name="docIds">文档id</param>
        /// <returns>成功返回true，否则返回false。</returns>
        public bool deleteDocuments(int[] docIds)
        {
            bool bRet = true;
            try
            {
                //连接数据库
                if (this._docDataBase.connectDataBase())
                {
                    //删除文档
                    if (docIds.Length > 0)
                    {
                        YParameters par = new YParameters();
                        string ids = "@id0";
                        par.add("@id0", docIds[0]);
                        for (int i = 1; i < docIds.Length; i++)
                        {
                            ids += ",@id" + i.ToString();
                            par.add("@id" + i.ToString(),docIds[i]);
                        }

                        string sql = "DELETE FROM DOC_DOCUMENT WHERE ID IN (" + ids + ")";
                        
                        if (this._docDataBase.executeSqlWithOutDs(sql, par) < 0)
                        {
                            bRet = false;
                        }
                    }
                }
                else
                {
                    this._errorMessage = "连接数据库出错！错误信息[" + this._docDataBase.errorText + "]";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this._docDataBase.disconnectDataBase();
            }

            return bRet;
        }

        /// <summary>
        /// 删除指定目录的文档。
        /// 作者：董帅 创建时间：2013-6-27 23:32:32
        /// </summary>
        /// <param name="catalogId">目录id</param>
        /// <param name="db">数据库连接</param>
        /// <returns>成功返回true，否则返回false。</returns>
        public bool deleteDocumentsByCatalogId(int catalogId,YDataBase db)
        {
            bool bRet = true;
            try
            {
                YParameters par = new YParameters();
                par.add("@catalogId",catalogId);

                string sql = "DELETE FROM DOC_DOCUMENT WHERE CATALOGID = @catalogId";

                if (db.executeSqlWithOutDs(sql, par) < 0)
                {
                    bRet = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bRet;
        }
    }
}
