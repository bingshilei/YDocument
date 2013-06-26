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

                this.bindCatalogs();
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
                    //获取父字典项
                    if (this.hidParentId.Value == "-1")
                    {
                        this.backButton.Disabled = true;
                    }
                    else
                    {
                        CatalogInfo catalog = docOper.getGatalog(Convert.ToInt32(this.hidParentId.Value));
                        this.backButton.InnerText = catalog.name;
                        this.hidReturnId.Value = catalog.parentId.ToString();
                    }

                    //获取菜单列表
                    List<CatalogInfo> catalogs = docOper.getGatalogsByParentId(Convert.ToInt32(this.hidParentId.Value));
                    if (catalogs != null)
                    {
                        this.catalogList.DataSource = catalogs;
                        this.catalogList.DataBind();
                    }
                    else
                    {
                        YMessageBox.show(this, "获取字典数据失败！错误信息[" + docOper.errorMessage + "]");
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
    }
}