using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YLR.YSystem.Organization;
using YLR.YMessage;
using YLR.YDocumentDB;
using YAgileASP.background.sys;

namespace YAgileASP.background.document
{
    public partial class documentInfo_edit : System.Web.UI.Page
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

                ////获取id
                //string strId = Request.QueryString["id"];
                //if (!string.IsNullOrEmpty(strId))
                //{
                //    this.hidCatalogId.Value = strId;

                //    //获取配置文件路径。
                //    string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                //    //创建操作对象
                //    DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                //    if (docOper != null)
                //    {
                //        ////获取字典项信息
                //        CatalogInfo catalog = docOper.getGatalog(Convert.ToInt32(strId));
                //        if (catalog != null)
                //        {
                //            this.txtCatalogName.Value = catalog.name;
                //        }
                //        else
                //        {
                //            YMessageBox.show(this, "获取目录信息失败！错误信息[" + docOper.errorMessage + "]");
                //            return;
                //        }
                //    }
                //    else
                //    {
                //        YMessageBox.show(this, "创建数据库操作对象失败！");
                //        return;
                //    }
                //}
            }
        }

        /// <summary>
        /// 保存数据。
        /// 作者：董帅 创建时间：2013-6-27 10:13:39
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

                DocumentInfo document = new DocumentInfo();

                document.title = this.txtDocumentTitle.Value;
                if (string.IsNullOrEmpty(document.title) || document.title.Length > 200)
                {
                    YMessageBox.show(this, "标题不合法！");
                    return;
                }

                document.user = user;
                document.catalogId = Convert.ToInt32(this.hidParentId.Value);

                //获取配置文件路径。
                string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                //创建操作对象
                DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                if (docOper != null)
                {
                    if (string.IsNullOrEmpty(this.hidCatalogId.Value))
                    {
                        //新增
                        if (docOper.createNewDocument(document) > 0)
                        {
                            YMessageBox.showAndResponseScript(this, "保存成功！", "", "window.parent.menuButtonOnClick('文档管理','icon-docManage','document/document_list.aspx?parentId=" + this.hidParentId.Value + "');window.parent.closePopupsWindow('#popups');");
                        }
                        else
                        {
                            YMessageBox.show(this, "创建文档失败！错误信息：[" + docOper.errorMessage + "]");
                            return;
                        }
                    }
                    else
                    {
                        //修改
                        document.id = Convert.ToInt32(this.hidCatalogId.Value);
                        //if (docOper.changeCatalog(document))
                        //{
                        //    YMessageBox.showAndResponseScript(this, "保存成功！", "", "window.parent.menuButtonOnClick('文档管理','icon-docManage','document/document_list.aspx?parentId=" + this.hidParentId.Value + "');window.parent.closePopupsWindow('#popups');");
                        //}
                        //else
                        //{
                        //    YMessageBox.show(this, "修改目录失败！错误信息：[" + docOper.errorMessage + "]");
                        //    return;
                        //}
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