<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="document_list.aspx.cs" Inherits="YAgileASP.background.document.document_list" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>文档管理</title>
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" ontent="no-cache">  
    <meta http-equiv="expires" content="0">  

    <style type="text/css">
        html,body{ height:100%;}
    </style>

    <link href="../../js/jquery-easyui/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../../js/jquery-easyui/themes/default/easyui.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="../../js/jquery/jquery.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-easyui/jquery.easyui.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-easyui/locale/easyui-lang-zh_CN.js"></script>
    <script type="text/javascript" src="../../js/YWindows.js"></script>

    <script type="text/javascript">
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
<body  style="width:100%;margin:0px;background-color:#EEF5FD;">
    <form id="form1" runat="server" class="easyui-layout" flt="true" style="width:100%;height:100%;margin:0px;background-color:#EEF5FD;">
    <div region="west" style="width:250px;padding:0px;background-color:#EEF5FD">
        <div class="easyui-panel" iconCls="icon-folder" title="目录" tools="#folderButton" flt="true" border="false">
        sdfdsf
        </div>
        <div id="folderButton">
		    <a href="javascript:void(0)" class="icon-add" onclick="javascript:alert('add')"></a>
		    <a href="javascript:void(0)" class="icon-edit" onclick="javascript:alert('edit')"></a>
		    <a href="javascript:void(0)" class="icon-cancel" onclick="javascript:alert('cut')"></a>
	    </div>
    </div>
    <div id="center" region="center" title="文档" iconCls="icon-doc" style="padding:1px;background-color:#EEF5FD">
    dd
    </div>
    </form>
</body>
</html>
