<%@ Page Language="C#" AutoEventWireup="true" validateRequest="false" CodeBehind="document_edit.aspx.cs" Inherits="YAgileASP.background.document.document_edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>编辑文档</title>
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
    <script src="../../ueditor/ueditor.all.js" type="text/javascript"></script>
    <script src="../../ueditor/ueditor.config.js" type="text/javascript"></script>

    <script type="text/javascript">
        function saveDocument()
        {
            $("#hidHtmlText").val(editor.getContent());
            $("#hidPlanText").val(editor.getPlainTxt());
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

        $(document).ready(function ()
        {
            alert($("#documentEditor").val());
            var editor = new baidu.editor.ui.Editor();
            editor.render("documentEditor");
        }); 
    </script>
</head>
<body style="width:100%;margin:0px;background-color:#EEF5FD;">
    <form id="form1" runat="server" class="easyui-layout" flt="true" style="width:100%;height:100%;margin:0px;background-color:#EEF5FD;">
    <div id="center" region="center" style="padding:1px;background-color:#EEF5FD">
    
    <input type="hidden" name="hidDocumentId" id="hidDocumentId" runat="server" value="" />
    <table class="editTable" style="width:100%;">
        <tr>
            <th style="width:120px">标题：</th>
            <td colspan="3" style="width:600px"><span id="txtDocumentTitle" name="txtDocumentTitle" runat="server" style="width:500px"></span></td>
        </tr>
        <tr>
            <th style="width:120px">作者：</th>
            <td style="width:300px"><span id="txtUserName" name="txtUserName" runat="server" style="width:300px"></span></td>
            <th style="width:120px">创建时间：</th>
            <td style="width:300px"><span id="txtCreateTime" name="txtCreateTime" runat="server" style="width:300px"></span></td>
        </tr>
        <tr>
            <td  colspan="4" style="width:980px;height:800px">
                <textarea id="documentEditor" runat="server" style="width:980px;height:800px"></textarea>
            </td>
        </tr>
    </table>
    </div>
    <input type="hidden" id="hidHtmlText" name="hidHtmlText" runat="server" />
    <input type="hidden" id="hidPlanText" name="hidPlanText" runat="server" />
    <div region="south" border="true" style="height:30px;background:#D9E5FD;padding:1px;">
	    <div style="width:170px;margin-left:auto;margin-right:5px">
            <a id="A1" href="#" class="easyui-linkbutton" iconCls="icon-save" runat="server" onclick="javascript:saveDocument();" onserverclick="butSave_Click" >保存</a>
            <a href="#" class="easyui-linkbutton" iconCls="icon-cancel" onclick="javascript:window.parent.closePopupsWindow('#popups')">取消</a>
        </div>
	</div>
    </form>
</body>
</html>