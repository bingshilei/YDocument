using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAgileASP.background.sys;
using YLR.YDocumentDB;
using YLR.YMessage;

namespace YAgileASP.background.document
{
    public partial class document_list : System.Web.UI.Page
    {
        protected string _catalogName = "/"; //目录名称

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //获取父id。
                string parentId = Request.QueryString["parentId"];
                if (!string.IsNullOrEmpty(parentId))
                {
                    this.hidParentId.Value = parentId;
                }
                else
                {
                    this.hidParentId.Value = "-1";
                }

                //获取页号
                string pageNum = Request.QueryString["pageNum"];
                if (!string.IsNullOrEmpty(pageNum))
                {
                    this.YPagerControl1.PageNum = Convert.ToInt32(pageNum);
                }

                this.bindCatalogs();
                this.bindDocuments();
            }
        }

        /// <summary>
        /// 绑定目录。
        /// </summary>
        private void bindCatalogs()
        {
            try
            {
                //获取配置文件路径。
                string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                //获取数据库操作对象
                DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                if (docOper != null)
                {
                    //获取父目录
                    if (this.hidParentId.Value == "-1")
                    {
                        this.backButton.Disabled = true;
                        this.backButton.InnerText = "/";
                    }
                    else
                    {
                        CatalogInfo catalog = docOper.getGatalog(Convert.ToInt32(this.hidParentId.Value));
                        this.backButton.InnerText = "/" + catalog.name;
                        this._catalogName += catalog.name;
                        this.hidReturnId.Value = catalog.parentId.ToString();
                    }

                    //获取目录列表
                    List<CatalogInfo> catalogs = docOper.getGatalogsByParentId(Convert.ToInt32(this.hidParentId.Value));
                    if (catalogs != null)
                    {
                        this.catalogList.DataSource = catalogs;
                        this.catalogList.DataBind();
                    }
                    else
                    {
                        YMessageBox.show(this, "获取目录数据失败！错误信息[" + docOper.errorMessage + "]");
                    }
                }
                else
                {
                    YMessageBox.show(this, "获取数据库操作对象失败！");
                }
            }
            catch (Exception ex)
            {
                YMessageBox.show(this, "运行错误！错误信息[" + ex.Message + "]");
            }
        }

        /// <summary>
        /// 绑定文档。
        /// </summary>
        private void bindDocuments()
        {
            try
            {
                //获取配置文件路径。
                string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                //获取数据库操作对象
                DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                if (docOper != null)
                {
                    //获取菜单列表
                    List<DocumentInfo> documents = docOper.getDocumentsByParentId(Convert.ToInt32(this.hidParentId.Value));
                    if (documents != null)
                    {
                        //分页显示。
                        this.YPagerControl1.PageCount = documents.Count / this.YPagerControl1.DataCount;
                        if (documents.Count % this.YPagerControl1.DataCount > 0)
                        {
                            this.YPagerControl1.PageCount++;
                        }

                        List<DocumentInfo> showDocs = new List<DocumentInfo>();
                        while (showDocs.Count < this.YPagerControl1.DataCount
                            && (this.YPagerControl1.PageNum - 1) * this.YPagerControl1.DataCount + showDocs.Count < documents.Count)
                        {
                            showDocs.Add(documents[(this.YPagerControl1.PageNum - 1) * this.YPagerControl1.DataCount + showDocs.Count]);
                        }

                        this.documentList.DataSource = showDocs;
                        this.documentList.DataBind();
                    }
                    else
                    {
                        YMessageBox.show(this, "获取文档数据失败！错误信息[" + docOper.errorMessage + "]");
                    }
                }
                else
                {
                    YMessageBox.show(this, "获取数据库操作对象失败！");
                }
            }
            catch (Exception ex)
            {
                YMessageBox.show(this, "运行错误！错误信息[" + ex.Message + "]");
            }
        }

        protected void YPagerControl1_PageChanged(object sender, EventArgs e)
        {
            this.bindDocuments();
        }

        /// <summary>
        /// 删除文档
        /// 作者：董帅 创建时间：2013-6-27 22:28:26
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butDeleteDocuments_Click(object sender, EventArgs e)
        {
            try
            {
                string s = Request["chkDocument"];
                string[] docIds = new string[0];
                if (!string.IsNullOrEmpty(s))
                {
                    docIds = s.Split(','); //要删除的文档id
                }

                if (docIds.Length > 0)
                {
                    //获取配置文件路径。
                    string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                    //创建数据库操作对象。
                    DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                    if (docOper != null)
                    {
                        //删除文档
                        int[] dicIntIds = new int[docIds.Length];
                        for (int i = 0; i < docIds.Length; i++)
                        {
                            dicIntIds[i] = Convert.ToInt32(docIds[i]);
                        }

                        if (docOper.deleteDocuments(dicIntIds))
                        {
                            this.Response.Redirect("document_list.aspx?parentId=" + this.hidParentId.Value + "&pageNum=" + this.YPagerControl1.PageNum.ToString());
                            //YMessageBox.showAndResponseScript(this, "删除数据成功！", "", "window.location.href='dataDictionary_list.aspx?parentId=" + this.hidParentId.Value + "'");
                        }
                        else
                        {
                            YMessageBox.show(this, "删除数据失败！错误信息[" + docOper.errorMessage + "]");
                        }
                    }
                    else
                    {
                        YMessageBox.show(this, "获取数据库实例失败！");
                    }
                }
                else
                {
                    YMessageBox.show(this, "没有选择要删除的字典！");
                }
            }
            catch (Exception ex)
            {
                YMessageBox.show(this, "系统运行异常！异常信息[" + ex.Message + "]");
            }
        }

        /// <summary>
        /// 删除目录
        /// 作者：董帅 创建时间：2013-6-27 23:54:32
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butDeleteCatalogs_Click(object sender, EventArgs e)
        {
            try
            {
                string s = Request["chkCatalog"];
                string[] catalogIds = new string[0];
                if (!string.IsNullOrEmpty(s))
                {
                    catalogIds = s.Split(','); //要删除的目录id
                }

                if (catalogIds.Length > 0)
                {
                    //获取配置文件路径。
                    string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                    //创建数据库操作对象。
                    DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                    if (docOper != null)
                    {
                        //删除文档
                        int[] dicIntIds = new int[catalogIds.Length];
                        for (int i = 0; i < catalogIds.Length; i++)
                        {
                            dicIntIds[i] = Convert.ToInt32(catalogIds[i]);
                        }

                        if (docOper.deleteCatalogs(dicIntIds))
                        {
                            this.Response.Redirect("document_list.aspx?parentId=" + this.hidParentId.Value);
                            //YMessageBox.showAndResponseScript(this, "删除数据成功！", "", "window.location.href='dataDictionary_list.aspx?parentId=" + this.hidParentId.Value + "'");
                        }
                        else
                        {
                            YMessageBox.show(this, "删除数据失败！错误信息[" + docOper.errorMessage + "]");
                        }
                    }
                    else
                    {
                        YMessageBox.show(this, "获取数据库实例失败！");
                    }
                }
                else
                {
                    YMessageBox.show(this, "没有选择要删除的字典！");
                }
            }
            catch (Exception ex)
            {
                YMessageBox.show(this, "系统运行异常！异常信息[" + ex.Message + "]");
            }
        }
    }
}