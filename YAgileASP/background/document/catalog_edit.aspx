<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="catalog_edit.aspx.cs" Inherits="YAgileASP.background.document.catalog_edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>编辑目录</title>
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" ontent="no-cache" />  
    <meta http-equiv="expires" content="0" />  

    <style type="text/css">
        html,body{ height:100%;}
    </style>

    <link href="../../js/jquery-easyui/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../../js/jquery-easyui/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../css/table.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="../../js/jquery/jquery.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-easyui/jquery.easyui.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-easyui/locale/easyui-lang-zh_CN.js"></script>
    <script type="text/javascript" src="../../js/YWindows.js"></script>

    <script type="text/javascript">
        /*!
        * \brief
        * 验证表单。
        * 作者：董帅 创建时间：2012-8-28 17:26:01
        */
        function checkForms()
        {
            if (!$("#txtCatalogName").validatebox("isValid"))
            {
                return false;
            }

            return true;
        }

        /*!
        * \brief
        * 动态调整layout。
        */
        $(function ()
        {
            $(window).resize(function ()
            {
                $('form#form1').height($(window).height());
                $('form#form1').width($(window).width());
                $('form#form1').height($(window).height());
                $('form#form1').layout();
            });
        });
    </script>
</head>
<body style="width:100%;margin:0px;background-color:#EEF5FD;">
    <form id="form1" runat="server" class="easyui-layout" flt="true" style="width:100%;height:100%;margin:0px;background-color:#EEF5FD;">
    <input type="hidden" name="hidCatalogId" id="hidCatalogId" runat="server" value="" />
    <input type="hidden" name="hidParentId" id="hidParentId" runat="server" value="" />
    <div id="center" region="center" style="padding:1px;background-color:#EEF5FD">
        <table class="editTable" style="width:100%;">
            <tr>
                <th style="width:120px">名称：</th>
                <td><input type="text" id="txtCatalogName" name="txtCatalogName" class="easyui-validatebox" required="true" maxlength="100" runat="server" style="width:200px" /></td>
            </tr>
        </table>
    </div>
    <div region="south" border="true" style="height:35px;background:#D9E5FD;padding:3px;">
	    <div style="width:170px;margin-left:auto;margin-right:5px">
            <a id="A1" href="#" class="easyui-linkbutton" iconCls="icon-save" runat="server" onclick="javascript:return checkForms();" onserverclick="butSave_Click" >保存</a>
            <a href="#" class="easyui-linkbutton" iconCls="icon-cancel" onclick="javascript:window.parent.closePopupsWindow('#popups')">取消</a>
        </div>
	</div>
    </form>
</body>
</html>