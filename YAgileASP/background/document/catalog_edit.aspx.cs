using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YLR.YDocumentDB;
using YLR.YMessage;
using YLR.YSystem.Organization;
using YAgileASP.background.sys;

namespace YAgileASP.background.document
{
    public partial class catalog_edit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //获取父id
                string parentId = Request.QueryString["parentId"];
                if (!string.IsNullOrEmpty(parentId))
                {
                    this.hidParentId.Value = parentId;
                }
                else
                {
                    this.hidParentId.Value = "-1";
                }

                //获取id
                string strId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(strId))
                {
                    this.hidCatalogId.Value = strId;

                    //获取配置文件路径。
                    string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                    //创建操作对象
                    DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                    if (docOper != null)
                    {
                        ////获取字典项信息
                        CatalogInfo catalog = docOper.getGatalog(Convert.ToInt32(strId));
                        if (catalog != null)
                        {
                            this.txtCatalogName.Value = catalog.name;
                        }
                        else
                        {
                            YMessageBox.show(this, "获取目录信息失败！错误信息[" + docOper.errorMessage + "]");
                            return;
                        }
                    }
                    else
                    {
                        YMessageBox.show(this, "创建数据库操作对象失败！");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 保存数据。
        /// 作者：董帅 创建时间：2013-6-26 11:24:42
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void butSave_Click(object sender, EventArgs e)
        {
            try
            {
                UserInfo user = (UserInfo)Session["UserInfo"];

                if (user == null)
                {
                    YMessageBox.show(this, "获取用户登陆信息失败，用户未登陆或已超时，请重新登陆！");
                    return;
                }

                CatalogInfo catalog = new CatalogInfo();

                catalog.name = this.txtCatalogName.Value;
                if (string.IsNullOrEmpty(catalog.name) || catalog.name.Length > 200)
                {
                    YMessageBox.show(this, "名称不合法！");
                    return;
                }

                catalog.user = user;
                catalog.parentId = Convert.ToInt32(this.hidParentId.Value);

                //获取配置文件路径。
                string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                //创建操作对象
                DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                if (docOper != null)
                {
                    if (string.IsNullOrEmpty(this.hidCatalogId.Value))
                    {
                        //新增
                        if (docOper.createNewCatalog(catalog) > 0)
                        {
                            YMessageBox.showAndResponseScript(this, "保存成功！", "", "window.parent.menuButtonOnClick('文档管理','icon-docManage','document/document_list.aspx?parentId=" + this.hidParentId.Value + "');window.parent.closePopupsWindow('#popups');");
                        }
                        else
                        {
                            YMessageBox.show(this, "创建目录失败！错误信息：[" + docOper.errorMessage + "]");
                            return;
                        }
                    }
                    else
                    {
                        //修改
                        catalog.id = Convert.ToInt32(this.hidCatalogId.Value);
                        if (docOper.changeCatalog(catalog))
                        {
                            YMessageBox.showAndResponseScript(this, "保存成功！", "", "window.parent.menuButtonOnClick('文档管理','icon-docManage','document/document_list.aspx?parentId=" + this.hidParentId.Value + "');window.parent.closePopupsWindow('#popups');");
                        }
                        else
                        {
                            YMessageBox.show(this, "修改目录失败！错误信息：[" + docOper.errorMessage + "]");
                            return;
                        }
                    }
                }
                else
                {
                    YMessageBox.show(this, "创建数据库操作对象失败！");
                    return;
                }
            }
            catch (Exception ex)
            {
                YMessageBox.show(this, "程序异常！错误信息[" + ex.Message + "]");
            }
        }
    }
}