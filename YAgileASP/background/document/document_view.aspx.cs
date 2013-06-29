using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YLR.YDocumentDB;
using YAgileASP.background.sys;
using YLR.YMessage;

namespace YAgileASP.background.document
{
    public partial class document_view : System.Web.UI.Page
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
                                this.txtDocumentHtml.InnerHtml = document.html;
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
    }
}