<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="document_list.aspx.cs" Inherits="YAgileASP.background.document.document_list" %>

<%@ Register assembly="YAgileControls" namespace="YLR.YAgileControls.PagerControl" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>文档管理</title>
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" ontent="no-cache" />  
    <meta http-equiv="expires" content="0" />  

    <style type="text/css">
        html,body{ height:100%;}
    </style>

    <link href="../../js/jquery-easyui/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../../js/jquery-easyui/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../css/table.css" rel="Stylesheet" type="text/css" />

    <script type="text/javascript" src="../../js/jquery/jquery.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-easyui/jquery.easyui.min.js"></script>
    <script type="text/javascript" src="../../js/jquery-easyui/locale/easyui-lang-zh_CN.js"></script>
    <script type="text/javascript" src="../../js/YWindows.js"></script>

    <script type="text/javascript">
        /*!
         * \brief
         * 新增目录。
         */
        function addCatalog()
        {
            window.parent.popupsWindow("#popups", "新增目录", 600, 120, "document/catalog_edit.aspx?parentId=" + +$("#hidParentId").val() + "&pageNum=<%=this.YPagerControl1.PageNum %>", "icon-add", true, true);
        }

        /*!
         * \brief
         * 编辑目录。
         */
        function editCatalog()
        {
            //判断选中
            if ($("input:checked[type='checkbox'][name='chkCatalog']").length != 1)
            {
                alert("请选中要编辑的目录项，一次只能选择一个！");
                return;
            }

            window.parent.popupsWindow("#popups", "编辑目录", 600, 120, "document/catalog_edit.aspx?parentId=" + +$("#hidParentId").val() + "&id=" + $("input:checked[type='checkbox'][name='chkCatalog']").eq(0).val() + "&pageNum=<%=this.YPagerControl1.PageNum %>", "icon-edit", true, true);
        }

        /*!
        * \brief
        * 删除目录。
        * 作者：董帅 创建时间：2013-6-27 22:29:26
        */
        function deleteCatalogs()
        {
            //判断选中
            if ($("input:checked[type='checkbox'][name='chkCatalog']").length > 0)
            {
                return confirm("确认要删除选中的目录？删除时将连同子目录和文档一并删除！");
            }
            else
            {
                alert("请选中要删除的目录！");
                return false;
            }
        }

        /*!
        * \brief
        * 新增文档。
        */
        function addDocument()
        {
            window.parent.popupsWindow("#popups", "新增文档", 800, 120, "document/documentInfo_edit.aspx?parentId=" + +$("#hidParentId").val(), "icon-add", true, true);
        }

        /*!
        * \brief
        * 编辑文档信息。
        */
        function editDocumentInfo()
        {
            //判断选中
            if ($("input:checked[type='checkbox'][name='chkDocument']").length != 1)
            {
                alert("请选中要编辑的文档，一次只能选择一个！");
                return;
            }

            window.parent.popupsWindow("#popups", "编辑文档信息", 800, 120, "document/documentInfo_edit.aspx?parentId=" + +$("#hidParentId").val() + "&id=" + $("input:checked[type='checkbox'][name='chkDocument']").eq(0).val() + "&pageNum=<%=this.YPagerControl1.PageNum %>", "icon-edit", true, true);
        }

        /*!
        * \brief
        * 删除文档。
        * 作者：董帅 创建时间：2013-6-27 22:29:26
        */
        function deleteDocuments()
        {
            //判断选中
            if ($("input:checked[type='checkbox'][name='chkDocument']").length > 0)
            {
                return confirm("确认要删除选中的文档？");
            }
            else
            {
                alert("请选中要删除的文档！");
                return false;
            }
        }

        /*!
         * \brief
         * 返回上级目录。
         */
        function returnParent()
        {
            window.parent.menuButtonOnClick('文档管理', 'icon-docManage', 'document/document_list.aspx?parentId=' + $("#hidReturnId").val()); 
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
<body  style="width:100%;margin:0px;background-color:#EEF5FD;">
    <form id="form1" runat="server" class="easyui-layout" flt="true" style="width:100%;height:100%;margin:0px;background-color:#EEF5FD;">
    <input type="hidden" name="hidParentId" id="hidParentId" runat="server" value="-1" />
    <input type="hidden" name="hidReturnId" id="hidReturnId" runat="server" value="-1" />
    <div region="west" style="width:250px;padding:0px;background-color:#EEF5FD">
        <div class="easyui-panel" title="目录列表" tools="#folderButton" flt="true" border="false" style="padding:2px;background-color:#EEF5FD">
            <table class="listTable" style="width:245px">
                <tr style="width:100%">
                    <td colspan="2"><a href="#" class="easyui-linkbutton" id="backButton" name="backButton" runat="server" iconCls="icon-back" plain="true" style="width:234px" onclick="javascript:returnParent();"></a></td>
                </tr>
                <asp:Repeater ID="catalogList" runat="server">
                    <ItemTemplate>
                        <tr class="tableBody1">
                            <td style="text-align:center;"><input type="checkbox" value="<%#Eval("ID") %>" name="chkCatalog" /></td>
                            <td style="width:auto">
                                <a href="document_list.aspx?parentId=<%#Eval("ID") %>" class="easyui-linkbutton" iconCls="icon-folder" id="<%#Eval("ID") %>" plain="true" style="width:210px" ><%#Eval("NAME")%></a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    </asp:Repeater>
            </table>
        </div>
        <div id="folderButton">
		    <a href="javascript:void(0)" class="icon-add" onclick="javascript:addCatalog()"></a>
		    <a href="javascript:void(0)" class="icon-edit" onclick="javascript:editCatalog()"></a>
		    <a href="javascript:void(0)" class="icon-cancel" onclick="javascript:return deleteCatalogs();" runat="server" onserverclick="butDeleteCatalogs_Click"></a>
	    </div>
    </div>
    <div id="center" region="center" title="<%=_catalogName%>" iconCls="icon-folder" style="padding:1px;background-color:#EEF5FD">
        <div class="easyui-layout" data-options="fit:true">
			<div data-options="region:'north',split:false,border:true" style="height:30px;padding:1px;background-color:#EEF5FD">
                <div style="width:200px;margin-left:auto;margin-top:0px;margin-right:0px">
                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="javascript:addDocument();">新增</a>
                    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-edit',plain:true" onclick="javascript:editDocumentInfo();">修改</a>
                    <a id="A1" href="#" class="easyui-linkbutton" data-options="iconCls:'icon-cancel',plain:true" onclick="javascript:return deleteDocuments();" runat="server" onserverclick="butDeleteDocuments_Click">删除</a>
                </div>
            </div>
			<div data-options="region:'center',border:true" style="padding:1px;background-color:#EEF5FD">
                <table class="listTable" style="width:100%">
                    <tr class="tableHead">
                        <th style="width:30px">选择</th>
                        <th style="width:auto">标题</th>
                        <th style="width:50px">作者</th>
                        <th style="width:150px">创建时间</th>
                        <th style="width:100px">编辑</th>
                    </tr>
                <asp:Repeater ID="documentList" runat="server">
                <ItemTemplate>
                    <tr class="tableBody1">
                        <td style="text-align:center;"><input type="checkbox" value="<%#Eval("ID") %>" name="chkDocument" /></td>
                        <td style="width:auto">
                            <a href="dataDictionary_list.aspx?parentId=<%#Eval("ID") %>" class="easyui-linkbutton" data-options="iconCls:'icon-doc',plain:true" id="<%#Eval("ID") %>" style="width:99%" ><%#Eval("TITLE")%></a>
                        </td>
                        <td style="text-align:center">
                            <%#Eval("user.name")%>
                        </td>
                        <td style="text-align:center">
                            <%#Eval("CREATETIME","{0:yyyy-MM-dd HH:mm:ss}")%>
                        </td>
                        <td style="text-align:center">
                            <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-editorDocument'" onclick="javascript:addDocument();">编辑</a>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="tableBody2">
                        <td style="text-align:center;"><input type="checkbox" value="<%#Eval("ID") %>" name="chkDocument" /></td>
                        <td style="width:auto">
                            <a href="dataDictionary_list.aspx?parentId=<%#Eval("ID") %>" class="easyui-linkbutton" data-options="iconCls:'icon-doc',plain:true" id="<%#Eval("ID") %>" style="width:99%" ><%#Eval("TITLE")%></a>
                        </td>
                        <td style="text-align:center">
                            <%#Eval("user.name")%>
                        </td>
                        <td style="text-align:center">
                            <%#Eval("CREATETIME","{0:yyyy-MM-dd HH:mm:ss}")%>
                        </td>
                        <td style="text-align:center">
                            <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-editorDocument'" onclick="javascript:addDocument();">编辑</a>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                </asp:Repeater>
                </table>
            </div>
            <div data-options="region:'south',split:false,border:true" style="height:30px;text-align:center;padding:1px;background-color:#EEF5FD">
                <cc1:YPagerControl ID="YPagerControl1" runat="server" 
                    onpagechanged="YPagerControl1_PageChanged" />
            </div>
		</div>
    </div>
    </form>
</body>
</html>
