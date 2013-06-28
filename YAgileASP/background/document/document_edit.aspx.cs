using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YLR.YMessage;
using YAgileASP.background.sys;
using YLR.YDocumentDB;

namespace YAgileASP.background.document
{
    public partial class document_edit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    //获取id
                    string strId = Request.QueryString["id"];
                    if (!string.IsNullOrEmpty(strId))
                    {
                        this.hidDocumentId.Value = strId;

                        //获取配置文件路径。
                        string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                        //创建操作对象
                        DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                        if (docOper != null)
                        {
                            //获取文档
                            DocumentInfo document = docOper.getDocument(Convert.ToInt32(strId));
                            if (document != null)
                            {
                                this.txtDocumentTitle.InnerText = document.title;
                                this.txtUserName.InnerText = document.user.name;
                                this.txtCreateTime.InnerText = document.createTime.ToString("yyyy-MM-dd HH:mm:ss");
                                this.hidHtmlText.Value = document.html;
                                this.hidPlanText.Value = document.text;
                                this.documentEditor.Value = document.html;
                            }
                            else
                            {
                                YMessageBox.show(this, "获取文档信息失败！错误信息[" + docOper.errorMessage + "]");
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
            catch (Exception ex)
            {
                YMessageBox.show(this, ex.Message);
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
                //获取配置文件路径。
                string configFile = AppDomain.CurrentDomain.BaseDirectory.ToString() + SystemConfig.databaseConfigFileName;

                //创建操作对象
                DocOper docOper = DocOper.createDocOper(configFile, SystemConfig.databaseConfigNodeName, SystemConfig.configFileKey);
                if (docOper != null)
                {
                    if (!string.IsNullOrEmpty(this.hidDocumentId.Value))
                    {
                        //修改
                        if (docOper.changeDocument(Convert.ToInt32(this.hidDocumentId.Value), this.hidHtmlText.Value, this.hidPlanText.Value))
                        {
                            YMessageBox.showAndResponseScript(this, "保存成功！", "", "window.parent.closePopupsWindow('#popups');");
                        }
                        else
                        {
                            YMessageBox.show(this, "保存失败！错误信息：[" + docOper.errorMessage + "]");
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